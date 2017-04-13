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
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_walk_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_walk_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_run_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_run_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_run_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_bound_gr_acd is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_acd, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_bound_gr_fps is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_gr_fps, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}
			if (!(eva.st_bound_fl is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_bound_fl, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ResetKerbalForwardVector; //FORWARD
				st.PreOnFixedUpdate += ResetControlOrientation;
				st.PreOnFixedUpdate += ApplyKerbalForwardTarget;
			}

			//*********************
			//***Turn to Heading***
			if (!(eva.st_heading_acquire is HookedKerbalFSMState)) {
				HookedKerbalFSMState st = new HookedKerbalFSMState (eva.st_heading_acquire, IsThisEVAIVA);
				st.Hook (eva);
				st.PreOnEnter += ForceArcadeMode;
				st.PreOnFixedUpdate += ResetControlOrientation;
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

		void ResetKerbalForwardVector(KerbalEVA eva, KFSMState s)
		{
			walk_start_fwd = eva.transform.forward;
			target_fwd = walk_start_fwd;
			//ReflectedMembers.eva_deltaHdg.SetValue (eva, 0f);
			//ReflectedMembers.eva_lastDeltaHdg.SetValue (eva, 0f);
			ReflectedMembers.eva_integral.SetValue (eva, Vector3.zero);
			ReflectedMembers.eva_prev_error.SetValue (eva, Vector3.zero);
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

