using UnityEngine;

namespace FirstPerson
{
	public class DebugUtils
	{
		public static void visualizeTransform(Transform transform, Color color) {
			if (transform == null) {
				return;
			}
			KSPLog.print("TRANSFORM: " + transform.position);
			GameObject gameObject = transform.gameObject;
			Vector3 origin = transform.position;
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			if (lineRenderer == null) {
				lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
				lineRenderer.SetVertexCount(6);
			}
			lineRenderer.SetWidth(0.05f, 0.01f);
			lineRenderer.SetColors(color, Color.white);
			lineRenderer.SetPosition(0, origin);
			lineRenderer.SetPosition(1, transform.TransformPoint(Vector3.right));
			lineRenderer.SetPosition(2, origin);
			lineRenderer.SetPosition(3, transform.TransformPoint(Vector3.up));
			lineRenderer.SetPosition(4, origin);
			lineRenderer.SetPosition(5, transform.TransformPoint(Vector3.forward));
		}
	}
}

