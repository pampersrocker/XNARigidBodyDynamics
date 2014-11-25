using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	public interface IConvexShape : IBoundingSphere
	{
		/// <summary>
		/// Returns a List of the vertices, which form the Convex the Body (point in index 0 is connected with point in index 1 and so on...)
		/// </summary>
		List<DVector2> GetVertices { get; }

       IConvexShape GetUpdatedPoints(double elapsedTime);
	}
}
