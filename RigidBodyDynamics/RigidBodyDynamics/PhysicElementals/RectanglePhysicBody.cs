using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	public class RectanglePhysicBody : PhysicBody, IRectangularBody
	{
		internal int width;
		internal int height;
		internal double radius;
		List<DVector2> vertices;
        List<DVector2> newvertices = new List<DVector2>();

		public RectanglePhysicBody(DVector2 centralPosition, double weight, int width, int height,PhysicWorldParameters phyWorldParams)
			: base(new DVector2(0,0), weight, phyWorldParams)
		{
			this.width = width;
			this.height = height;
			this.gPosition = centralPosition;
			radius = (double)Math.Sqrt(width * width + height * height);
			CalculateMomentOfInertia();
			vertices = new List<DVector2>();
			vertices.Add(new DVector2(centerOfMass.X - width / 2, centerOfMass.Y - height / 2));
			vertices.Add(new DVector2(centerOfMass.X + width / 2, centerOfMass.Y - height / 2));
			vertices.Add(new DVector2(centerOfMass.X + width / 2, centerOfMass.Y + height / 2));
			vertices.Add(new DVector2(centerOfMass.X - width / 2, centerOfMass.Y + height / 2));
            newvertices.AddRange(vertices);
			Orientation = 0;
			SetVertices();
			this.CentralPositionChanged += new Action(RectanglePhysicBody_CentralPositionChanged);
			
		}

		void RectanglePhysicBody_CentralPositionChanged()
		{
			SetVertices();
		}

		protected override void CalculateMomentOfInertia()
		{
			momentOfInertia = (mass / 12f)*(width * width + height * height);
		}

		protected override void ApplyForces()
		{
			
		}

		

		#region IRectangularBody Member

		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				SetVertices();
			}
		}

		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				SetVertices();
			}
		}

		public DVector2 CentralPosition
		{
			get
			{
				return gPosition;
			}
			set
			{
				gPosition = value;
				SetVertices();
			}
		}

		public double Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				SetVertices();
			}
		}

		private void SetVertices()
		{
			vertices[0] = (new DVector2(-width / 2, -height / 2).Rotate(orientation) + gPosition);
			vertices[1] = (new DVector2(+width / 2, -height / 2).Rotate(orientation) + gPosition);
			vertices[2] = (new DVector2(+width / 2, +height / 2).Rotate(orientation) + gPosition);
			vertices[3] = (new DVector2(-width / 2, +height / 2).Rotate(orientation) + gPosition);
		}

		public DVector2 RelativeCOMToPosition
		{
			get
			{
				return centerOfMass;
			}
			set
			{
				centerOfMass = value;
			}
		}

		

		#endregion

		#region IBoundingSphere Member

		public override double Radius
		{
			get { return radius; }
		}

		#endregion

		public override List<DVector2> GetVertices
		{
			get { return vertices; }
		}

		public override IConvexShape GetUpdatedPoints(double elapsedTime)
		{
            newvertices.Clear();
            DVector2 newPosition = gPosition + base.GetIntegratedVelocity(elapsedTime);
			double newOrientation = orientation + base.GetIntegratedAngularVelocity(elapsedTime);
			newvertices.Add(new DVector2(-width / 2, -height / 2).Rotate(newOrientation) + newPosition);
			newvertices.Add(new DVector2(+width / 2, -height / 2).Rotate(newOrientation) + newPosition);
			newvertices.Add(new DVector2(+width / 2, +height / 2).Rotate(newOrientation) + newPosition);
			newvertices.Add(new DVector2(-width / 2, +height / 2).Rotate(newOrientation) + newPosition);

			return new ConvexShape(newPosition, this.radius, newvertices);
		}
	}
}
