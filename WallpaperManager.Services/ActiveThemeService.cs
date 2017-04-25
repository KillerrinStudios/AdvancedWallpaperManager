using KillerrinStudiosToolkit.Services;
using KillerrinStudiosToolkit.UserProfile;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using WallpaperManager.DAL.Repositories;

namespace WallpaperManager.Services
{
    public class ActiveThemeService : ServiceBase
    {
        KillerrinStudiosToolkit.UserProfile.WallpaperManager m_wallpaperManager = new KillerrinStudiosToolkit.UserProfile.WallpaperManager();
        ActiveDesktopThemeSetting m_activeDesktopThemeSetting = new ActiveDesktopThemeSetting();
        ActiveDesktopThemeHistorySetting m_activeDesktopThemeHistorySetting = new ActiveDesktopThemeHistorySetting();

        LockscreenManager m_lockscreenManager = new LockscreenManager();
        ActiveLockscreenThemeSetting m_activeLockscreenThemeSetting = new ActiveLockscreenThemeSetting();
        ActiveLockscreenThemeHistorySetting m_activeLockscreenThemeHistorySetting = new ActiveLockscreenThemeHistorySetting();

        public ActiveThemeService()
        {

        }

        #region Change
        public void ChangeActiveDesktopTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            m_activeDesktopThemeSetting.Value = theme.ID;
            m_activeDesktopThemeHistorySetting.RevertToDefault();
            m_wallpaperManager.DeleteFilesInFolders();
        }
        public void ChangeActiveLockscreenTheme(WallpaperTheme theme)
        {
            if (!ServiceEnabled) return;
            m_activeLockscreenThemeSetting.Value = theme.ID;
            m_activeLockscreenThemeHistorySetting.RevertToDefault();
            m_lockscreenManager.DeleteFilesInFolders();
        }
        #endregion

        #region Deselect
        public void DeselectActiveDesktopTheme()
        {
            if (!ServiceEnabled) return;
            m_activeDesktopThemeSetting.RevertToDefault();
            m_activeDesktopThemeHistorySetting.RevertToDefault();
            m_wallpaperManager.DeleteFilesInFolders();
        }
        public void DeselectActiveLockscreenTheme()
        {
            if (!ServiceEnabled) return;
            m_activeLockscreenThemeSetting.RevertToDefault();
            m_activeLockscreenThemeHistorySetting.RevertToDefault();
            m_lockscreenManager.DeleteFilesInFolders();
        }
        #endregion

        #region Get Theme
        public int? GetActiveDesktopThemeID() { return m_activeDesktopThemeSetting.Value; }
        public WallpaperTheme GetActiveDesktopTheme()
        {
            var value = m_activeDesktopThemeSetting.Value;
            if (!value.HasValue) return null;

            return GetTheme(value.Value);
        }

        public int? GetActiveLockscreenThemeID() { return m_activeLockscreenThemeSetting.Value; }
        public WallpaperTheme GetActiveLockscreenTheme()
        {
            var value = m_activeLockscreenThemeSetting.Value;
            if (!value.HasValue) return null;

            return GetTheme(value.Value);
        }
        private WallpaperTheme GetTheme(int id)
        {
            using (var context = new WallpaperManagerContext())
            {
                var themeRepo = new WallpaperThemeRepository(context);

                try { return themeRepo.Find(m_activeLockscreenThemeSetting.Value.Value); }
                catch (Exception) { }
            }

            return null;
        } 
        #endregion
    }
}
