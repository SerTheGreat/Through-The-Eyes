using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace FirstPerson
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class ThroughTheEyes : MonoBehaviour
    {


        bool keysDisabled, disableMapView, forceIVA;
        KeyCode EVAKey,reviewDataKey;
        CameraManager.CameraMode flight = CameraManager.CameraMode.Flight;
        CameraManager.CameraMode IVA = CameraManager.CameraMode.IVA;
        CameraManager.CameraMode map = CameraManager.CameraMode.Map;

        List<KeyBinding> keyBindings = new List<KeyBinding>
           {
               GameSettings.CAMERA_MODE,
               GameSettings.CAMERA_NEXT,
               GameSettings.MAP_VIEW_TOGGLE,
           };

        List<KeyCode> keySaver = new List<KeyCode>()
           {
               GameSettings.CAMERA_MODE.primary,
               GameSettings.CAMERA_MODE.secondary,
               GameSettings.CAMERA_NEXT.primary,
               GameSettings.CAMERA_NEXT.secondary,
               GameSettings.MAP_VIEW_TOGGLE.primary,
               GameSettings.MAP_VIEW_TOGGLE.secondary,
           };

        void OnGUI()
        {
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel.isEVA)
            {
                Vessel pVessel = FlightGlobals.ActiveVessel;
				KerbalEVA keva = FlightGlobals.ActiveVessel.evaController;
                int fuelPercent = (int)(keva.Fuel / keva.FuelCapacity);



            }
        }

        public ThroughTheEyes()
        {
         	reviewDataKey = ConfigUtil.checkKeys();
			EVAKey = ConfigUtil.EVAKey(GameSettings.CAMERA_MODE.primary);
         	forceIVA = ConfigUtil.ForceIVA();
         	disableMapView = ConfigUtil.checkMapView();
        }

        void disableKeys()
        {
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
                        k.primary = KeyCode.None;
                        k.secondary = KeyCode.None;
                    }

                }
            }
            keysDisabled = true;

        }



        void revertKeys()
        {
			for (int i=0; i < keyBindings.Count; i++)
			{
				KeyBinding k = keyBindings[i];
                k.primary = keySaver[i];
                i += 1;
                k.secondary = keySaver[i];
                i += 1;
            }
            keysDisabled = false;
        }





        void CheckAndSetCamera(Vessel pVessel)
        {
            CameraManager camManage = CameraManager.Instance;

            if (pVessel.situation == Vessel.Situations.PRELAUNCH) { return; }
            if (!pVessel.isEVA)
			{
                if (pVessel.GetCrewCount() > 0)
                {

                    if (disableMapView)
                    {
                        if (camManage.currentCameraMode == flight || camManage.currentCameraMode == map)
                        {
                            if (MapView.MapIsEnabled) { MapView.ExitMapView(); }
                            camManage.SetCameraMode(IVA);
                        }
                    }
                    else
                    {
                        if (camManage.currentCameraMode == flight)
                        { camManage.SetCameraMode(IVA); }
                    }

                }
                else if (pVessel.GetCrewCount() < 1)
                {


                }
            }
        }


        void Start()
        {

            GameEvents.onLaunch.Add((v) =>
            {
                if (forceIVA) { CameraManager.Instance.SetCameraIVA(); disableKeys(); }
            });

        }

        void Update()
        {
            Vessel pVessel = FlightGlobals.ActiveVessel;
            CameraManager flightCam = CameraManager.Instance;
            if (HighLogic.LoadedSceneIsFlight && pVessel != null && pVessel.isActiveVessel && pVessel.state != Vessel.State.DEAD)
            {
                if (forceIVA)
                {
                    if (HighLogic.CurrentGame.Parameters.Flight.CanIVA)
                    {
                        CheckAndSetCamera(pVessel);
                    }
					if (pVessel.situation != Vessel.Situations.PRELAUNCH) {
						if (Input.GetKeyDown(EVAKey)) {
							KeyControls.GoEVA();
							KeyControls.rescueAfterHatchCheck();
						}
					} else if (keysDisabled) {
						revertKeys();
					}
                    if (Input.GetKeyDown(reviewDataKey))
                    {
                        KeyControls.MyReviewData();
                    }
                    if ((Input.GetKeyDown(keySaver[2])) || Input.GetKeyDown(keySaver[3]))
                    {
                        KeyControls.CameraSwitch(flightCam, pVessel);
                    }
                }
            }
        }

        void Destroy()
        {
            if (keysDisabled) { revertKeys(); }
        }

    }
}
