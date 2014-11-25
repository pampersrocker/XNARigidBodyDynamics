using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RigidBodyDynamics.PhysicElementals
{
    public struct State
    {
        public DVector2 x;
        public DVector2 v;
        public double Orientation;
        public double angularVelocity;

        public State(DVector2 x, DVector2 v,double orientation, double angularVelocity)
        {
            this.x = x;
            this.v = v;
            this.Orientation = orientation;
            this.angularVelocity = angularVelocity;
        }
    }
}
