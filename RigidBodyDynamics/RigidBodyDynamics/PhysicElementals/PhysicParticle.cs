using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	public class PhysicParticle
	{
		internal DVector2 centerOfMass;
		internal double weight;
		internal double mass;
		internal DVector2 velocity;
		internal DVector2 forces;
		PhysicWorldParameters phyWorldParameters;

		public event Action CenterOfMassChanged;

		public DVector2 Velocity
		{
			get { return velocity; }
		}

		public DVector2 CenterOfMass
		{
			get { return centerOfMass; }
			set
			{
				centerOfMass = value;
				try
				{
					CenterOfMassChanged();
				}
				catch (Exception)
				{

				};
			}
		}

		public double Weight
		{
			get { return weight; }
			set { weight = value; }
		}

		public PhysicParticle(DVector2 centerOfMass, double weight, PhysicWorldParameters phyWorldParams)
		{
			this.centerOfMass = centerOfMass;
			this.weight = weight;
			this.mass = weight;// / phyWorldParams.Gravity;
			phyWorldParameters = phyWorldParams;
			phyWorldParameters.GravityChanged += new PhysicWorldParameters.OnDoubleChange(phyWorldParameters_GravityChanged);
		}

		void phyWorldParameters_GravityChanged(double value)
		{
			mass = weight;// /value;
		}

		public void ApplyForce(DVector2 force)
		{
			forces += force;
		}
	}
}
