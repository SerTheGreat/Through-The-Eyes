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
			
		public void Hook(KerbalEVA eva)
		{
			ReflectedMembers.Initialize();

			iparenteva = eva;
			oldstate = eva.st_idle_fl;
			eva.st_idle_fl = this;

			//Fix up the FSM

			List<KFSMState> States;
			KFSMState currentState;
			KFSMState lastState;

			States = (List<KFSMState>)ReflectedMembers.fsm_states.GetValue (eva.fsm);
			currentState = (KFSMState)ReflectedMembers.fsm_currentstate.GetValue (eva.fsm);
			lastState = (KFSMState)ReflectedMembers.fsm_laststate.GetValue (eva.fsm);

			for (int i = 0; i < States.Count; ++i) {
				if (States [i] == oldstate) {
					States [i] = this;
					break;
				}
			}
			if (currentState == oldstate)
				ReflectedMembers.fsm_currentstate.SetValue (eva.fsm, this);
			if (lastState == oldstate)
				ReflectedMembers.fsm_laststate.SetValue (eva.fsm, this);

			//******State reference fixups******
			eva.On_bound_fall.GoToStateOnEvent = this;

			//Wtf private??
			((KFSMTimedEvent)ReflectedMembers.eva_onjumpcomplete.GetValue(eva)).GoToStateOnEvent = this;

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

