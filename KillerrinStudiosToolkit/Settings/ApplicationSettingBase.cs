using KillerrinStudiosToolkit.Models;
using Windows.Storage;

namespace KillerrinStudiosToolkit.Settings
{
    public abstract class ApplicationSettingBase<T> : ModelBase, IApplicationSetting<T>
    {
        public ApplicationDataContainer Container { get; protected set; }

        public string Key { get; protected set; }
        public T DefaultValue { get; set; }
        public virtual T Value
        {
            get { return (T)Container.Values[Key]; }
            set
            {
                Container.Values[Key] = value;
                RaisePropertyChanged(nameof(Value));
            }
        }


        public ApplicationSettingBase(ApplicationDataContainer container, string key, T defaultValue)
            : this(container, key, defaultValue, defaultValue)
        {
        }
        public ApplicationSettingBase(ApplicationDataContainer container, string key, T defaultValue, T value)
        {
            Container = container;
            Key = key;
            DefaultValue = defaultValue;

            if (!Container.Values.ContainsKey(Key))
                Value = value;
        }

        public void RevertToDefault()
        {
            Value = DefaultValue;
        }

        public virtual void RaiseValuePropertyChanged()
        {
            RaisePropertyChanged(nameof(Value));
        }
        public void RaiseDefaultValuePropertyChanged()
        {
            RaisePropertyChanged(nameof(DefaultValue));
        }

        public override string ToString()
        {
            return $"Application Setting: {Key} | {Value.ToString()}";
        }
    }

}
