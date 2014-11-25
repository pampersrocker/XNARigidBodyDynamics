using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
    public abstract class PhysicBody : PhysicParticle, IConvexShape
    {
        public enum PhysicBodyType
        {
            RigidBody,
            StaticBody
        }

        internal double torque;//Total Moment on body (Rotational Moment)
        internal double momentOfInertia;
        internal PhysicWorldParameters pyhWorldParams;
        internal DVector2 gPosition;
        private double speed;
        internal double angularVelocity = 0;
        internal double orientation = 0;
        internal PhysicBodyType phyBodyType = PhysicBodyType.RigidBody;
        internal double frictionCoefficient = 0.2;
        private bool calculatedUpdatedPosition = false;
        private List<DVector2> updatedPointstmp = new List<DVector2>();
        private IConvexShape shape;

        public PhysicBodyType PhyBodyType
        {
            get { return phyBodyType; }
            set { phyBodyType = value; }
        }

        public event Action CentralPositionChanged;

        public PhysicBody(DVector2 centerOfMass, double Weight, PhysicWorldParameters phyWorldParams)
            : base(centerOfMass, Weight, phyWorldParams)
        {
            this.pyhWorldParams = phyWorldParams;
            phyWorldParams.GravityChanged += new PhysicWorldParameters.OnDoubleChange(phyWorldParams_GravityChanged);
            CalculateMomentOfInertia();
        }

        private void phyWorldParams_GravityChanged(double value)
        {
            CalculateMomentOfInertia();
        }

        protected abstract void CalculateMomentOfInertia();

        protected abstract void ApplyForces();

        public void Update(double elapsedTime)
        {
            //ApplyForce(new Vector2(0, this.pyhWorldParams.Gravity));

            //velocity.Y += pyhWorldParams.Gravity * elapsedTime;
            if (phyBodyType != PhysicBodyType.StaticBody)
            {
                ApplyForce(new DVector2(0, pyhWorldParams.Gravity * mass));
                calculatedUpdatedPosition = false;
                UpdateBody(elapsedTime);
                forces = DVector2.Zero;
                torque = 0; 
            }
        }

        public void ApplyForce(DVector2 force, DVector2 positionRelToCenter)
        {
            torque += DVector2.Dot(force, new DVector2(-positionRelToCenter.Y, positionRelToCenter.X));
            forces += force;
        }

        protected void UpdateBody(double elapsedTime)
        {
            if (phyBodyType != PhysicBodyType.StaticBody)
            {
                //PhysicBody body = (PhysicBody)this.MemberwiseClone();
                DVector2 accTmp, k1, k2;
                double angularAccTmp, k1a, k2a, integratedTime = elapsedTime;

                #region Runge-Kutta 4

                State state = new State(gPosition, velocity, orientation, angularVelocity);

                Derivative a = Evaluate(state, 0, new Derivative(velocity, new DVector2(), angularVelocity, 0));
                Derivative b = Evaluate(state, elapsedTime * 0.5, a);
                Derivative c = Evaluate(state, elapsedTime * 0.5, b);
                Derivative d = Evaluate(state, elapsedTime, c);

                DVector2 dxdt = (1.0 / 6.0) * (a.dx + 2.0 * (b.dx + c.dx) + d.dx);
                DVector2 dvdt = (1.0 / 6.0) * (a.dv + 2.0 * (b.dv + c.dv) + d.dv);

                double Orientationdt = 1.0 / 6.0 * (a.dA + 2.0 * (b.dA + c.dA) + d.dA);
                double AngularVelocitydt = 1.0 / 6.0 * (a.dAV + 2.0 * (b.dAV + c.dAV) + d.dAV);

                gPosition += dxdt * elapsedTime;
                velocity += dvdt * elapsedTime;

                orientation += Orientationdt * elapsedTime;
                angularVelocity += AngularVelocitydt * elapsedTime;

                #endregion Runge-Kutta 4

                #region Euler

                //this.velocity += (forces / this.mass) * integratedTime;
                //this.angularVelocity += (this.torque / this.momentOfInertia) * integratedTime;
                //
                //this.gPosition += this.velocity * integratedTime;
                //this.orientation += angularVelocity * integratedTime;

                #endregion Euler

                angularVelocity *=0.9;
                velocity *= 0.99;

                try
                {
                    CentralPositionChanged();
                }
                catch (Exception)
                {
                }

                speed = velocity.Length();
            }
        }

        protected virtual DVector2 Acceleration(State state, double dt)
        {
            return forces / mass;
        }

        protected Derivative Evaluate(State initial, double dt, Derivative d)
        {
            State state = new State(new DVector2(), new DVector2(), 0, 0);
            state.x = initial.x + d.dx * dt;
            state.v = initial.v + d.dv * dt;

            state.Orientation = initial.Orientation + d.dA * dt;
            state.angularVelocity = initial.angularVelocity + d.dAV * dt;

            Derivative output = new Derivative();
            output.dx = state.v;
            output.dv = Acceleration(state, dt);
            output.dA = state.angularVelocity;
            output.dAV = torque / momentOfInertia;
            return output;
        }

        internal void ApplyImpulse(CollisionDetection.CollisionResponse response, PhysicBody body2, double elapsedTime)
        {
            if (response.Colliding)
            {
                bool doRespone = false;
                DVector2 body1AngularVelocity = new DVector2(-response.CollisionPointRelativeToBody1.Y, response.CollisionPointRelativeToBody1.X);

                //body1AngularVelocity.Normalize();
                body1AngularVelocity *= angularVelocity;
                DVector2 body1Velocity = velocity + body1AngularVelocity;

                //Rigid Body Collision Response
                if (body2.phyBodyType != PhysicBodyType.StaticBody)
                {
                    double j;
                    doRespone = false;
                    DVector2 body2AngularVelocity = new DVector2(-response.CollisionPointRelativeToBody2.Y, response.CollisionPointRelativeToBody2.X);

                    //body2AngularVelocity.Normalize();
                    body2AngularVelocity *= body2.angularVelocity;
                    DVector2 body2Velocity = body2.velocity + body2AngularVelocity;

                    //if (body1Velocity.LengthSquared() == 0 || body2Velocity.LengthSquared() == 0)
                    //{
                    //    doRespone = true;
                    //}
                    //else

                    //TODO Implement Rotating Speed
                    //doRespone = DVector2.Dot(gPosition - body2.gPosition, body2Velocity - body1Velocity) >= 0f;
                    //
                    DebugLine line1 = new DebugLine(gPosition, gPosition + response.CollisionPointRelativeToBody1, Color.Blue);
                    DebugLine line2 = new DebugLine(body2.gPosition , body2.gPosition + response.CollisionPointRelativeToBody2 , Color.Blue);

                    doRespone = DVector2.Dot(body1Velocity - body2Velocity, response.CollisionVector) > 0;

                    //doRespone = true;
                    if (doRespone)
                    {
                        DebugDrawer.AddLine(line1);
                        //DebugDrawer.AddLine(line2);
                        double epsilon = 1;
                        double jNom =
                        -(1 + epsilon) *
                        ((body1Velocity.X - body2Velocity.X) *
                        response.CollisionVector.X +
                        (body1Velocity.Y - body2Velocity.Y) *
                        response.CollisionVector.Y);

                        j = (response.CollisionVector.X * response.CollisionVector.X +
                            response.CollisionVector.Y * response.CollisionVector.Y) *
                            (1 / this.mass + 1 / body2.mass);
                        j += Math.Pow(
                            -response.CollisionPointRelativeToBody1.Y *
                            response.CollisionVector.X +
                            response.CollisionVector.Y *
                            response.CollisionPointRelativeToBody1.X, 2) /
                            momentOfInertia;
                        j += Math.Pow(
                            -response.CollisionPointRelativeToBody2.Y *
                            response.CollisionVector.X +
                            response.CollisionVector.Y *
                            response.CollisionPointRelativeToBody2.X, 2) /
                            body2.momentOfInertia;

                        if (j != 0)
                        {
                            j = jNom / j;

                            this.velocity += (j * response.CollisionVector) / mass;

                            this.angularVelocity += (-response.CollisionPointRelativeToBody1.Y * response.CollisionVector.X * j + response.CollisionPointRelativeToBody1.X * response.CollisionVector.Y * j) / momentOfInertia;

                            body2.velocity -= (j * response.CollisionVector) / body2.mass;

                            body2.angularVelocity -= (-response.CollisionPointRelativeToBody2.Y * response.CollisionVector.X * j + response.CollisionPointRelativeToBody2.X * response.CollisionVector.Y * j) / body2.momentOfInertia;

                            //DVector2 posDif1 = (this.velocity * elapsedTime);
                            //DVector2 posDif2 = (body2.velocity * elapsedTime);

                            //response.CollisionDepth = response.CollisionDepth - this.velocity * elapsedTime -  body2.velocity * elapsedTime;

                            DVector2 posDif1 = (-body1Velocity * elapsedTime);

                            // DVector2 posDif2 = (-body2Velocity * elapsedTime);

                            DVector2 collisionDif1 = (response.CollisionPointRelativeToBody1 * elapsedTime);

                            // DVector2 collisionDif2 = (response.CollisionPointRelativeToBody2 * elapsedTime);

                            if (DVector2.Dot(posDif1, collisionDif1) < 0)
                            {
                                //TODO fix rotation correction
                                //gPosition += ((posDif1) - collisionDif1);
                                //orientation += DVector2.Dot(posDif1, collisionDif1) * angularVelocity;
                            }

                            //if (DVector2.Dot(posDif2, collisionDif2) < 0)
                            //{
                            //    //body2.gPosition -= (posDif2 - collisionDif2) * (body2.mass/(mass+body2.mass));
                            //}

                            DVector2 slidingVector1 = new DVector2(-response.CollisionPointRelativeToBody1.Y, response.CollisionPointRelativeToBody1.X);

                            //slidingVector1.Normalize();
                            DVector2 slidingVector2 = new DVector2(-response.CollisionPointRelativeToBody2.Y, response.CollisionPointRelativeToBody2.X);

                            //slidingVector2.Normalize();

                            //ApplyForce(-frictionCoefficient * (slidingVector1 *velocity) * slidingVector1 / slidingVector1.Length(), response.CollisionPointRelativeToBody1);
                            //body2.ApplyForce(-frictionCoefficient * (slidingVector2 * body2.velocity) * slidingVector2 / slidingVector2.Length(), response.CollisionPointRelativeToBody2);
                            if (body2.phyBodyType == PhysicBodyType.StaticBody)
                            {
                                body2.velocity = DVector2.Zero;
                                body2.angularVelocity = 0;
                            }

                            //ApplyForce(-frictionCoefficient * (velocity / velocity.Length()));
                            //body2.ApplyForce(frictionCoefficient * (body2.velocity / body2.velocity.Length()));
                        }
                    }
                }

                //Static Body Response
                else
                {
                    //TODO Fix DropThrough if Moving Down right.....
                    //doRespone = DVector2.Dot(gPosition - body2.gPosition, body1Velocity) <= 0;
                    //doRespone = DVector2.Dot(body1Velocity, response.CollisionVector) > 0;

                    doRespone = true;
                    if (doRespone)
                    {
                        double j = 0;
                        double epsilon = 0.5;
                        double jNom =
                            -(1 + epsilon) * (body1Velocity.X * response.CollisionVector.X + body1Velocity.Y * response.CollisionVector.Y);

                        j = (response.CollisionVector.X * response.CollisionVector.X + response.CollisionVector.Y * response.CollisionVector.Y) * (1.0 / this.mass);
                        j += Math.Pow(-response.CollisionPointRelativeToBody1.Y * response.CollisionVector.X + response.CollisionVector.Y * response.CollisionPointRelativeToBody1.X, 2) / momentOfInertia;

                        if (j != 0)
                        {
                            j = jNom / j;
                            this.velocity += (j * response.CollisionVector) / mass;

                            this.angularVelocity += (-response.CollisionPointRelativeToBody1.Y * response.CollisionVector.X * j + response.CollisionPointRelativeToBody1.X * response.CollisionVector.Y * j) / momentOfInertia;

                            DVector2 posDif = (gPosition + this.velocity * elapsedTime - gPosition);

                            //DVector2 posDif = -body1Velocity * elapsedTime;
                            DVector2 collisionDif = (response.CollisionPointRelativeToBody1 * elapsedTime);
                            if (DVector2.Dot(posDif, collisionDif) < 0)
                            {
                                //gPosition += (posDif) - collisionDif;
                            }

                            //gPosition -= response.CollisionDepth;

                            //if (response.CollisionDepth.X < response.CollisionDepth.Y)
                            //{
                            //    gPosition.X -= response.CollisionDepth.X;
                            //}
                            //else
                            //{
                            //    gPosition.Y -= response.CollisionDepth.Y;
                            //}
                        }

                        this.velocity -= pyhWorldParams.Gravity / 10 * (response.CollisionPointRelativeToBody1 / response.CollisionPointRelativeToBody1.Length()) / mass;

                        DVector2 slidingVector1 = new DVector2(response.CollisionVector.X, response.CollisionVector.Y);

                        //slidingVector1.Normalize();
                        slidingVector1 = (slidingVector1 * velocity) * slidingVector1 / slidingVector1.Length();

                        //slidingVector1.Normalize();
                        //ApplyForce(-frictionCoefficient * slidingVector1, response.CollisionPointRelativeToBody1);

                        // ApplyFriction(-frictionCoefficient* (velocity / velocity.Length()), response.CollisionPointRelativeToBody1);

                        //ApplyForce(-frictionCoefficient * (velocity / velocity.Length()));
                    }
                }

                //Push Object away from each other
                //if ((!doRespone ||body2.PhyBodyType == PhysicBodyType.StaticBody))
                //{
                //    this.velocity -= pyhWorldParams.Gravity/10 * (response.CollisionPointRelativeToBody1 / response.CollisionPointRelativeToBody1.Length()) / mass;

                //    body2.velocity -= pyhWorldParams.Gravity/10 * (response.CollisionPointRelativeToBody2 / response.CollisionPointRelativeToBody2.Length()) / body2.mass;

                //}
            }
        }

        internal DVector2 GetIntegratedVelocity(double elapsedTime)
        {
            PhysicBody body = (PhysicBody)this.MemberwiseClone();
            DVector2 accTmp, k1, k2;
            double integratedTime = elapsedTime;
            body.velocity.Y += body.pyhWorldParams.Gravity * elapsedTime;

            //body.ApplyForces();
            //ApplyForces();
            accTmp = body.forces / body.mass;
            k1 = accTmp * integratedTime;

            body.ApplyForces();

            accTmp = body.forces / body.mass;
            k2 = accTmp * integratedTime;

            return (body.velocity + (k1 + k2) / 2f) * integratedTime;
        }

        internal double GetIntegratedAngularVelocity(double elapsedTime)
        {
            PhysicBody body = (PhysicBody)this.MemberwiseClone();
            double angularAccTmp, k1a, k2a, integratedTime = elapsedTime;

            //body.ApplyForces();
            //ApplyForces();

            angularAccTmp = body.torque / body.momentOfInertia;
            k1a = angularAccTmp * integratedTime;
            body.ApplyForces();

            angularAccTmp = body.torque / body.momentOfInertia;
            k2a = angularAccTmp * integratedTime;

            return (body.angularVelocity + (k1a + k2a) / 2f) * integratedTime;
        }

        public override string ToString()
        {
            return gPosition.ToString() + " : " + velocity.ToString();
        }

        #region ConvexShape Member

        public abstract List<DVector2> GetVertices { get; }

        #endregion ConvexShape Member

        #region convexShape

        private enum CurrentShape
        {
            Shape1, Shape2
        }

        #endregion convexShape

        #region IBoundingSphere Member

        public abstract double Radius { get; }

        public DVector2 CentralPosition
        {
            get { return gPosition; }
        }

        #endregion IBoundingSphere Member

        public abstract IConvexShape GetUpdatedPoints(double elapsedTime);

        public IConvexShape GetUpdatedConvexShape(double elapsedTime)
        {
            if (calculatedUpdatedPosition == false)
            {
                updatedPointstmp.Clear();
                updatedPointstmp.AddRange(GetUpdatedPoints(elapsedTime).GetVertices);

                updatedPointstmp.AddRange(GetVertices);

                shape = new ConvexShape(gPosition, Radius, ConvexShape.getConvexShape(updatedPointstmp).GetVertices);
                calculatedUpdatedPosition = true;
            }
            return shape;
        }
    }
}