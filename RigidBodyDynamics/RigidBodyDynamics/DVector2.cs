using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics
{
    /// <summary>
    /// Implements a Vector with 2 dimensions, which acts like the Vector2 in the XNA Framework, but with double precision
    /// </summary>
    public struct DVector2
    {
        public double X;
        public double Y;

        public DVector2(double value)
        {
            X = value;
            Y = value;
        }

        public DVector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public DVector2(DVector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        #region Operator

        public static DVector2 operator +(DVector2 v1, DVector2 v2)
        {
            return new DVector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static DVector2 operator -(DVector2 v1, DVector2 v2)
        {
            return new DVector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static DVector2 operator -(DVector2 v1)
        {
            return new DVector2(-v1.X, -v1.Y);
        }

        public static DVector2 operator *(DVector2 v1, double scalar)
        {
            return new DVector2(v1.X * scalar, v1.Y * scalar);
        }

        public static DVector2 operator *(double scalar, DVector2 v1)
        {
            return new DVector2(v1.X * scalar, v1.Y * scalar);
        }

        public static DVector2 operator /(DVector2 v1, double scalar)
        {
            return new DVector2(v1.X / scalar, v1.Y / scalar);
        }

        public static double operator *(DVector2 v1, DVector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static bool operator ==(DVector2 v1, DVector2 v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(DVector2 v1, DVector2 v2)
        {
            return !(v1 == v2);
        }

        public static explicit operator Vector2(DVector2 v1)
        {
            return new Vector2((float)v1.X, (float)v1.Y);
        }

        #endregion

        #region Methods

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y;
        }

        public void Normalize()
        {
            double length = Length();
            X /= length;
            Y /= length;
        }

        /// <summary>
        /// Returns the Angle of the DVector2
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The Angle of the DVector2 in Radians</returns>
        public double GetRotation()
        {
            double rotation;
            rotation = Math.Acos(X / (Length()));
            if (Y < 0)
                rotation *= -1.0;
            return rotation;
        }

        /// <summary>
        /// Rotates the DVector2
        /// </summary>
        /// <param name="radians">The amount of rotation in radians</param>
        /// <param name="relative">When true, the vector will be rotated with the given radians
        /// When false, the vector orientation will be set to the radians</param>
        /// <returns>The rotated Vector</returns>
        public DVector2 Rotate(double radians, bool relative)
        {
            if (!relative)
            {
                radians -= GetRotation();
            }
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);
            return new DVector2(
                cos * X - sin * Y,
                sin * X + cos * Y);
        }

        /// <summary>
        /// Rotates the DVector2
        /// </summary>
        /// <param name="radians">The amount of rotation in radians</param>
        /// <returns>The rotated Vector</returns>
        public DVector2 Rotate(double radians)
        {
            return Rotate(radians, true);
        }

        public static double Dot(DVector2 v1, DVector2 v2)
        {
            return v1 * v2;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(DVector2))
            {
                return this == (DVector2)obj;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            long bitsX = BitConverter.DoubleToInt64Bits(X);
            long bitsY = BitConverter.DoubleToInt64Bits(Y);
            return (int)bitsX ^ (int)(bitsY >> 32);
        }

        public override string ToString()
        {
            return "{ X:" + X.ToString() + " Y:" + Y.ToString() + " }";
        }
        #endregion

        #region ConstantValues

        public static DVector2 Zero
        {
            get
            {
                return new DVector2();
            }
        }

        public static DVector2 One
        {
            get
            {
                return new DVector2(1);
            }
        }

        public static DVector2 OneX
        {
            get
            {
                return new DVector2(1, 0);
            }
        }

        public static DVector2 OneY
        {
            get
            {
                return new DVector2(0, 1);
            }
        }
        #endregion



    }
}
