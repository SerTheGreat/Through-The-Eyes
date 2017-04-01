using System;
using UnityEngine;

namespace FirstPerson
{
	/// <summary>
	/// Description of FirstPersonCameraManager.
	/// </summary>
	public class FirstPersonCameraManager
	{
		public bool isFirstPerson;
		
		private bool showSightAngle;
		private CameraState cameraState;
		private float yaw = 0f;
		private float pitch = 0f;
		private const float MAX_LATITUDE = 45.0F; // Don't allow further motion than these (degrees)
		private const float MAX_AZIMUTH = 60.0F;
		private FPGUI fpgui;
		
		private Vector3 eyeOffset = Vector3.zero;//Vector3.forward * 0.1F; //Eyes don't exist at a point when you move your head
		private Vector3 headLocation = Vector3.up * .35f; // Where the centre of the head is
		
		private FirstPersonCameraManager(){	}
		
		public static FirstPersonCameraManager initialize(bool showSightAngle = true) {
			FirstPersonCameraManager instance = new FirstPersonCameraManager();
			instance.cameraState = new CameraState();
			//instance.resetCamera();
			//instance.EnableRenderingOnPrevious(null);
			instance.showSightAngle = showSightAngle;
			return instance;
		}
		
		public void CheckAndSetFirstPerson(Vessel pVessel)
		{
			FlightCamera flightCam = FlightCamera.fetch;

			if (pVessel.isEVA)
			{
				if (!isFirstPerson
				    && !pVessel.packed //this prevents altering camera until EVA is unpacked or else various weird effects are possible
				   )
				{
					/*Component[] components = pVessel.transform.GetComponentsInChildren(typeof(Transform));
					for (int i = 0; i < components.Length; i++)
                    {
						Component component = components[i];
						if (component.name.Contains("helmetCollider")) { flightCam.transform.parent.parent = component.transform; }
                    }*/

					flightCam.transform.parent = FlightGlobals.ActiveVessel.transform;

					enableRenderers(pVessel.transform, false);

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
					viewToNeutral();
					reorient();
				}
			}
			else
			{
				if (isFirstPerson)
				{
					resetCamera(null);
				}
				else
				{
					cameraState.saveState(flightCam);
				}
			}


		}
		
		public void saveCameraState(FlightCamera flightCam) {
			cameraState.saveState(flightCam);
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

		private void EnableRenderingOnPrevious(Vessel vessel)
		{
			if (vessel != null && vessel.isEVA)
			{
				enableRenderers(FlightGlobals.ActiveVessel.transform, true);
			}
		}

		public void resetCamera(Vessel previousVessel) {

			GameObject.Destroy(fpgui);

			if (!isFirstPerson) {
				return;
			}

			Vessel pVessel = FlightGlobals.ActiveVessel;
			FlightCamera flightCam = FlightCamera.fetch;

			cameraState.recallState(flightCam);

			if (FlightGlobals.ActiveVessel != null)
			{
				flightCam.SetTargetTransform(pVessel.transform);
			}
			flightCam.ActivateUpdate();

			isFirstPerson = false;
			
			EnableRenderingOnPrevious(previousVessel);
		}
		
		public bool isCameraProperlyPositioned(FlightCamera flightCam) {
			//Not a particular ellegant way to determine if camera isn't crapped by some background stock logic or change view attempts:
			return Vector3.Distance(flightCam.transform.localPosition, getFPCameraPosition(getFPCameraRotation())) < 0.001f;
		}
		
		public void updateGUI() {
			if (isFirstPerson && fpgui != null) {
				fpgui.yawAngle = -yaw;
				fpgui.pitchAngle = -pitch;
			}
		}
		
		public void viewToNeutral() {
			yaw = 0f;
			pitch = 0f;
		}
		
		public void addYaw(float amount) {
			yaw = Mathf.Clamp(yaw + amount, -MAX_AZIMUTH, MAX_AZIMUTH);
			/*if (Mathf.Abs(yaw) > MAX_AZIMUTH) {
				this.yaw = MAX_AZIMUTH * Mathf.Sign(this.yaw);
			} */
		}
		
		public void addPitch(float amount) {
			pitch = Mathf.Clamp(pitch + amount, -MAX_LATITUDE, MAX_LATITUDE);
			/*if (Mathf.Abs(pitch) > MAX_LATITUDE) {
				this.pitch = MAX_LATITUDE * Mathf.Sign(this.pitch);
			}*/
		}
		
		private Quaternion getFPCameraRotation() {
			return Quaternion.Euler(0.0F, yaw, 0.0F) * Quaternion.Euler(-pitch, 0.0F, 0.0F);
		}

		private Vector3 getFPCameraPosition(Quaternion rotation) {
			return headLocation + rotation * eyeOffset;
		}

		public void reorient() {
			Quaternion rotation = getFPCameraRotation();
			Vector3 cameraForward = rotation * Vector3.forward;
			Vector3 cameraUp = rotation * Vector3.up;
			Vector3 cameraPosition = getFPCameraPosition(rotation);
			FlightCamera flightCam = FlightCamera.fetch;
			flightCam.transform.localRotation = Quaternion.LookRotation(cameraForward, cameraUp);
			flightCam.transform.localPosition = cameraPosition;
		}
		
	}
}
