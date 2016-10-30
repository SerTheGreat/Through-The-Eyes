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
		private float yaw = 0f;
		private float pitch = 0f;
		private float maxLatitude = 45.0F; // Don't allow further motion that this (degrees)
		private float maxAzimuth = 60.0F;
		private Vector3 eyeOffset = Vector3.zero;//Vector3.forward * 0.1F; //Eyes don't exist at a point when you move your head
		private Vector3 headLocation = Vector3.up * .35f; // Where the centre of the head is
		private Quaternion fixedRotation = new Quaternion();

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
					/*Component[] components = pVessel.transform.GetComponentsInChildren(typeof(Transform));
					for (int i = 0; i < components.Length; i++)
                    {
						Component component = components[i];
						if (component.name.Contains("helmetCollider")) { flightCam.transform.parent.parent = component.transform; }
                    }*/

					flightCam.transform.parent = FlightGlobals.ActiveVessel.transform;

					enableRenderers(pVessel.transform, false);

                    kerbal = pVessel;
                    /*flightCam.maxPitch = .98f;
                    flightCam.minPitch = -.98f;
                    flightCam.pivotTranslateSharpness = 50;
                    flightCam.minHeight = 0f;
                    flightCam.minHeightAtMaxDist = 0f;
                    flightCam.minHeightAtMinDist = 0f;
                    flightCam.minDistance = 0.01f;
                    flightCam.maxDistance = 0.01f;
                    flightCam.startDistance = 0.01f;
                    flightCam.SetDistanceImmediate(0.01f);*/
					//evaInst.animation.cullingType = AnimationCullingType.AlwaysAnimate;
					flightCam.mainCamera.nearClipPlane = 0.01f;
                    isFirstPerson = true;
					if (showSightAngle) {
						fpgui = flightCam.gameObject.AddOrGetComponent<FPGUI>();
					}
					flightCam.SetTargetNone();
					flightCam.DeactivateUpdate();
					yaw = 0f;
					pitch = 0f;
					reorient();
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
					renderer.name.Contains("downTeeth") || 
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
			flightCam.ActivateUpdate();

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
					fpgui.yawAngle = -yaw;
					fpgui.pitchAngle = -pitch;
				}

            }
        }

		void FixedUpdate() {
			if (isFirstPerson) {
				if(Input.GetMouseButton(1)) { // Right Mouse Button Down
					//Change the angles by the mouse movement
					yaw += Input.GetAxis("Mouse X");
					pitch += Input.GetAxis("Mouse Y");
					if (Mathf.Abs(yaw) > maxAzimuth) {
						yaw = maxAzimuth * Mathf.Sign(yaw);
					}
					if (Mathf.Abs(pitch) > maxLatitude) {
						pitch = maxLatitude * Mathf.Sign(pitch);
					}
					reorient();
					fixedRotation = FlightCamera.fetch.transform.rotation;
				} //button held down

				if (FlightGlobals.ActiveVessel.Landed && (GameSettings.EVA_back.GetKey() || GameSettings.EVA_forward.GetKey())) {
					yaw = 0f;
					pitch = 0f;
					FlightCamera.fetch.transform.rotation = fixedRotation;
				}

				if (FlightGlobals.ActiveVessel.Landed && (GameSettings.EVA_back.GetKeyUp() || GameSettings.EVA_forward.GetKeyUp())) {
					yaw = 0f;
					pitch = 0f;
					reorient();
				}

			}
		}

		private void reorient() {
			Quaternion rotation = Quaternion.Euler(0.0F, yaw, 0.0F) * Quaternion.Euler(-pitch, 0.0F, 0.0F);
			Vector3 cameraForward = rotation * Vector3.forward;
			Vector3 cameraUp = rotation * Vector3.up;
			Vector3 cameraPosition = headLocation + rotation * eyeOffset;
			FlightCamera flightCam = FlightCamera.fetch;
			flightCam.transform.localRotation = Quaternion.LookRotation(cameraForward, cameraUp);
			flightCam.transform.localPosition = cameraPosition;
		}

    }
    
}
