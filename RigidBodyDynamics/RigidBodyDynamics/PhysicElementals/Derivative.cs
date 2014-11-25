using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RigidBodyDynamics.PhysicElementals
{
    public struct Derivative
    {
        public DVector2 dx;
        public DVector2 dv;
        public double dA;
        public double dAV;

        public Derivative(DVector2 dx, DVector2 dv,double dA,double dAV)
        {
            this.dx = dx;
            this.dv = dv;
            this.dA = dA;
            this.dAV = dAV;
        }
    }
}
