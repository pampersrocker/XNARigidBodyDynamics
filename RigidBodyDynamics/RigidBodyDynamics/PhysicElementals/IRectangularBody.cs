using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	public interface IRectangularBody : IBoundingSphere
	{
		int Width { get; set; }
		int Height { get; set; }
		double Orientation { get; set; }
		DVector2 RelativeCOMToPosition { get; set; }
	}
}
