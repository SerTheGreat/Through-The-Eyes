using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPerson
{
	public class EVAIVAState
	{
		public KerbalEVA myeva = null;
		public Vector3 rotationpid_integral = Vector3.zero;
		public Vector3 rotationpid_previouserror = Vector3.zero;

		//Persist this one globally for now
		static float seva_throttle = 1f;
		public float eva_throttle { get { return seva_throttle; } set { seva_throttle = value; } }

		public void Reset(KerbalEVA eva)
		{
			myeva = eva;

			rotationpid_integral = Vector3.zero;
			rotationpid_previouserror = Vector3.zero;
		}
	}
}

