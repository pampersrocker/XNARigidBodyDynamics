using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics
{
    public class DebugLine
    {
        DVector2 start;
        public RigidBodyDynamics.DVector2 Start
        {
            get { return start; }
            set { start = value; }
        }
        DVector2 finish;
        public RigidBodyDynamics.DVector2 Finish
        {
            get { return finish; }
            set { finish = value; }
        }
        Color color;
        public Microsoft.Xna.Framework.Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public DebugLine(DVector2 start, DVector2 finish, Color color)
        {
            this.start = start;
            this.finish = finish;
            this.color = color;
        }


    }
}
