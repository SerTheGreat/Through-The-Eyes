using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//using Toolbar;

namespace FirstPerson
{
    class FPGUI : MonoBehaviour
    {

		private const float TICK_HEIGHT = 0.01f;
		private const float TICK_WIDTH = 0.009f;
		private const float TICK_GAP = 0.001f;
    
		public float yawAngle;
		public float pitchAngle;
	
		static Material lineMaterial;
		static void CreateLineMaterial ()
		{
			if (!lineMaterial)
			{
				// Unity has a built-in shader that is useful for drawing
				// simple colored things.
				Shader shader = Shader.Find ("Hidden/Internal-Colored");
				lineMaterial = new Material (shader);
				lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				lineMaterial.SetInt ("_ZWrite", 0);
			}
		}
	
		// Will be called after all regular rendering is done
		public void OnRenderObject ()
		{
			CreateLineMaterial ();
			// Apply the line material
			lineMaterial.SetPass (0);
	
			GL.PushMatrix ();
			// Set transformation matrix for drawing to
			// match our transform
			//GL.MultMatrix (transform.localToWorldMatrix);
			GL.LoadOrtho();
	
			// Draw horizontal ticks:
			GL.Begin (GL.QUADS);
			GL.Color(new Color(0, 1f, 0, 0.2f));

			int repeats = Mathf.RoundToInt(Math.Abs(yawAngle) / (90 / (0.5f / (TICK_WIDTH + TICK_GAP))));
			for (int i = 0; i < repeats; ++i)
			{
				if (i == 0) {
					continue;
				}
				GL.Vertex3(0.5f + Math.Sign(yawAngle) * i * (TICK_WIDTH + TICK_GAP) - TICK_WIDTH / 2, 0.001f, 0);
				GL.Vertex3(0.5f + Math.Sign(yawAngle) * i * (TICK_WIDTH + TICK_GAP) - TICK_WIDTH / 2, 0.001f + TICK_HEIGHT, 0);
				GL.Vertex3(0.5f + Math.Sign(yawAngle) * i * (TICK_WIDTH + TICK_GAP) + TICK_WIDTH - TICK_WIDTH / 2, 0.001f + TICK_HEIGHT, 0);
				GL.Vertex3(0.5f + Math.Sign(yawAngle) * i * (TICK_WIDTH + TICK_GAP) + TICK_WIDTH - TICK_WIDTH / 2, 0.001f, 0);
			}

			//Draw vertical ticks:
			repeats = Mathf.RoundToInt(Math.Abs(pitchAngle) / (90 / (0.5f / (TICK_HEIGHT + TICK_GAP))));
			for (int i = 0; i < repeats; ++i)
			{
				if (i == 0) {
					continue;
				}
				GL.Vertex3(0.001f, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) - TICK_HEIGHT / 2, 0);
				GL.Vertex3(0.001f + TICK_WIDTH, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) - TICK_HEIGHT / 2, 0);
				GL.Vertex3(0.001f + TICK_WIDTH, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) + TICK_HEIGHT - TICK_HEIGHT / 2, 0);
				GL.Vertex3(0.001f, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) + TICK_HEIGHT - TICK_HEIGHT / 2, 0);

				GL.Vertex3(1f - TICK_WIDTH - 0.001f, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) - TICK_HEIGHT / 2, 0);
				GL.Vertex3(1f - TICK_WIDTH - 0.001f + TICK_WIDTH, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) - TICK_HEIGHT / 2, 0);
				GL.Vertex3(1f - TICK_WIDTH - 0.001f + TICK_WIDTH, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) + TICK_HEIGHT - TICK_HEIGHT / 2, 0);
				GL.Vertex3(1f - TICK_WIDTH - 0.001f, 0.5f + Math.Sign(pitchAngle) * i * (TICK_HEIGHT + TICK_GAP) + TICK_HEIGHT - TICK_HEIGHT / 2, 0);
			}

			GL.End ();
			GL.PopMatrix ();
		}
        

    }
}
