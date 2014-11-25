using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RigidBodyDynamics.PhysicElementals
{
    class HexagonalRigidBody : PhysicBody
    {
        double radius;
        List<DVector2> points;

        public HexagonalRigidBody(DVector2 position, double radius, double weight, PhysicWorldParameters phyparam)
            : base(new DVector2(0,0), weight, phyparam)
        {
            gPosition = position;
            base.orientation = 0;
            points = new List<DVector2>();
            DVector2 tmp =new DVector2(0, -radius);
            tmp.Rotate(orientation);
            points.Add(tmp+ position);
            this.radius = radius;
            for (int i = 0; i < 5; i++)
			{
                tmp = tmp.Rotate(Math.PI*(1.0/3.0));
                points.Add(tmp + position);
			}
            CentralPositionChanged += HexagonalRigidBody_CentralPositionChanged;
            CalculateMomentOfInertia();
            

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

        void HexagonalRigidBody_CentralPositionChanged()
        {
            SetVertices();
        }

        private void SetVertices()
        {
            points = new List<DVector2>();
            DVector2 tmp = new DVector2(0, -radius);
            tmp = tmp.Rotate(orientation);
            points.Add(tmp+ gPosition);
            for (int i = 1; i < 6; i++)
            {
                tmp = tmp.Rotate(Math.PI * (1.0 / 3.0));
                points.Add(tmp + gPosition);
            }
        }


        protected override void CalculateMomentOfInertia()
        {
            if (points != null)
            {
                momentOfInertia = 0;
                for (int i = 0; i < 6; i++)
                {
                    momentOfInertia += (mass / 6.0) * (points[i] - gPosition).LengthSquared();
                }
            }
        
        }

        protected override void ApplyForces()
        {
           // throw new NotImplementedException();
        }

        public override List<DVector2> GetVertices
        {
            get {
                return points; }
        }

        public override double Radius
        {
            get { return radius; }
        }

        public override IConvexShape GetUpdatedPoints(double elapsedTime)
        {
            List<DVector2> newpoints = new List<DVector2>();

            DVector2 tmp = new DVector2(0, -radius);
            tmp = tmp.Rotate(orientation + angularVelocity * elapsedTime);
            newpoints.Add(tmp + gPosition + velocity * elapsedTime);
            for (int i = 0; i < 5; i++)
            {
                tmp = tmp.Rotate(Math.PI * (1.0 / 3.0));
                newpoints.Add(tmp + gPosition + velocity*elapsedTime);
            }
            newpoints.AddRange(points);

            return new ConvexShape(gPosition + velocity * elapsedTime, radius, newpoints);
        }
    }
}
