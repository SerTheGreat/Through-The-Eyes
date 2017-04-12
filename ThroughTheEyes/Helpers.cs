using System;
using UnityEngine;

namespace FirstPerson
{
	public static class Helpers
	{

		public static Vector3 ClampVectorComponents(Vector3 v, float min, float max)
		{
			Vector3 ret = new Vector3 ();
			ret.x = Mathf.Clamp (v.x, min, max);
			ret.y = Mathf.Clamp (v.y, min, max);
			ret.z = Mathf.Clamp (v.z, min, max);
			return ret;
		}

		public static Vector3 PairwiseMultiplyVectors(Vector3 a, Vector3 b)
		{
			Vector3 ret = new Vector3 ();
			ret.x = a.x * b.x;
			ret.y = a.y * b.y;
			ret.z = a.z * b.z;
			return ret;
		}


	}
}

