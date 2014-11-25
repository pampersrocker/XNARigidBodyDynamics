using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	public interface IBoundingSphere
	{
		double Radius { get; }
		DVector2 CentralPosition { get; }
	}
}
