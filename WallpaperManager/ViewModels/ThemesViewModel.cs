using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallpaperManager.Models;

namespace WallpaperManager.ViewModels
{
    public class ThemesViewModel : WallpaperManagerViewModelBase
    {
        public static ThemesViewModel Instance { get { return ServiceLocator.Current.GetInstance<ThemesViewModel>(); } }

        public ObservableCollection<WallpaperTheme> Themes { get; set; } = new ObservableCollection<WallpaperTheme>();

        public ThemesViewModel()
            : base()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            ResetViewModel();
        }

        public override void Loaded()
        {
        }

        public override void OnNavigatedFrom()
        {
        }

        public override void OnNavigatedTo()
        {
            Themes.Clear();

            var tmp = ThemeRepository.GetAll();
            foreach (var item in tmp)
            {
                Themes.Add(item);
            }
        }

        public override void ResetViewModel()
        {
            Themes.Clear();
        }
    }
}
