using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	public class KeyDisabler
	{

		public const int CAMERA_MODE = 0;
		public const int CAMERA_NEXT = 1;
		public const int MAP_VIEW = 2;

		List<KeyBinding> keyBindings = new List<KeyBinding>
		{
			GameSettings.CAMERA_MODE,
			GameSettings.CAMERA_NEXT,
			GameSettings.MAP_VIEW_TOGGLE,
		};

		static List<KeyCode> keySaver = new List<KeyCode>()
		{
			GameSettings.CAMERA_MODE.primary,
			GameSettings.CAMERA_MODE.secondary,
			GameSettings.CAMERA_NEXT.primary,
			GameSettings.CAMERA_NEXT.secondary,
			GameSettings.MAP_VIEW_TOGGLE.primary,
			GameSettings.MAP_VIEW_TOGGLE.secondary,
		};

		public bool keysDisabled;

		private KeyDisabler()
		{
		}

		static KeyDisabler inst = null;
		public static KeyDisabler initialize() {
			if (inst == null)
				inst = new KeyDisabler ();
			return inst;
		}

		/*public void disableKeys(bool disableMapView)
		{

			if (keysDisabled) {
				return;
			}

			for (int i=0; i < keyBindings.Count; i++)
			{
				KeyBinding k = keyBindings[i];
				if (disableMapView)
				{
					k.primary = KeyCode.None;
					k.secondary = KeyCode.None;
				}
				else
				{
					if (k != GameSettings.MAP_VIEW_TOGGLE)
					{
						disableMapView();
					}

				}
			}
			keysDisabled = true;

		}*/

		public void restoreAllKeys()
		{
			for (int i=0; i < keyBindings.Count; i++)
			{
				restoreKey(i);
			}
			keysDisabled = false;
		}


		public void disableKey(int index) {
			keyBindings[index].primary = KeyCode.None;
			keyBindings[index].secondary = KeyCode.None;
		}

		public void restoreKey(int index) {
			int saverIndex = index * 2;
			keyBindings[index].primary = keySaver[saverIndex];
			keyBindings[index].secondary = keySaver[saverIndex + 1];
		}

	}
}

