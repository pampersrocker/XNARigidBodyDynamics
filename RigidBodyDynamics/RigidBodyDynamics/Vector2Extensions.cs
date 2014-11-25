using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics
{
	public static class Vector2Extensions
	{
		/// <summary>
		/// Returns the Angle of the Vector2
		/// </summary>
		/// <param name="v"></param>
		/// <returns>The Angle of the Vector2 in Radians</returns>
		public static float GetRotation(this Vector2 v)
		{
			float rotation;
			rotation = (float)Math.Acos(v.X / (v.Length()));
			if (v.Y < 0)
				rotation *= -1;
			return rotation;
		}

		/// <summary>
		/// Rotates a <c>Vector</c> <c>Absolute or Relative</c> 
		/// to the defined <c>Angle</c>
		/// </summary>
		/// <param name="v">The Vector to Rotate</param>
		/// <param name="rad">The Angle</param>
		/// <param name="absolute">Determines if the Angle is absolute or not</param>
		/// <returns>The rotated Vector2</returns>
		public static Vector2 Rotate(this Vector2 v, float rad, bool absolute)
		{
			Vector2 vOut = new Vector2(v.X, v.Y);
			vOut = Vector2.Transform(
				vOut, 
				Matrix.CreateRotationZ(
				rad - (absolute ? vOut.GetRotation() : 0)));
			return vOut;
		}

		/// <summary>
		/// Rotates a <c>Vector</c> relative to its current Rotation
		/// </summary>
		/// <param name="v">The Vector which will be rotated</param>
		/// <param name="rad">The angle in Radians</param>
		/// <returns>The rotated Vector</returns>
		public static Vector2 Rotate(this Vector2 v, float rad)
		{
			return Rotate(v, rad, false);
		}
	}
}
