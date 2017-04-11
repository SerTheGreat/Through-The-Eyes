using System;
using System.Collections.Generic;

namespace FirstPerson
{
	public class OverrideKFSMIdleState : KFSMState
	{
		KFSMState oldstate = null;

		KerbalEVA iparenteva = null;
		public KerbalEVA ParentEVA { get { return iparenteva; } }

		public OverrideKFSMIdleState ()
			:base("Idle (Floating)")
		{
			this.OnEnter = new KFSMStateChange (H_OnEnter);
			this.OnFixedUpdate = new KFSMCallback (H_OnFixedUpdate);
		}

		static bool hasrefs = false;
		//Kerbal FSM
		internal static System.Reflection.FieldInfo mi_fsm_states = null;
		internal static System.Reflection.FieldInfo mi_fsm_currentstate = null;
		internal static System.Reflection.FieldInfo mi_fsm_laststate = null;
		//Kerbal EVA
		internal static System.Reflection.FieldInfo mi_eva_onjumpcomplete = null;

		internal static void GetRefs()
		{
			if (hasrefs)
				return;

			System.Reflection.MemberInfo[] mi = typeof(KerbalFSM).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField| System.Reflection.BindingFlags.GetField| System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, null);
			foreach (System.Reflection.MemberInfo m in mi) {
				if (m.Name == "States")
					mi_fsm_states = (System.Reflection.FieldInfo)m;
				else if (m.Name == "currentState")
					mi_fsm_currentstate = (System.Reflection.FieldInfo)m;
				else if (m.Name == "lastState")
					mi_fsm_laststate = (System.Reflection.FieldInfo)m;
			}

			mi = typeof(KerbalEVA).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField| System.Reflection.BindingFlags.GetField| System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, null);
			foreach (System.Reflection.MemberInfo m in mi) {
				if (m.Name == "On_jump_complete")
					mi_eva_onjumpcomplete = (System.Reflection.FieldInfo)m;
			}
		}

		public void Hook(KerbalEVA eva)
		{
			GetRefs ();

			iparenteva = eva;
			oldstate = eva.st_idle_fl;
			eva.st_idle_fl = this;

			//Fix up the FSM

			List<KFSMState> States;
			KFSMState currentState;
			KFSMState lastState;

			States = (List<KFSMState>)mi_fsm_states.GetValue (eva.fsm);
			currentState = (KFSMState)mi_fsm_currentstate.GetValue (eva.fsm);
			lastState = (KFSMState)mi_fsm_laststate.GetValue (eva.fsm);

			for (int i = 0; i < States.Count; ++i) {
				if (States [i] == oldstate) {
					States [i] = this;
					break;
				}
			}
			if (currentState == oldstate)
				mi_fsm_currentstate.SetValue (eva.fsm, this);
			if (lastState == oldstate)
				mi_fsm_laststate.SetValue (eva.fsm, this);

			//******State reference fixups******
			eva.On_bound_fall.GoToStateOnEvent = this;

			//Wtf private??
			((KFSMTimedEvent)mi_eva_onjumpcomplete.GetValue(eva)).GoToStateOnEvent = this;

			//eva.fsm.AddEvent (eva.On_land_start, this);
			this.AddEvent(eva.On_land_start);

			eva.On_fall.GoToStateOnEvent = this;

			//Recover complete event

			//eva.fsm.AddEvent(eva.On_packToggle, eva.st_idle_gr, this);
			this.AddEvent(eva.On_packToggle);

			//eva.fsm.AddEvent(eva.On_feet_wet, this, eva.st_idle_gr, eva.st_run_acd, eva.st_walk_acd);
			this.AddEvent(eva.On_feet_wet);

			//Feet dry event

			//eva.fsm.AddEvent(eva.On_ladderGrabStart, this, eva.st_idle_gr, eva.st_swim_idle);
			this.AddEvent(eva.On_ladderGrabStart);

			eva.On_ladderLetGo.GoToStateOnEvent = this;

			eva.On_LadderEnd.GoToStateOnEvent = this;

			eva.On_LadderPushOff.GoToStateOnEvent = this;

			//eva.fsm.AddEvent (eva.On_boardPart, this, eva.st_idle_gr, eva.st_ladder_idle, eva.st_swim_idle);
			this.AddEvent(eva.On_boardPart);

			//eva.fsm.AddEvent(eva.On_seatBoard, this, eva.st_idle_gr, eva.st_swim_idle, eva.st_ladder_idle);
			this.AddEvent(eva.On_seatBoard);

			eva.On_seatDeboard.GoToStateOnEvent = this;
		}

		void H_OnEnter(KFSMState st)
		{
			//KSPLog.print ("H_OnEnter start");
			oldstate.OnEnter (st);
			//KSPLog.print ("H_OnEnter stop");
		}

		void H_OnFixedUpdate()
		{
			KSPLog.print ("H_OnFixedUpdate start");
			if (FirstPersonEVA.instance != null)
				FirstPersonEVA.instance.PreKerbalStateFixedUpdate (ParentEVA);
			oldstate.OnFixedUpdate ();
			//KSPLog.print ("H_OnFixedUpdate stop");
		}



	}
}

