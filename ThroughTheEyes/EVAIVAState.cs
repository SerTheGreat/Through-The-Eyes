using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	public class EVAIVAState
	{
		public Quaternion kerballookrotation = new Quaternion();
		public KerbalEVA myeva = null;
		public Vector3 PackSASTarget_Rot_Up = Vector3.up;
		public Vector3 PackSASTarget_Rot_Fwd = Vector3.forward;

		public void Reset(KerbalEVA eva)
		{
			kerballookrotation = new Quaternion ();
			myeva = eva;

			PackSASTarget_Rot_Up = eva.transform.up;
			PackSASTarget_Rot_Fwd = eva.transform.forward;
		}
	}
}

