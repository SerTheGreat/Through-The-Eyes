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

		public FPStateWalkRun (FirstPersonEVA pmgr)
		{
			imgr = pmgr;
			imgr.fpCameraManager.OnEnterFirstPerson += new FirstPersonCameraManager.delEvtEVA(evt_OnEnterFirstPerson);
			imgr.OnFixedUpdate += new EventHandler(evt_OnFixedUpdate);
		}

		void evt_OnEnterFirstPerson(KerbalEVA eva)
		{
			//Hook it!
			ReflectedMembers.Initialize ();

			//*********************
			//***Walk/Run States***
			if (!(eva.st_walk_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_walk_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_walk_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_walk_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_run_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_run_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_bound_gr_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_bound_gr_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}
			if (!(eva.st_bound_fl is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_fl, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}

			//*********************
			//***Turn to Heading***
			if (!(eva.st_heading_acquire is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_heading_acquire, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;

				st.Override_OnFixedUpdate = true;
			}

			//*******************
			//***Idle Grounded***
			if (!(eva.st_idle_gr is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_idle_gr, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetJetpackManualControls;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += GroundedJetpackCheck_PreFixedUpdate;

				st.Override_OnFixedUpdate = true;
			}

			//*************
			//***Jumping***
			if (!(eva.st_jump is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_jump, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnUpdate += Replacement_UpdateMovement;
				st.PreOnUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;

				st.Override_OnUpdate = true;
				st.Override_OnFixedUpdate = true;
			}

			//**************
			//***Swimming***
			if (!(eva.st_swim_fwd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_swim_fwd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}

			if (!(eva.st_swim_idle is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_swim_idle, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetJetpackManualControls;
				st.PreOnFixedUpdate += ForceArcadeMode;
				st.PreOnFixedUpdate += Replacement_CorrectGroundedRotation;
				st.PreOnFixedUpdate += Replacement_UpdateMovement;
				st.PreOnFixedUpdate += Replacement_UpdateHeading;
				st.PreOnFixedUpdate += Replacement_UpdateRagdollVelocities;
				st.Override_OnFixedUpdate = true;
			}

			walk_start_fwd = eva.transform.forward;
			target_fwd = walk_start_fwd;
		}

		bool IsThisEVAIVA(KerbalEVA eva)
		{
			return imgr.fpCameraManager.isFirstPerson && FlightGlobals.ActiveVessel != null && imgr.fpCameraManager.currentfpeva == eva;
		}


		//**********************************
		//********Events for OnEnter********
		//**********************************

		void ResetKerbalForwardVector(KerbalEVA eva, KFSMState s)
		{
			walk_start_fwd = eva.transform.forward;
			target_fwd = walk_start_fwd;
			ReflectedMembers.eva_integral.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_prev_error.SetValue (eva, Vector3.zero);
		}

		void ResetJetpackManualControls(KerbalEVA eva, KFSMState s)
		{
			//When jetpacking, the kerbal may have been in manual axis control.
			//We are now grounded, so that needs to be disabled or we will sidestep-walk
			//rather than turning.
			ReflectedMembers.eva_manualAxisControl.SetValue (eva, false);
			ReflectedMembers.eva_cmdRot.SetValue (eva, Vector3.zero);
		}


		//**************************************
		//********Events for FixedUpdate********
		//**************************************

		void ForceArcadeMode(KerbalEVA eva)
		{
			ForceArcadeMode (eva, null);
		}
		void ForceArcadeMode(KerbalEVA eva, KFSMState s)
		{
			//Force arcade mode walking
			eva.CharacterFrameModeToggle = false;
		}

		void ResetControlOrientation(KerbalEVA eva)
		{
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

			ReflectedMembers.eva_m_HandleMovementInput.Invoke (eva, null);
		}

		void ApplyKerbalForwardTarget(KerbalEVA eva)
		{
			ReflectedMembers.eva_tgtUp.SetValue (eva, eva.fUp);
			ReflectedMembers.eva_tgtFwd.SetValue (eva, target_fwd);
		}

		void Replacement_CorrectGroundedRotation(KerbalEVA eva)
		{
			ReflectedMembers.eva_m_correctGroundedRotation.Invoke (eva, null);
		}

		void Replacement_UpdateMovement(KerbalEVA eva)
		{
			ReflectedMembers.eva_m_UpdateMovement.Invoke (eva, null);
		}

		KerbalEVA cachedrbeva = null;
		Rigidbody cachedrb = null;
		void Replacement_UpdateHeading(KerbalEVA eva)
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

		void Replacement_UpdateRagdollVelocities(KerbalEVA eva)
		{
			ReflectedMembers.eva_m_updateRagdollVelocities.Invoke (eva, null);
		}

		//********Idle State FixedUpdate********

		void GroundedJetpackCheck_PreFixedUpdate(KerbalEVA eva)
		{
			//If the jetpack is on, use jetpack controls while grounded.
			if (eva.JetpackDeployed)
				FirstPersonEVA.instance.fpStateFloating.evtHook_PreOnFixedUpdate (eva);

			Replacement_CorrectGroundedRotation (eva);
			Replacement_UpdateMovement (eva);
			Replacement_UpdateHeading (eva);
			ReflectedMembers.eva_m_UpdatePackLinear.Invoke (eva, null);
			Replacement_UpdateRagdollVelocities (eva);

			//If the jetpack is on, use jetpack controls while grounded.
			if (eva.JetpackDeployed)
				FirstPersonEVA.instance.fpStateFloating.evtHook_PostOnFixedUpdate (eva);
		}


		//***********************************
		//********Global Update Logic********
		//***********************************

		void evt_OnFixedUpdate(object sender, EventArgs none)
		{
			if (!imgr.fpCameraManager.isFirstPerson)
				return;

			//Note that turnrate is in radians/sec, thus * 57.29578f (180/pi)
			if (GameSettings.EVA_left.GetKey (false)) {
				//Quaternion turn = Quaternion.AngleAxis (-60f * Time.fixedDeltaTime, imgr.fpCameraManager.currentfpeva.transform.up);
				Quaternion turn = Quaternion.AngleAxis (-(imgr.fpCameraManager.currentfpeva.turnRate * 0.99f) * Time.fixedDeltaTime * 57.29578f, imgr.fpCameraManager.currentfpeva.transform.up);
				target_fwd = turn * target_fwd;
			}
			else if (GameSettings.EVA_right.GetKey (false)) {
				Quaternion turn = Quaternion.AngleAxis ((imgr.fpCameraManager.currentfpeva.turnRate * 0.99f) * Time.fixedDeltaTime * 57.29578f, imgr.fpCameraManager.currentfpeva.transform.up);
				target_fwd = turn * target_fwd;
			}

		}




	}
}

