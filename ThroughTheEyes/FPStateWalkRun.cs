using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	public class FPStateWalkRun
	{
		FirstPersonEVA imgr;
		Vector3 walk_start_fwd;
		Vector3 target_fwd;
		//bool first_frame = false;

		public FPStateWalkRun (FirstPersonEVA pmgr)
		{
			imgr = pmgr;
			imgr.fpCameraManager.OnEnterFirstPerson += new FirstPersonCameraManager.delEvtEVA(evt_OnEnterFirstPerson);
			imgr.fpCameraManager.OnExitFirstPerson += new FirstPersonCameraManager.delEvtEVA(evt_OnExitFirstPerson);
			imgr.OnFixedUpdate += new EventHandler(evt_OnFixedUpdate);
			imgr.OnLateUpdate += new EventHandler(evt_OnLateUpdate);
		}

		void evt_OnEnterFirstPerson(KerbalEVA eva)
		{
			//Hook it!

			//*********************
			//***Walk/Run States***
			if (!(eva.st_walk_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_walk_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				//st.PreOnFixedUpdate += ResetControlOrientation;
				//st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += ArcadeWalkDebug;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_walk_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_walk_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_run_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_run_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_bound_gr_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_bound_gr_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}
			if (!(eva.st_bound_fl is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_fl, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;

				//st.PostOnFixedUpdate += DEBUG_DeltaHdg;
			}

			//*********************
			//***Turn to Heading***
			if (!(eva.st_heading_acquire is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_heading_acquire, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ForceArcadeMode;
				//st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += TurnToHeadingDebug;
			}

			//*******************
			//***Idle Grounded***
			if (!(eva.st_idle_gr is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_idle_gr, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetJetpackManualControls;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += GroundedJetpackCheck_PreFixedUpdate;
				st.PostOnFixedUpdate += GroundedJetpackCheck_PostFixedUpdate;
			}

			walk_start_fwd = eva.transform.forward;
			target_fwd = walk_start_fwd;
		}

		void evt_OnExitFirstPerson(KerbalEVA eva)
		{

		}

		bool IsThisEVAIVA(KerbalEVA eva)
		{
			return imgr.fpCameraManager.isFirstPerson && FlightGlobals.ActiveVessel != null && imgr.fpCameraManager.currentfpeva == eva;
		}

		void DEBUG_DeltaHdg(KerbalEVA eva, string nn)
		{
			float dotproduct = Vector3.Dot (eva.transform.forward, (Vector3)ReflectedMembers.eva_tgtFwd.GetValue (eva));
			float dheading = Mathf.Acos (Vector3.Dot (eva.transform.forward, (Vector3)ReflectedMembers.eva_tgtFwd.GetValue (eva))) * 57.29578f;
			float num = Mathf.Sign ((Quaternion.Inverse (eva.transform.rotation) * (Vector3)ReflectedMembers.eva_tgtFwd.GetValue (eva)).x);
			KSPLog.print (nn + "LastDH: " + ReflectedMembers.eva_lastDeltaHdg.GetValue (eva).ToString ()
				+ ", DH: " + ReflectedMembers.eva_deltaHdg.GetValue (eva).ToString ()
				+ ", TgtF: " + ReflectedMembers.eva_tgtFwd.GetValue(eva).ToString()
				+ ", TrnsF: " + eva.transform.forward.ToString()
				+ ", dot: " + dotproduct.ToString()
				+ ", dh: " + dheading.ToString()
				+ ", num: " + num.ToString()
				+ ", rpos: " + ReflectedMembers.eva_tgtRpos.GetValue(eva).ToString()
				+ ", myTgtF: " + target_fwd.ToString()
			);
		}

		void ResetKerbalForwardVector(KerbalEVA eva, KFSMState s)
		{
			KSPLog.print ("ResetKerbalForwardVector");
			//walk_start_fwd = Quaternion.AngleAxis(0.000001f, eva.transform.up) * eva.transform.forward;
			walk_start_fwd = eva.vessel.transform.forward;
			target_fwd = walk_start_fwd;
			//ReflectedMembers.eva_deltaHdg.SetValue (eva, 0f);
			//ReflectedMembers.eva_lastDeltaHdg.SetValue (eva, 0f);
			ReflectedMembers.eva_integral.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_prev_error.SetValue (eva, Vector3.zero);
			//first_frame = true;
		}

		void TurnToHeadingDebug(KerbalEVA eva)
		{
			ReflectedMembers.Initialize ();

			//DEBUG_DeltaHdg (eva, "HA ");
			ResetControlOrientation (eva);
			//DEBUG_DeltaHdg (eva, "HB ");
			ReflectedMembers.eva_m_correctGroundedRotation.Invoke (eva, null);
			//DEBUG_DeltaHdg (eva, "HD ");
			//ReflectedMembers.eva_m_UpdateHeading.Invoke (eva, null);
			ReplacementUpdateHeading(eva);
			//DEBUG_DeltaHdg (eva, "HE ");
			ReflectedMembers.eva_m_updateRagdollVelocities.Invoke (eva, null);
			//DEBUG_DeltaHdg (eva, "HF ");

			HookedKerbalFSMState.makecall = false;
		}

		void ArcadeWalkDebug(KerbalEVA eva)
		{
			ReflectedMembers.Initialize ();

			//DEBUG_DeltaHdg (eva, "A ");
			ResetControlOrientation (eva);
			ApplyKerbalForwardTarget (eva);
			//DEBUG_DeltaHdg (eva, "B ");
			ReflectedMembers.eva_m_correctGroundedRotation.Invoke (eva, null);
			//DEBUG_DeltaHdg (eva, "C ");
			ReflectedMembers.eva_m_UpdateMovement.Invoke (eva, null);
			//DEBUG_DeltaHdg (eva, "D ");
			//ReflectedMembers.eva_m_UpdateHeading.Invoke (eva, null);
			ReplacementUpdateHeading(eva);
			//DEBUG_DeltaHdg (eva, "E ");
			ReflectedMembers.eva_m_updateRagdollVelocities.Invoke (eva, null);
			//DEBUG_DeltaHdg (eva, "F ");

			HookedKerbalFSMState.makecall = false;
		}

		KerbalEVA cachedrbeva = null;
		Rigidbody cachedrb = null;
		void ReplacementUpdateHeading(KerbalEVA eva)
		{
			if (eva.vessel.packed)
				return;
			float newDH = (float)ReflectedMembers.eva_deltaHdg.GetValue (eva);
			if ((Vector3)ReflectedMembers.eva_tgtRpos.GetValue (eva) != Vector3.zero) {
				newDH = Vector3.Dot (eva.transform.forward, (Vector3)ReflectedMembers.eva_tgtFwd.GetValue (eva));
				if (newDH >= 1f)
					newDH = 1f;
				newDH = Mathf.Acos (newDH) * 57.29578f; //180/pi
			}
			float sign = Mathf.Sign ((Quaternion.Inverse (eva.transform.rotation) * (Vector3)ReflectedMembers.eva_tgtFwd.GetValue (eva)).x);
			newDH *= sign;
			if (eva != cachedrbeva) {
				cachedrb = eva.GetComponent<Rigidbody> ();
				cachedrbeva = eva;
			}
			if (Mathf.Abs (newDH) < (eva.turnRate * 2.0f))
				cachedrb.angularVelocity = newDH * 0.5f * eva.fUp;
			else
				cachedrb.angularVelocity = eva.turnRate * sign * eva.fUp;
			ReflectedMembers.eva_deltaHdg.SetValue (eva, newDH);
		}

		void ApplyKerbalForwardTarget(KerbalEVA eva)
		{
			ReflectedMembers.eva_tgtUp.SetValue (eva, eva.fUp);
			ReflectedMembers.eva_tgtFwd.SetValue (eva, target_fwd);
		}

		void ResetJetpackManualControls(KerbalEVA eva, KFSMState s)
		{
			//When jetpacking, the kerbal may have been in manual axis control.
			//We are now grounded, so that needs to be disabled or we will sidestep-walk
			//rather than turning.
			ReflectedMembers.eva_manualAxisControl.SetValue (eva, false);
			ReflectedMembers.eva_cmdRot.SetValue (eva, Vector3.zero);
		}

		/*
		void ResetDeltaHdg(KerbalEVA eva)
		{
			if (!imgr.fpCameraManager.isFirstPerson || FlightGlobals.ActiveVessel == null || imgr.fpCameraManager.currentfpeva != eva)
				return;
			
			//ReflectedMembers.eva_deltaHdg.SetValue (eva, 0f);
			//ReflectedMembers.eva_lastDeltaHdg.SetValue (eva, 0f);
		}
		*/

		void ForceArcadeMode(KerbalEVA eva)
		{
			ForceArcadeMode (eva, null);
		}
		void ForceArcadeMode(KerbalEVA eva, KFSMState s)
		{
			//Force arcade mode walking
			eva.CharacterFrameModeToggle = false;
		}

		void GroundedJetpackCheck_PreFixedUpdate(KerbalEVA eva)
		{
			//If the jetpack is on, use jetpack controls while grounded.
			if (eva.JetpackDeployed)
				FirstPersonEVA.instance.fpStateFloating.evtHook_PreOnFixedUpdate (eva);
			else {
				ReflectedMembers.Initialize ();

				//DEBUG_DeltaHdg (eva, "IA ");
				ReflectedMembers.eva_m_correctGroundedRotation.Invoke (eva, null);
				//DEBUG_DeltaHdg (eva, "IB ");
				ReflectedMembers.eva_m_UpdateMovement.Invoke (eva, null);
				//DEBUG_DeltaHdg (eva, "IC ");
				//ReflectedMembers.eva_m_UpdateHeading.Invoke (eva, null);
				ReplacementUpdateHeading(eva);
				//DEBUG_DeltaHdg (eva, "ID ");
				ReflectedMembers.eva_m_UpdatePackLinear.Invoke (eva, null);
				//DEBUG_DeltaHdg (eva, "IE ");
				ReflectedMembers.eva_m_updateRagdollVelocities.Invoke (eva, null);
				//DEBUG_DeltaHdg (eva, "IF ");

				HookedKerbalFSMState.makecall = false;
			}
		}

		void GroundedJetpackCheck_PostFixedUpdate(KerbalEVA eva)
		{
			//If the jetpack is on, use jetpack controls while grounded.
			if (eva.JetpackDeployed)
				FirstPersonEVA.instance.fpStateFloating.evtHook_PostOnFixedUpdate (eva);
		}

		void ResetControlOrientation(KerbalEVA eva)
		{
			ReflectedMembers.Initialize ();

			//Finished turn to heading.
			//Reset stuff.
			//imgr.fpCameraManager.reorient();

			//Re-run the movement input calculations from fixedupdate
			eva.fUp = FlightCamera.fetch.getReferenceFrame() * Vector3.up;
			eva.fFwd = Vector3.ProjectOnPlane(walk_start_fwd, eva.fUp).normalized;
			eva.fRgt = Vector3.Cross (eva.fUp, eva.fFwd);

			ReflectedMembers.eva_deltaHdg.SetValue (eva, 0f);
			ReflectedMembers.eva_cmdRot.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_packLinear.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_fuelFlowRate.SetValue (eva, 0f);
			ReflectedMembers.eva_tgtRpos.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_ladderTgtRPos.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_packTgtRPos.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_packRRot.SetValue (eva, Vector3.zero);

			ForceArcadeMode (eva);

			ReflectedMembers.eva_m_HandleMovementInput.Invoke (eva, null);
		}

		void evt_OnFixedUpdate(object sender, EventArgs none)
		{
			if (!imgr.fpCameraManager.isFirstPerson)
				return;

			if (GameSettings.EVA_left.GetKey (false)) {
				Quaternion turn = Quaternion.AngleAxis (-60f * Time.fixedDeltaTime, imgr.fpCameraManager.currentfpeva.transform.up);
				target_fwd = turn * target_fwd;
			}
			else if (GameSettings.EVA_right.GetKey (false)) {
				Quaternion turn = Quaternion.AngleAxis (60f * Time.fixedDeltaTime, imgr.fpCameraManager.currentfpeva.transform.up);
				target_fwd = turn * target_fwd;
			}

		}

		void evt_OnLateUpdate(object sender, EventArgs none)
		{
			if (!imgr.fpCameraManager.isFirstPerson)
				return;

		}




	}
}

