using System;
using System.Collections.Generic;

namespace FirstPerson
{
	public static class ReflectedMembers
	{
		static bool hasrefs = false;

		//Kerbal FSM
		internal static System.Reflection.FieldInfo fsm_states = null;
		internal static System.Reflection.FieldInfo fsm_currentstate = null;
		internal static System.Reflection.FieldInfo fsm_laststate = null;

		//Kerbal EVA
		internal static System.Reflection.FieldInfo eva_onjumpcomplete = null;
		internal static System.Reflection.FieldInfo eva_tgtFwd = null;
		internal static System.Reflection.FieldInfo eva_tgtUp = null;
		internal static System.Reflection.FieldInfo eva_manualAxisControl = null;
		internal static System.Reflection.FieldInfo eva_packRRot = null;
		internal static System.Reflection.FieldInfo eva_packTgtRPos = null;
		internal static System.Reflection.FieldInfo eva_cmdRot = null;
		internal static System.Reflection.FieldInfo eva_fuelFlowRate = null;
		internal static System.Reflection.FieldInfo eva_packLinear = null;
		internal static System.Reflection.FieldInfo eva_tgtRpos = null;
		internal static System.Reflection.FieldInfo eva_lastDeltaHdg = null;
		internal static System.Reflection.FieldInfo eva_deltaHdg = null;
		internal static System.Reflection.FieldInfo eva_ladderTgtRPos = null;
		internal static System.Reflection.FieldInfo eva_integral = null;
		internal static System.Reflection.FieldInfo eva_prev_error = null;

		internal static System.Reflection.MethodInfo eva_m_HandleMovementInput = null;
		internal static System.Reflection.MethodInfo eva_m_correctGroundedRotation = null;
		internal static System.Reflection.MethodInfo eva_m_UpdateMovement = null;
		internal static System.Reflection.MethodInfo eva_m_UpdateHeading = null;
		internal static System.Reflection.MethodInfo eva_m_updateRagdollVelocities = null;
		internal static System.Reflection.MethodInfo eva_m_UpdatePackLinear = null;

		//Kerbal EVA state members
		internal static List<System.Reflection.FieldInfo> eva_type_kfsmstate = new List<System.Reflection.FieldInfo>();


		public static void Initialize()
		{
			if (hasrefs)
				return;
			hasrefs = true;

			try {
				//KerbalFSM Fields
				System.Reflection.MemberInfo[] mi = typeof(KerbalFSM).FindMembers (System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null, null);
				foreach (System.Reflection.MemberInfo m in mi) {
					if (m.Name == "States")
						fsm_states = (System.Reflection.FieldInfo)m;
					else if (m.Name == "currentState")
						fsm_currentstate = (System.Reflection.FieldInfo)m;
					else if (m.Name == "lastState")
						fsm_laststate = (System.Reflection.FieldInfo)m;
				}

				//KerbalEVA Fields
				eva_type_kfsmstate.Clear ();
				mi = typeof(KerbalEVA).FindMembers (System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null, null);
				foreach (System.Reflection.MemberInfo m in mi) {
					System.Reflection.FieldInfo tf = m as System.Reflection.FieldInfo;
					if (tf == null)
						continue;

					if (m.Name == "On_jump_complete")
						eva_onjumpcomplete = tf;
					else if (m.Name == "tgtFwd")
						eva_tgtFwd = tf;
					else if (m.Name == "tgtUp")
						eva_tgtUp = tf;
					else if (m.Name == "manualAxisControl")
						eva_manualAxisControl = tf;
					else if (m.Name == "packRRot")
						eva_packRRot = tf;
					else if (m.Name == "packTgtRPos")
						eva_packTgtRPos = tf;
					else if (m.Name == "cmdRot")
						eva_cmdRot = tf;
					else if (m.Name == "fuelFlowRate")
						eva_fuelFlowRate = tf;
					else if (m.Name == "packLinear")
						eva_packLinear = tf;
					else if (m.Name == "tgtRpos")
						eva_tgtRpos = tf;
					else if (m.Name == "lastDeltaHdg")
						eva_lastDeltaHdg = tf;
					else if (m.Name == "deltaHdg")
						eva_deltaHdg = tf;
					else if (m.Name == "ladderTgtRPos")
						eva_ladderTgtRPos = tf;
					else if (m.Name == "integral")
						eva_integral = tf;
					else if (m.Name == "prev_error")
						eva_prev_error = tf;

					if (tf.FieldType == typeof(KFSMState))
						eva_type_kfsmstate.Add (tf);
				}

				mi = typeof(KerbalEVA).FindMembers (System.Reflection.MemberTypes.Method, System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null, null);
				foreach (System.Reflection.MemberInfo m in mi) {
					System.Reflection.MethodInfo tf = m as System.Reflection.MethodInfo;
					if (tf == null)
						continue;

					if (m.Name == "HandleMovementInput")
						eva_m_HandleMovementInput = tf;
					else if (m.Name == "correctGroundedRotation")
						eva_m_correctGroundedRotation = tf;
					else if (m.Name == "UpdateMovement")
						eva_m_UpdateMovement = tf;
					else if (m.Name == "UpdateHeading")
						eva_m_UpdateHeading = tf;
					else if (m.Name == "updateRagdollVelocities")
						eva_m_updateRagdollVelocities = tf;
					else if (m.Name == "UpdatePackLinear")
						eva_m_UpdatePackLinear = tf;
				}

			} finally {
				WarnOnNotFound ();
			}


		}

		static void WarnOnNotFound()
		{
			System.Reflection.MemberInfo[] mi = typeof(ReflectedMembers).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public, null, null);
			foreach (System.Reflection.MemberInfo m in mi) {
				System.Reflection.FieldInfo tf = m as System.Reflection.FieldInfo;
				if (tf == null)
					continue;

				if (typeof(System.Reflection.MemberInfo).IsAssignableFrom (tf.FieldType)) {
					System.Reflection.MemberInfo val = tf.GetValue (null) as System.Reflection.MemberInfo;
					if (val == null) {
						KSPLog.print ("WARNING: REFLECTEDMEMBERS cannot find member: '" + tf.Name + "'");
					} else {
						KSPLog.print ("RM found " + tf.Name);
					}
				}
			}
		}


	}
}

