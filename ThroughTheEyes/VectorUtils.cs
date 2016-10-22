using UnityEngine;

namespace FirstPerson
{
	public class VectorUtils
	{

		public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
			// angle in [0,180]
			float angle = Vector3.Angle(a,b);
			float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

			// angle in [-179,180]
			float signed_angle = angle * sign;
			return signed_angle;
		}

		public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal,Vector3 v) {
			planeNormal.Normalize();
			var distance = -Vector3.Dot(planeNormal.normalized, v);
			return v + planeNormal * distance;
		}    

	}
}

