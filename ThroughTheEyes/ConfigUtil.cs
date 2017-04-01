using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FirstPerson
{

    public static class ConfigUtil
    {

		static string configPath = KSP.IO.IOUtils.GetFilePathFor(typeof(ThroughTheEyes), "options.cfg").Replace("/", System.IO.Path.DirectorySeparatorChar.ToString());
        static ConfigNode cfg;


        private static void checkConfig()
        {
			cfg = ConfigNode.Load(configPath);
            if (cfg == null)
            {
                cfg = new ConfigNode();
				cfg.Save(configPath);
                Debug.Log("No config found. Writing one.");
            }
            if (!cfg.HasValue("disableMapView"))
            {
                cfg.AddValue("disableMapView", "false");
				cfg.Save(configPath);
                Debug.Log("No disableMapView value found. Adding one.");
            }

            if (!cfg.HasValue("forceIVA"))
            {
                cfg.AddValue("forceIVA", "false");
				cfg.Save(configPath);
                Debug.Log("No forceIVA value found. Making one");
            }
            if (!cfg.HasValue("forceEVA"))
            {
                cfg.AddValue("forceEVA", "false");
				cfg.Save(configPath);
                Debug.Log("No forceEVA value found. Making one");
            }
			if (!cfg.HasValue("showSightAngle"))
			{
				cfg.AddValue("showSightAngle", "true");
				cfg.Save(configPath);
				Debug.Log("No showSightAngle value found. Making one");
			}
            if (!cfg.HasValue("toggleFirstPersonKey"))
            {
                cfg.AddValue("toggleFirstPersonKey", "default");
				cfg.Save(configPath);
                Debug.Log("No toggleFirstPersonKey value found. Reverting to camera mode key");
            }

            if (!cfg.HasValue("EVAKey"))
            {
                cfg.AddValue("EVAKey", "default");
				cfg.Save(configPath);
                Debug.Log("No EVAKey value found. Reverting to camera mode key");
            }
			if (!cfg.HasValue("recoverKey"))
			{
				cfg.AddValue("recoverKey", "R");
				cfg.Save(configPath);
				Debug.Log("No recoverKey value found. Making one");
			}
            if (!cfg.HasValue("reviewDataKey"))
            {
                cfg.AddValue("reviewDataKey", "Backslash");
				cfg.Save(configPath);
                Debug.Log("No reviewDataKey value found. Adding one");
            }
        }

		public static KeyCode RecoverKey()
		{
			checkConfig();

			KeyCode key;
			try
			{
				key = (KeyCode)Enum.Parse(typeof(KeyCode), cfg.GetValue("recoverKey"));
				return key;
			}
			catch
			{
				cfg.SetValue("recoverKey", "R");
				cfg.Save(configPath);
				Debug.Log("Make sure to use the list of keys to set the key! Reverting to R");
				return KeyCode.R;

			}
		}

        public static KeyCode EVAKey(KeyCode key)
        {
            checkConfig();


            if (cfg.GetValue("EVAKey") == "default")
            {
                return key;
            }
            else
            {
                try
                {
                    key = (KeyCode)Enum.Parse(typeof(KeyCode), cfg.GetValue("EVAKey"));
                    return key;
                }
                catch
                {
                    cfg.SetValue("EVAKey", "default");
					cfg.Save(configPath);
                    Debug.Log("Set the key from the list of keys or use the string 'default'! Reverting to camera mode key");
                    return key;

                }
            }
        }

        public static KeyCode ToggleFirstPersonKey(KeyCode key)
        {
            checkConfig();


            if (cfg.GetValue("toggleFirstPersonKey") == "default")
            {
                return key;
            }
            else
            {
                try
                {
                    key = (KeyCode)Enum.Parse(typeof(KeyCode), cfg.GetValue("toggleFirstPersonKey"));
                    return key;
                }
                catch
                {
                    cfg.SetValue("toggleFirstPersonKey", "default");
					cfg.Save(configPath);
                    Debug.Log("Set the key from the list of keys or use the string 'default'! Reverting to camera mode key");
                    return key;

                }
            }
        }

        public static bool ForceEVA()
        {
            checkConfig();

            bool isForced;
            if (!Boolean.TryParse(cfg.GetValue("forceEVA"), out isForced))
            {
                cfg.SetValue("forceEVA", "false");
				cfg.Save(configPath);
                Debug.Log("Set forceIVA to all lowercase true or false! Reverting to false.");
                return false;
            }
            else
            {
                return isForced;
            }
        }

        public static bool ForceIVA()
        {
            checkConfig();

            bool isForced;
            if (!Boolean.TryParse(cfg.GetValue("forceIVA"), out isForced))
            {
                cfg.SetValue("forceIVA", "false");
				cfg.Save(configPath);
                Debug.Log("Set forceIVA to all lowercase true or false! Reverting to false.");
                return false;
            }
            else
            {
                return isForced;
            }
        }

		public static bool ShowSightAngle()
		{
			checkConfig();

			bool show;
			if (!Boolean.TryParse(cfg.GetValue("showSightAngle"), out show))
			{
				cfg.SetValue("showSightAngle", "true");
				cfg.Save(configPath);
				Debug.Log("Set showSightAngle to all lowercase true or false! Reverting to true.");
				return true;
			}
			else
			{
				return show;
			}
		}

        public static bool checkMapView()
        {
            checkConfig();

            bool disable;
            if (!Boolean.TryParse(cfg.GetValue("disableMapView"), out disable))
            {
                cfg.SetValue("disableMapView", "false");
				cfg.Save(configPath);
                Debug.Log("Make sure to set disableMapView to all lowercase true or false!");
                return false;
            }
            else
            {
                return disable;
            }

        }

        public static KeyCode checkKeys()
        {
            checkConfig();

            KeyCode key;
            try
            {
                key = (KeyCode)Enum.Parse(typeof(KeyCode), cfg.GetValue("reviewDataKey"));
                return key;
            }
            catch
            {
                cfg.SetValue("reviewDataKey", "Backslash");
				cfg.Save(configPath);
                Debug.Log("Make sure to use the list of keys to set the key! Reverting to Backslash");
                return KeyCode.Backslash;
            }

        }
    }
}