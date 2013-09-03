using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPToReaderClassLib
{
    public class AppSettings
    {
        // Our isolated storage settings
        IsolatedStorageSettings isolatedStore;

        public AppSettings()
        {
            try
            {
                isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured while initializing AppSettings" + e.Message);
            }
        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (isolatedStore.Contains(Key))
            {
                // If the value has changed
                if (isolatedStore[Key] != value)
                {
                    // Store the new value
                    isolatedStore[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                isolatedStore.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }


        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (isolatedStore.Contains(Key))
            {
                value = (valueType)isolatedStore[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }


        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            isolatedStore.Save();
        }

        string UserNameKey = "UserName";
        string PasswordKey = "Password";
        string BgTaskEnableKey = "BgTaskEnable";
        string defaultVal = "";

        /// <summary>
        /// Property to get and set a Username Setting Key.
        /// </summary>
        public string UserName
        {
            get
            {
                return GetValueOrDefault<string>(UserNameKey, defaultVal);
            }
            set
            {
                AddOrUpdateValue(UserNameKey, value);
                Save();
            }
        }

        public string Password
        {
            get
            {
                byte[] password = null;
                password = GetValueOrDefault<byte[]>(PasswordKey, password);
                if (null != password)
                {
                    byte[] passByte = ProtectedData.Unprotect(password, null);
                    return Encoding.UTF8.GetString(passByte, 0, passByte.Length);
                }
                else
                    return "";
            }
            set
            {
                byte[] byPass = Encoding.UTF8.GetBytes(value);
                if (null != byPass)
                {
                    AddOrUpdateValue(PasswordKey, ProtectedData.Protect(byPass, null));
                }
                else
                {
                    AddOrUpdateValue(PasswordKey, byPass);
                }
                Save();
            }
        }

        public bool BgTaskEnabled
        {
            get
            {
                return GetValueOrDefault<bool>(BgTaskEnableKey, false);
            }
            set
            {
                AddOrUpdateValue(BgTaskEnableKey, value);
                Save();
            }
        }

    }
}
