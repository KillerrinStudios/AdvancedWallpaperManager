﻿using Windows.Storage;

namespace WallpaperManager.Killerrin_Studios_Toolkit.Settings
{
    public class GenericApplicationSetting<T> : ApplicationSettingBase<T>
    {
        public GenericApplicationSetting(ApplicationDataContainer container, string key, T defaultValue)
            : this(container, key, defaultValue, defaultValue)
        {
        }
        public GenericApplicationSetting(ApplicationDataContainer container, string key, T defaultValue, T value)
            :base(container, key, defaultValue, value)
        {
        }
    }

}
