using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics.PhysicElementals
{
	class ConvexShape : IConvexShape
	{
		DVector2 centralPosition;
		double radius;
		List<DVector2> vertices;

		public ConvexShape(DVector2 centralPosition, double Radius, List<DVector2> vertices)
		{
			this.centralPosition = centralPosition;
			this.radius = Radius;
			this.vertices = vertices;
		}

		#region IConvexShape Member

		public List<DVector2> GetVertices
		{
			get { return vertices; }
		}

		#endregion

		#region IBoundingSphere Member

		public double Radius
		{
			get { return radius; }
		}

		public DVector2 CentralPosition
		{
			get { return centralPosition; }
		}

		#endregion

		public static IConvexShape getConvexShape(List<DVector2> points)
		{
			ConvexShape shape = new ConvexShape(DVector2.Zero, 0, new List<DVector2>());
			DVector2[] hull = new DVector2[2*points.Count];
			points.Sort(new SortVector());
			int i, t, k = 0;

			/* lower hull */
			for (i = 0; i < points.Count; ++i)
			{
				while (k >= 2 && ccw(hull[k - 2], hull[k - 1], points[i]) <= 0) --k;
				hull[k++] =(points[i]);
				
			}

			/* upper hull */
			for (i = points.Count - 2, t = k + 1; i >= 0; --i)
			{
				while (k >= t && ccw(hull[k - 2], hull[k - 1], points[i]) <= 0) --k;
				hull[k++] =(points[i]);
			}
			shape.vertices = hull.ToList();
			shape.vertices.RemoveRange(k, shape.vertices.Count - k);
			shape.vertices.Reverse();


			return shape;
		}

		private static double ccw(DVector2 p1, DVector2 p2, DVector2 p3)
		{
			return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
		}




        public IConvexShape GetUpdatedPoints(double elapsedTime)
        {
            throw new NotImplementedException();
        }
    }

	class SortVector : IComparer<DVector2>
	{

		#region IComparer<DVector2> Member

		public int Compare(DVector2 x, DVector2 y)
		{
			return x.X < y.X || (x.X == y.X && x.Y < y.Y) ? -1 : 1;
		}
		#endregion
	}
}
