using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	//TODO place the camera at correct position of a ragdolled kerbal (depend on helmet transform position?)
    
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FirstPersonEVA : MonoBehaviour
	{
		public static FirstPersonEVA instance = null;

		public FirstPersonCameraManager fpCameraManager;
		KSP.UI.Screens.Flight.NavBall navball_;
    	
		bool forceEVA;
		KeyCode toggleFirstPersonKey;

		private const float mouseViewSensitivity = 3000f; //TODO take into account in-game mouse view sensitivity
		public EVAIVAState state = new EVAIVAState();

		private bool needCamReset = false;
		private bool stopTouchingCamera = false;

		public FirstPersonEVA() { } 
		
		private void onVesselDestroy(Vessel v) {
			navball_ = null;
			if (v.isActiveVessel && v.isEVA) {
				fpCameraManager.resetCamera(v);
			}
		}
		
		private void onVesselSwitching(Vessel from, Vessel to) {
			navball_ = null;
			fpCameraManager.resetCamera((Vessel)from);
			if (((Vessel)to).isEVA) {
				CameraManager.Instance.SetCameraFlight();
			}
		}
		
		private void onMapExited() {
			//When exitting map view an attempt to set 1st person camera in the same update cycle is overridden with some stock camera handling
			//so we have to set flag to reset 1st person camera a bit later
			navball_ = null;
			needCamReset = true; 
		}

		private void onSceneLoadRequested(GameScenes scene) {
			//This is needed to avoid fighting stock camera during "Revert to launch" as that causes NullRefences in Unity breaking the revert process
			navball_ = null;
			stopTouchingCamera = true;
		}
		
		void Start()
		{
			instance = this;

			forceEVA = ConfigUtil.ForceEVA();
			toggleFirstPersonKey = ConfigUtil.ToggleFirstPersonKey(GameSettings.CAMERA_MODE.primary);

			stopTouchingCamera = false;
			
			fpCameraManager = FirstPersonCameraManager.initialize(ConfigUtil.ShowSightAngle());
 			
			GameEvents.onVesselDestroy.Add(onVesselDestroy);
			/*GameEvents.onCrewKilled.Add((v) => {
           		fpCameraManager.resetCamera(null);
			});*/

			GameEvents.onVesselSwitching.Add(onVesselSwitching);
			GameEvents.OnMapExited.Add(onMapExited);
			GameEvents.onGameSceneLoadRequested.Add(onSceneLoadRequested);
		}

		void Update()
		{
			Vessel pVessel = FlightGlobals.ActiveVessel;
			FlightCamera flightCam = FlightCamera.fetch;
			if (FlightGlobals.ActiveVessel.isEVA && fpCameraManager.isFirstPerson && needCamReset) {
				fpCameraManager.isFirstPerson = false;
				fpCameraManager.CheckAndSetFirstPerson(pVessel);
			}
			needCamReset = false;

			if (HighLogic.LoadedSceneIsFlight && pVessel != null && pVessel.isActiveVessel && pVessel.state != Vessel.State.DEAD && !stopTouchingCamera) {
				if (forceEVA || fpCameraManager.isFirstPerson) {
					if (!fpCameraManager.isCameraProperlyPositioned(flightCam)) {
						fpCameraManager.isFirstPerson = false;
					}
					fpCameraManager.CheckAndSetFirstPerson(pVessel);
				} 
				if (!forceEVA && pVessel.isEVA) {
					if (Input.GetKeyDown(toggleFirstPersonKey)) {
						if (!fpCameraManager.isFirstPerson) {
							fpCameraManager.saveCameraState(flightCam);
							fpCameraManager.CheckAndSetFirstPerson(pVessel);
						} else {
							fpCameraManager.resetCamera(pVessel);
						}
					}
				}

				fpCameraManager.updateGUI();

			}
		}

		void FixedUpdate()
		{
			if (fpCameraManager.isFirstPerson) {
				if (Input.GetMouseButton(1)) { // Right Mouse Button Down
					//Change the angles by the mouse movement
					fpCameraManager.addYaw(Input.GetAxis("Mouse X") / Screen.width * mouseViewSensitivity);
					fpCameraManager.addPitch(Input.GetAxis("Mouse Y") / Screen.height * mouseViewSensitivity);
					fpCameraManager.reorient();
					state.kerballookrotation = FlightCamera.fetch.transform.rotation;
				} //button held down

				if (FlightGlobals.ActiveVessel.Landed && (GameSettings.EVA_back.GetKey() || GameSettings.EVA_forward.GetKey())) {
					fpCameraManager.viewToNeutral();
					FlightCamera.fetch.transform.rotation = state.kerballookrotation;
				}

				if (FlightGlobals.ActiveVessel.Landed && (GameSettings.EVA_back.GetKeyUp() || GameSettings.EVA_forward.GetKeyUp())) {
					//fpCameraManager.viewToNeutral();
					fpCameraManager.reorient();
				}






				/////////////////////////////////////////////////////////
				System.Reflection.FieldInfo mi_states = null;
				//List<KFSMState> States;

				System.Reflection.FieldInfo mi_currentstate = null;
				//KFSMState currentState;

				System.Reflection.FieldInfo mi_laststate = null;
				//KFSMState lastState;

				System.Reflection.MemberInfo[] mi = typeof(KerbalFSM).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField| System.Reflection.BindingFlags.GetField| System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, null);
				foreach (System.Reflection.MemberInfo m in mi) {
					if (m.Name == "States")
						mi_states = (System.Reflection.FieldInfo)m;
					else if (m.Name == "currentState")
						mi_currentstate = (System.Reflection.FieldInfo)m;
					else if (m.Name == "lastState")
						mi_laststate = (System.Reflection.FieldInfo)m;
				}

				KFSMState cur = (          (KFSMState)mi_currentstate.GetValue (FlightGlobals.ActiveVessel.evaController.fsm)        );
				KSPLog.print("State: " + cur.name + ", " +  cur.GetType().ToString());
				/// ///////////////////////////////////////////////////////
			}
		}

		public void PreKerbalStateFixedUpdate(KerbalEVA parent)
		{

			if (fpCameraManager.isFirstPerson && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.evaController == parent) {

				ReflectedMembers.Initialize();

				if ((FlightGlobals.ActiveVessel.situation != Vessel.Situations.SPLASHED
					&& FlightGlobals.ActiveVessel.situation != Vessel.Situations.LANDED)
					&& FlightGlobals.ActiveVessel.evaController.JetpackDeployed) {

					//************Rotation************
					Quaternion manualRotation = Quaternion.identity;
					Vector3 commandedManualRotation = Vector3.zero;
					if (GameSettings.YAW_LEFT.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.up);
						commandedManualRotation -= parent.transform.up;
						KSPLog.print ("YAW LEFT");
					}
					else if (GameSettings.YAW_RIGHT.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.up);
						commandedManualRotation += parent.transform.up;
						KSPLog.print ("YAW RIGHT");
					}

					if (GameSettings.PITCH_UP.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.right);
						commandedManualRotation -= parent.transform.right;
						KSPLog.print ("PITCH UP");
					}
					else if (GameSettings.PITCH_DOWN.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.right);
						commandedManualRotation += parent.transform.right;
						KSPLog.print ("PITCH DOWN");
					}

					if (GameSettings.ROLL_RIGHT.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.forward);
						commandedManualRotation -= parent.transform.forward;
						KSPLog.print ("ROLL RIGHT");
					}
					else if (GameSettings.ROLL_LEFT.GetKey (false)) {
						manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.forward);
						commandedManualRotation += parent.transform.forward;
						KSPLog.print ("ROLL LEFT");
					}

					//Testing
					Vector3 kp = new Vector3 (3.0f, 3.0f, 3.0f);
					Vector3 ki = new Vector3 (0.25f, 0.25f, 0.25f);
					//Vector3 kd = new Vector3 (0.3f, 0.3f, 0.3f);
					Vector3 kd = new Vector3 (0.001f, 0.001f, 0.001f);

					ReflectedMembers.eva_manualAxisControl.SetValue (parent, true);
					if (manualRotation == Quaternion.identity) {
						//No rotation controls active. SAS active, maybe.

						//Set manual mode based on SAS mode.
						if (GameSettings.EVA_ROTATE_ON_MOVE) {
							//Run PID.
							Vector3 angularvelocity = parent.part.Rigidbody.angularVelocity;
							Vector3 currenterror = -ClampVectorComponents(angularvelocity, -0.5f, 0.5f);
							state.rotationpid_integral = ClampVectorComponents (state.rotationpid_integral + currenterror * Time.fixedDeltaTime, -1f, 1f);
							Vector3 derivative = (currenterror - state.rotationpid_previouserror) / Time.fixedDeltaTime;
							Vector3 pidresult = PairwiseMultiplyVectors(kp, currenterror)
								+ PairwiseMultiplyVectors(ki, state.rotationpid_integral)
								+ PairwiseMultiplyVectors(kd, derivative);

							KSPLog.print ("currenterror: " + currenterror.ToString ());
							KSPLog.print ("rotationpid_integral: " + state.rotationpid_integral.ToString ());
							KSPLog.print ("derivative: " + derivative.ToString ());

							state.rotationpid_previouserror = currenterror;

							//Assign command
							ReflectedMembers.eva_cmdRot.SetValue (parent, pidresult);
							KSPLog.print ("SAS on, no command, PID result: " + pidresult.ToString() + ", actual velocity: " + angularvelocity.ToString());
						} else {
							KSPLog.print ("SAS off, no command");
							//Idle and SAS off. Do nothing.
							ReflectedMembers.eva_cmdRot.SetValue (parent, Vector3.zero);
						}
					} else {
						//Rotation controls active.

						//Reset PID
						state.rotationpid_integral = Vector3.zero;
						state.rotationpid_previouserror = Vector3.zero;

						ReflectedMembers.eva_cmdRot.SetValue (parent, commandedManualRotation);

						KSPLog.print ("Manual command");
					}

					//************Translation************
					Vector3 manualTranslation = Vector3.zero;
					if (GameSettings.TRANSLATE_LEFT.GetKey (false)) {
						manualTranslation += Vector3.left;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.up);
						KSPLog.print ("YAW LEFT");
					}
					else if (GameSettings.TRANSLATE_RIGHT.GetKey (false)) {
						manualTranslation += Vector3.right;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.up);
						KSPLog.print ("YAW RIGHT");
					}

					if (GameSettings.TRANSLATE_UP.GetKey (false)) {
						manualTranslation += Vector3.up;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.right);
						KSPLog.print ("PITCH UP");
					}
					else if (GameSettings.TRANSLATE_DOWN.GetKey (false)) {
						manualTranslation += Vector3.down;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.right);
						KSPLog.print ("PITCH DOWN");
					}

					if (GameSettings.TRANSLATE_FWD.GetKey (false)) {
						manualTranslation += Vector3.forward;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) (-(double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.forward);
						KSPLog.print ("ROLL RIGHT");
					}
					else if (GameSettings.TRANSLATE_BACK.GetKey (false)) {
						manualTranslation += Vector3.back;
						//manualRotation = manualRotation * Quaternion.AngleAxis((float) ((double) FlightGlobals.ActiveVessel.evaController.turnRate * 57.2957801818848) * Time.deltaTime, FlightGlobals.ActiveVessel.evaController.transform.forward);
						KSPLog.print ("ROLL LEFT");
					}

					manualTranslation.Normalize ();
					manualTranslation = FlightGlobals.ActiveVessel.transform.rotation * manualTranslation;

					KSPLog.print ("Resetting rpos. Old value: " + ((Vector3)ReflectedMembers.eva_packTgtRPos.GetValue (FlightGlobals.ActiveVessel.evaController)).ToString ()
						+", new value: " + manualTranslation.ToString());
					ReflectedMembers.eva_packTgtRPos.SetValue (FlightGlobals.ActiveVessel.evaController, manualTranslation);

				}



			}
		}

		void LateUpdate()
		{
			if (fpCameraManager.isFirstPerson) {
				if (navball_ == null) {
					navball_ = (KSP.UI.Screens.Flight.NavBall)FindObjectOfType (typeof(KSP.UI.Screens.Flight.NavBall));
				}

				FixupNavBall ();

			}
		}


		private void FixupNavBall()
		{
			CelestialBody currentMainBody = FlightGlobals.currentMainBody;
			Quaternion offsetGymbal = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
			Quaternion attitudeGymbal = offsetGymbal * Quaternion.Inverse(navball_.target.rotation);
			Quaternion relativeGymbal = attitudeGymbal * Quaternion.LookRotation(Vector3.ProjectOnPlane((Vector3) (currentMainBody.position + (Vector3d) currentMainBody.transform.up * currentMainBody.Radius - navball_.target.position), (Vector3) (navball_.target.position - currentMainBody.position).normalized).normalized, (Vector3) (navball_.target.position - currentMainBody.position).normalized);
			navball_.navBall.rotation = relativeGymbal;

			if (navball_.progradeVector.gameObject.activeSelf)
				navball_.progradeVector.gameObject.SetActive(false);
			if (navball_.retrogradeVector.gameObject.activeSelf)
				navball_.retrogradeVector.gameObject.SetActive(false);
			if (navball_.progradeWaypoint.gameObject.activeSelf)
				navball_.progradeWaypoint.gameObject.SetActive(false);
			if (navball_.retrogradeWaypoint.gameObject.activeSelf)
				navball_.retrogradeWaypoint.gameObject.SetActive(false);

			if (navball_.radialInVector.gameObject.activeSelf)
				navball_.radialInVector.gameObject.SetActive(false);
			if (navball_.radialOutVector.gameObject.activeSelf)
				navball_.radialOutVector.gameObject.SetActive(false);
			if (navball_.normalVector.gameObject.activeSelf)
				navball_.normalVector.gameObject.SetActive(false);
			if (navball_.antiNormalVector.gameObject.activeSelf)
				navball_.antiNormalVector.gameObject.SetActive (false);

			Vessel activeVessel = FlightGlobals.ActiveVessel;

			Vector3 displayVelocity = Vector3.zero;
			switch (FlightGlobals.speedDisplayMode)
			{
			case FlightGlobals.SpeedDisplayModes.Orbit:
				displayVelocity = (Vector3) FlightGlobals.ship_obtVelocity;
				break;
			case FlightGlobals.SpeedDisplayModes.Surface:
				displayVelocity = (Vector3) FlightGlobals.ship_srfVelocity;
				break;
			case FlightGlobals.SpeedDisplayModes.Target:
				displayVelocity = (Vector3) FlightGlobals.ship_tgtVelocity;
				break;
			}
			float displaySpeed = displayVelocity.magnitude;
			if (displaySpeed == 0.0f)
				displaySpeed = 1E-06f;
			Vector3 displayVelDir = displayVelocity / displaySpeed;

			navball_.progradeVector.localPosition = attitudeGymbal * displayVelDir * navball_.VectorUnitScale;
			navball_.progradeVector.gameObject.SetActive((double) displaySpeed > (double) navball_.VectorVelocityThreshold && (double) navball_.progradeVector.transform.localPosition.z >= (double) navball_.VectorUnitCutoff);

			navball_.retrogradeVector.localPosition = attitudeGymbal * -displayVelDir * navball_.VectorUnitScale;
			navball_.retrogradeVector.gameObject.SetActive((double) displaySpeed > (double) navball_.VectorVelocityThreshold && (double) navball_.retrogradeVector.transform.localPosition.z > (double) navball_.VectorUnitCutoff);

			if (FlightGlobals.fetch.vesselTargetDirection != Vector3.zero)
				navball_.progradeWaypoint.localPosition = attitudeGymbal * FlightGlobals.fetch.vesselTargetDirection * navball_.VectorUnitScale;
			navball_.progradeWaypoint.gameObject.SetActive(FlightGlobals.fetch.vesselTargetTransform != null && (double) navball_.progradeWaypoint.transform.localPosition.z >= (double) navball_.VectorUnitCutoff);

			if (FlightGlobals.fetch.vesselTargetDirection != Vector3.zero)
				navball_.retrogradeWaypoint.localPosition = attitudeGymbal * -FlightGlobals.fetch.vesselTargetDirection * navball_.VectorUnitScale;
			navball_.retrogradeWaypoint.gameObject.SetActive(FlightGlobals.fetch.vesselTargetTransform != null && (double) navball_.retrogradeWaypoint.transform.localPosition.z > (double) navball_.VectorUnitCutoff);

			SetVectorAlphaTint(navball_.progradeVector);
			SetVectorAlphaTint(navball_.retrogradeVector);
			SetVectorAlphaTint(navball_.progradeWaypoint);
			SetVectorAlphaTint(navball_.retrogradeWaypoint);

			if (activeVessel.orbit != null && activeVessel.orbit.referenceBody != null && FlightGlobals.speedDisplayMode == FlightGlobals.SpeedDisplayModes.Orbit)
			{
				Vector3 wCoM = activeVessel.CurrentCoM;
				Vector3 obtVel = (Vector3) activeVessel.orbit.GetVel();
				Vector3 cbPos = (Vector3) activeVessel.mainBody.position;
				Vector3 radial = Vector3.ProjectOnPlane((wCoM - cbPos).normalized, obtVel).normalized;
				Vector3 normal = Vector3.Cross(radial, obtVel.normalized);
				radial = attitudeGymbal * radial * navball_.VectorUnitScale;
				normal = attitudeGymbal * normal * navball_.VectorUnitScale;

				navball_.antiNormalVector.localPosition = normal;
				navball_.normalVector.localPosition = -normal;
				navball_.antiNormalVector.gameObject.SetActive((double) normal.z > (double) navball_.VectorUnitCutoff);
				navball_.normalVector.gameObject.SetActive((double) normal.z < -(double) navball_.VectorUnitCutoff);

				SetVectorAlphaTint(navball_.antiNormalVector);
				SetVectorAlphaTint(navball_.normalVector);

				navball_.radialInVector.localPosition = -radial;
				navball_.radialOutVector.localPosition = radial;
				navball_.radialInVector.gameObject.SetActive((double) radial.z < -(double) navball_.VectorUnitCutoff);
				navball_.radialOutVector.gameObject.SetActive((double) radial.z > (double) navball_.VectorUnitCutoff);

				SetVectorAlphaTint(navball_.radialInVector);
				SetVectorAlphaTint(navball_.radialOutVector);
			}

			navball_.headingText.text = Quaternion.Inverse(relativeGymbal).eulerAngles.y.ToString("000") + "°";
		}


		private void SetVectorAlphaTint(Transform vector)
		{
			float opacity = Mathf.Clamp01(Vector3.Dot(vector.localPosition.normalized, Vector3.forward));
			float orientation = Vector3.Dot(vector.localPosition.normalized, Vector3.up);
			if ((double) orientation >= 0.649999976158142)
				opacity *= Mathf.Clamp01(Mathf.InverseLerp(0.9f, 0.65f, orientation));
			else if ((double) orientation <= -0.75)
				opacity *= Mathf.Clamp01(Mathf.InverseLerp(-0.95f, -0.75f, orientation));
			vector.GetComponent<MeshRenderer>().materials[0].SetFloat("_Opacity", opacity);
		}

		Vector3 ClampVectorComponents(Vector3 v, float min, float max)
		{
			Vector3 ret = new Vector3 ();
			ret.x = Mathf.Clamp (v.x, min, max);
			ret.y = Mathf.Clamp (v.y, min, max);
			ret.z = Mathf.Clamp (v.z, min, max);
			return ret;
		}

		Vector3 PairwiseMultiplyVectors(Vector3 a, Vector3 b)
		{
			Vector3 ret = new Vector3 ();
			ret.x = a.x * b.x;
			ret.y = a.y * b.y;
			ret.z = a.z * b.z;
			return ret;
		}

	}
    
}
