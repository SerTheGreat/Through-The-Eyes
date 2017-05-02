using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	public class FPNavBall
	{
		FirstPersonEVA imgr;
		KSP.UI.Screens.Flight.NavBall navball_;

		public FPNavBall (FirstPersonEVA pmgr)
		{
			imgr = pmgr;
			imgr.OnLateUpdate += new EventHandler(evt_OnLateUpdate);
			imgr.fpCameraManager.OnEnterFirstPerson+= Imgr_fpCameraManager_OnEnterFirstPerson;
		}

		void Imgr_fpCameraManager_OnEnterFirstPerson (KerbalEVA eva)
		{
			if (navball_ == null) {
				navball_ = (KSP.UI.Screens.Flight.NavBall)MonoBehaviour.FindObjectOfType (typeof(KSP.UI.Screens.Flight.NavBall));
			}
				
			//When deboarding a seat, the reference transform is set to the kerbal's part.
			//If you have not deboarded a seat, it is empty.
			//This is to make it always consistent.
			eva.vessel.SetReferenceTransform(eva.part);
		}

		void evt_OnLateUpdate(object sender, EventArgs none)
		{
			if (!imgr.fpCameraManager.isFirstPerson)
				return;

			if (navball_ == null) {
				navball_ = (KSP.UI.Screens.Flight.NavBall)MonoBehaviour.FindObjectOfType (typeof(KSP.UI.Screens.Flight.NavBall));
			}

			Vessel activeVessel = FlightGlobals.ActiveVessel;
			if (activeVessel == null)
				return;

			CelestialBody currentMainBody = FlightGlobals.currentMainBody;

			//NOTE: Kerbal parts seem to be facing towards the sky.
			Quaternion offsetGymbal;
			Part referencepart = activeVessel.GetReferenceTransformPart ();
			if ((referencepart == null)
				|| (referencepart == imgr.fpCameraManager.currentfpeva.part))
				offsetGymbal = Quaternion.AngleAxis(90f, Vector3.right);
			else
				offsetGymbal = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

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



	}
}

