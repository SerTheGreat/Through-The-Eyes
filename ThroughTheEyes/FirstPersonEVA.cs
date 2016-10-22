using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FirstPerson
{

    
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class FirstPersonEVA : MonoBehaviour
    {
        bool forceEVA;
		bool showSightAngle;
        KeyCode toggleFirstPersonKey;

		private Vessel kerbal;
		public bool isFirstPerson;
		private FPGUI fpgui;
		private CameraState cameraState;

        public FirstPersonEVA()
        {
            forceEVA = ConfigUtil.ForceEVA();
			showSightAngle = ConfigUtil.ShowSightAngle();
			toggleFirstPersonKey = ConfigUtil.ToggleFirstPersonKey(GameSettings.CAMERA_MODE.primary);
        }


        void CheckAndSetFirstPerson(Vessel pVessel)
        {
            FlightCamera flightCam = FlightCamera.fetch;

            if (pVessel.isEVA)
            {
                if (!isFirstPerson)
                {
					KerbalEVA evaInst = pVessel.evaController;
                    
					Component[] components = pVessel.transform.GetComponentsInChildren(typeof(Transform));
					for (int i = 0; i < components.Length; i++)
                    {
						Component component = components[i];
						if (component.name.Contains("helmetCollider")) { flightCam.transform.parent.parent = component.transform; }
                    }
					
					enableRenderers(pVessel.transform, false);

                    kerbal = pVessel;
                    flightCam.maxPitch = .98f;
                    flightCam.minPitch = -.98f;
                    flightCam.pivotTranslateSharpness = 50;
                    flightCam.minHeight = 0f;
                    flightCam.minHeightAtMaxDist = 0f;
                    flightCam.minHeightAtMinDist = 0f;
                    flightCam.minDistance = 0.01f;
                    flightCam.maxDistance = 0.01f;
                    flightCam.startDistance = 0.01f;
                    flightCam.SetDistanceImmediate(0.01f);
					flightCam.mainCamera.nearClipPlane = 0.01f;
					//evaInst.animation.cullingType = AnimationCullingType.AlwaysAnimate;
                    isFirstPerson = true;
					if (showSightAngle) {
						fpgui = flightCam.gameObject.AddOrGetComponent<FPGUI>();
					}
                }
            }
            else
            {
                if (isFirstPerson)
                {
                    resetCamera();
                }
                else
                {
					cameraState.saveState(flightCam);
                }
            }


        }

        private void enableRenderers(Transform transform, bool enable) {
			Component[] renderers = transform.GetComponentsInChildren(typeof(Renderer));
			for (int i = 0; i < renderers.Length; i++) {
				Renderer renderer = (Renderer)renderers[i];
				if (renderer.name.Contains("headMesh") || 
				    renderer.name.Contains("eyeball") || 
				    renderer.name.Contains("upTeeth") || 
				    renderer.name.Contains("pupil")
				   ) {
					renderer.enabled = enable;
				}
			}
        }

       void EnableRenderingOnPrevious()
        {
            if (kerbal != null && kerbal.isEVA)
            {

                enableRenderers(FlightGlobals.ActiveVessel.transform, true);
            }
        }

		void resetCamera() {

			GameObject.Destroy(fpgui);

			Vessel pVessel = FlightGlobals.ActiveVessel;
			FlightCamera flightCam = FlightCamera.fetch;

			cameraState.recallState(flightCam);

			if (FlightGlobals.ActiveVessel != null)
			{
				flightCam.SetTargetTransform(pVessel.transform);
			}

			isFirstPerson = false;
		}

       void Start()
       {
		   cameraState = new CameraState();
           //resetCamera();
           EnableRenderingOnPrevious();
           GameEvents.onCrewKilled.Add((v) =>
           {
               resetCamera();
           });
           GameEvents.onVesselChange.Add((v) =>
           {
               EnableRenderingOnPrevious();
               resetCamera();
               if (FlightGlobals.ActiveVessel.isEVA)
               {
                   CameraManager.Instance.SetCameraFlight();

               }
           });
       }

       void Update()
        {
            Vessel pVessel = FlightGlobals.ActiveVessel;
            FlightCamera flightCam = FlightCamera.fetch;
            if (HighLogic.LoadedSceneIsFlight && pVessel != null && pVessel.isActiveVessel && pVessel.state != Vessel.State.DEAD)
            {
                if (forceEVA)
                {
                    CheckAndSetFirstPerson(pVessel);
                }
                else
                {
                    if (pVessel.isEVA)
                    {
                        if (Input.GetKeyDown(toggleFirstPersonKey))
                        {
                            if (!isFirstPerson)
                            {
								cameraState.saveState(flightCam);
                                CheckAndSetFirstPerson(pVessel);
                            }
                            else
                            {
                               resetCamera();
                               EnableRenderingOnPrevious();
                            }
                        }
                    }

                }

				if (isFirstPerson && fpgui != null) {
					float yawAngle = VectorUtils.SignedAngleBetween(
						pVessel.evaController.referenceTransform.up, 
						VectorUtils.ProjectVectorOnPlane(pVessel.evaController.referenceTransform.forward, flightCam.transform.forward),
						pVessel.evaController.referenceTransform.forward);

					fpgui.yawAngle = yawAngle;
					fpgui.pitchAngle = flightCam.getPitch();
				}

            }
        }

    }
    
}
