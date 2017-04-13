using System;
using System.Collections.Generic;

namespace FirstPerson
{
	public class HookedKerbalFSMState : KFSMState
	{
		KFSMState originalstate;
		bool ishooked = false;
		KerbalEVA eva;

		public delegate bool delHookCondition(KerbalEVA eva);
		public event delHookCondition HookCondition;
		public delegate void delHookedKFSMStateChange(KerbalEVA eva, KFSMState st);
		public delegate void delHookedKFSMCallback(KerbalEVA eva);
		public event delHookedKFSMStateChange PreOnEnter;
		public event delHookedKFSMStateChange PostOnEnter;
		public event delHookedKFSMStateChange PreOnLeave;
		public event delHookedKFSMStateChange PostOnLeave;
		public event delHookedKFSMCallback PreOnUpdate;
		public event delHookedKFSMCallback PostOnUpdate;
		public event delHookedKFSMCallback PreOnFixedUpdate;
		public event delHookedKFSMCallback PostOnFixedUpdate;
		public event delHookedKFSMCallback PreOnLateUpdate;
		public event delHookedKFSMCallback PostOnLateUpdate;

		public bool Override_OnUpdate = false;
		public bool Override_OnFixedUpdate = false;
		public bool Override_OnLateUpdate = false;

		public HookedKerbalFSMState (KFSMState poriginalstate, delHookCondition hookoperationcondition)
			:base(poriginalstate.name)
		{
			originalstate = poriginalstate;

			HookCondition = hookoperationcondition;

			updateMode = originalstate.updateMode;
			OnEnter = new KFSMStateChange (H_OnEnter);
			OnLeave = new KFSMStateChange (H_OnLeave);
			OnUpdate = new KFSMCallback (H_OnUpdate);
			OnFixedUpdate = new KFSMCallback (H_OnFixedUpdate);
			OnLateUpdate = new KFSMCallback (H_OnLateUpdate);
		}

		public void Hook(KerbalEVA peva)
		{
			if (ishooked)
				throw new Exception ("HookedKerbalFSMState already hooked.");
			ishooked = true;
			eva = peva;

			ReflectedMembers.Initialize();

			//******************************************
			//Add the original events back to our state.
			for (int i = 0; i < originalstate.StateEvents.Count; ++i) {
				AddEvent (originalstate.StateEvents [i]);
			}

			//*******************************************
			//Replace references to our state in the FSM.
			List<KFSMState> States;
			KFSMState currentState;
			KFSMState lastState;

			States = (List<KFSMState>)ReflectedMembers.fsm_states.GetValue (eva.fsm);
			currentState = (KFSMState)ReflectedMembers.fsm_currentstate.GetValue (eva.fsm);
			lastState = (KFSMState)ReflectedMembers.fsm_laststate.GetValue (eva.fsm);

			for (int i = 0; i < States.Count; ++i) {
				if (States [i] == originalstate) {
					States [i] = this;
					break;
				}
			}
			if (currentState == originalstate)
				ReflectedMembers.fsm_currentstate.SetValue (eva.fsm, this);
			if (lastState == originalstate)
				ReflectedMembers.fsm_laststate.SetValue (eva.fsm, this);

			//*******************************************
			//Replace references to our state in the EVA.
			for (int i = 0; i < ReflectedMembers.eva_type_kfsmstate.Count; ++i) {
				System.Reflection.FieldInfo f = ReflectedMembers.eva_type_kfsmstate [i];
				KFSMState refval = f.GetValue (eva) as KFSMState;
				if (refval == null)
					continue;
				if (refval == originalstate) {
					f.SetValue (eva, this);
				}
			}

			//******************************************
			//Replace references to our state in events.
			for (int i = 0; i < States.Count; ++i) {
				List<KFSMEvent> thisstateevents = States [i].StateEvents;
				for (int j = 0; j < thisstateevents.Count; ++j) {
					KFSMEvent thisevent = thisstateevents [j];

					if (thisevent.GoToStateOnEvent == originalstate) {
						thisevent.GoToStateOnEvent = this;

					}
				}
			}


		}

		void H_OnEnter(KFSMState s)
		{
			if (HookCondition (eva)) {
				if (PreOnEnter != null)
					PreOnEnter (eva, s);
				originalstate.OnEnter (s);
				if (PostOnEnter != null)
					PostOnEnter (eva, s);
			}
			else
				originalstate.OnEnter (s);
		}

		void H_OnLeave(KFSMState s)
		{
			if (HookCondition (eva)) {
				if (PreOnLeave != null)
					PreOnLeave (eva, s);
				originalstate.OnLeave (s);
				if (PostOnLeave != null)
					PostOnLeave (eva, s);
			}
			else
				originalstate.OnLeave (s);
		}

		void H_OnUpdate()
		{
			if (HookCondition (eva)) {
				if (PreOnUpdate != null)
					PreOnUpdate (eva);
				if (!Override_OnUpdate)
					originalstate.OnUpdate ();
				if (PostOnUpdate != null)
					PostOnUpdate (eva);
			}
			else
				originalstate.OnUpdate ();
		}

		void H_OnFixedUpdate()
		{
			if (HookCondition (eva)) {
				if (PreOnFixedUpdate != null)
					PreOnFixedUpdate (eva);
				if (!Override_OnFixedUpdate)
					originalstate.OnFixedUpdate ();
				if (PostOnFixedUpdate != null)
					PostOnFixedUpdate (eva);
			}
			else
				originalstate.OnFixedUpdate ();
		}

		void H_OnLateUpdate()
		{
			if (HookCondition (eva)) {
				if (PreOnLateUpdate != null)
					PreOnLateUpdate (eva);
				if (!Override_OnLateUpdate)
					originalstate.OnLateUpdate ();
				if (PostOnLateUpdate != null)
					PostOnLateUpdate (eva);
			}
			else
				originalstate.OnLateUpdate ();
		}



	}
}

