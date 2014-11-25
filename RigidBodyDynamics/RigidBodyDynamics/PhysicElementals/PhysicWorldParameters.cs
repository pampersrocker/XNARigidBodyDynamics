using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RigidBodyDynamics.PhysicElementals
{
	public class PhysicWorldParameters
	{
		private double gravity;

		public double Gravity
		{
			get { return gravity; }
			set
			{
				gravity = value;
				if (GravityChanged != null)
				{
					GravityChanged(value);
				}
			}
		}

		public PhysicWorldParameters()
		{
			gravity = 981f;
		}



		public delegate void OnDoubleChange(double value);

		public event OnDoubleChange GravityChanged;


	}
}
