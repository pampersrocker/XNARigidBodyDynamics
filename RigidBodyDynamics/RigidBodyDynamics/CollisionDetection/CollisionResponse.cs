using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.CollisionDetection
{
	public enum CollisionType
	{
		None,
		Intersecting,
		Penetrating
	}

	public struct CollisionResponse
	{

		public CollisionType CollisionType;
		public DVector2 CollisionPointRelativeToBody1;
		public DVector2 CollisionPointRelativeToBody2;
		public DVector2 CollisionVector;
		public bool Colliding;

	}
}
