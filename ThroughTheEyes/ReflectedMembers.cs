using System;

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


		public static void Initialize()
		{
			if (hasrefs)
				return;
			hasrefs = true;

			System.Reflection.MemberInfo[] mi = typeof(KerbalFSM).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField| System.Reflection.BindingFlags.GetField| System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, null);
			foreach (System.Reflection.MemberInfo m in mi) {
				if (m.Name == "States")
					fsm_states = (System.Reflection.FieldInfo)m;
				else if (m.Name == "currentState")
					fsm_currentstate = (System.Reflection.FieldInfo)m;
				else if (m.Name == "lastState")
					fsm_laststate = (System.Reflection.FieldInfo)m;
			}

			mi = typeof(KerbalEVA).FindMembers(System.Reflection.MemberTypes.Field, System.Reflection.BindingFlags.SetField| System.Reflection.BindingFlags.GetField| System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, null);
			foreach (System.Reflection.MemberInfo m in mi) {
				if (m.Name == "On_jump_complete")
					eva_onjumpcomplete = (System.Reflection.FieldInfo)m;
				else if (m.Name == "tgtFwd")
					eva_tgtFwd = (System.Reflection.FieldInfo)m;
				else if (m.Name == "tgtUp")
					eva_tgtUp = (System.Reflection.FieldInfo)m;
				else if (m.Name == "manualAxisControl")
					eva_manualAxisControl = (System.Reflection.FieldInfo)m;
				else if (m.Name == "packRRot")
					eva_packRRot = (System.Reflection.FieldInfo)m;
				else if (m.Name == "packTgtRPos")
					eva_packTgtRPos = (System.Reflection.FieldInfo)m;
				else if (m.Name == "cmdRot")
					eva_cmdRot = (System.Reflection.FieldInfo)m;
			}


		}


	}
}

