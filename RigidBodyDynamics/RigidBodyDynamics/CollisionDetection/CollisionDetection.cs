using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RigidBodyDynamics.PhysicElementals;
using Microsoft.Xna.Framework.Graphics;

namespace RigidBodyDynamics.CollisionDetection
{
	public static class CollisionDetection
	{
		static DVector2[] rectanglePoints1 = new DVector2[4], rectanglePoints2 = new DVector2[4];
		public static double intersectionEpsilon = 0.01f;

		public static CollisionResponse CheckForRectangleCollision(IRectangularBody rectangle1, IRectangularBody rectangle2)
		{
			CollisionResponse response;
			response = SingleRectangleCollision(rectangle1, rectangle2);
			if (response.Colliding)
			{
				response.CollisionPointRelativeToBody2 = rectangle1.CentralPosition - rectangle2.CentralPosition + response.CollisionPointRelativeToBody1;
				return response;
			}
			else
			{
				response = SingleRectangleCollision(rectangle2, rectangle1);
				if (response.Colliding)
				{
					response.CollisionPointRelativeToBody2 = response.CollisionPointRelativeToBody1;
					response.CollisionPointRelativeToBody1 += rectangle2.CentralPosition - rectangle1.CentralPosition;
				}
			}
			return response;

		}

		public static CollisionResponse CheckForConvexCollision<T>(T convexShape1, T convexShape2)
			where T : IConvexShape
		{
			CollisionResponse response;

			response = CheckForSingleConvexCollision(convexShape1, convexShape2);
			if (response.Colliding)
			{
				response.CollisionPointRelativeToBody2 = convexShape1.CentralPosition - convexShape2.CentralPosition + response.CollisionPointRelativeToBody1;
				return response;
			}

			return response;
		}

		public static CollisionType IsThereACollision<T>(T convexShape1, T convexShape2)
			where T : IConvexShape
		{
			CollisionType type = CollisionType.None;
			//TODO Step collision
			if ((convexShape1.CentralPosition - convexShape2.CentralPosition).LengthSquared() < convexShape1.Radius * convexShape1.Radius + convexShape2.Radius * convexShape2.Radius)
			{
				DVector2 relativeCenterVector = convexShape2.CentralPosition - convexShape1.CentralPosition;
				int incI;
				int incJ;
				int convexShape1Count = convexShape1.GetVertices.Count;
				int convexShape2Count = convexShape2.GetVertices.Count;
				List<DVector2> convexShape1Vertices = convexShape1.GetVertices;
				List<DVector2> convexShape2Vertices = convexShape2.GetVertices;

				for (int i = 0; i < convexShape1Count; i++)
				{

					incI = (i + 1) % convexShape1Count;
					for (int j = 0; j < convexShape2Count; j++)
					{
						incJ = (j + 1) % convexShape2Count;
						//Calculate the Equation for the intersection.
						double d =
						   (convexShape2Vertices[incJ].Y - convexShape2Vertices[j].Y) * (convexShape1Vertices[incI].X - convexShape1Vertices[i].X) -
						   (convexShape2Vertices[incJ].X - convexShape2Vertices[j].X) * (convexShape1Vertices[incI].Y - convexShape1Vertices[i].Y);


						double nominatorA =
						   (convexShape2Vertices[incJ].X - convexShape2Vertices[j].X) * (convexShape1Vertices[i].Y - convexShape2Vertices[j].Y) -
						   (convexShape2Vertices[incJ].Y - convexShape2Vertices[j].Y) * (convexShape1Vertices[i].X - convexShape2Vertices[j].X);

						double nominatorB =
						   (convexShape1Vertices[incI].X - convexShape1Vertices[i].X) * (convexShape1Vertices[i].Y - convexShape2Vertices[j].Y) -
						   (convexShape1Vertices[incI].Y - convexShape1Vertices[i].Y) * (convexShape1Vertices[i].X - convexShape2Vertices[j].X);

						if (d != 0)
						{
							//Do the division.
							double ua = nominatorA / d;
							double ub = nominatorB / d;

							//Check if the Intersecting point is on the line.
							if (ua >= -intersectionEpsilon && ua <= 1.0 + intersectionEpsilon && ub >= -intersectionEpsilon && ub <= 1.0 + intersectionEpsilon)
							{
								if (ua > 1f - intersectionEpsilon || ua < intersectionEpsilon || ub > 1f - intersectionEpsilon || ub < intersectionEpsilon)
								{
									type = CollisionType.Intersecting;
								}
								else if (type == CollisionType.None)
								{
									type = CollisionType.Penetrating;
								}
							}


						}
					}
				}
			}
			return type;
		}

		private static CollisionResponse CheckForSingleConvexCollision<T>(T convexShape1, T convexShape2)
			where T : IConvexShape
		{
			CollisionResponse response;
			response = new CollisionResponse();
			if ((convexShape1.CentralPosition - convexShape2.CentralPosition).LengthSquared() < convexShape1.Radius * convexShape1.Radius + convexShape2.Radius * convexShape2.Radius)
			{
				DVector2 relativeCenterVector = convexShape2.CentralPosition - convexShape1.CentralPosition;
				int incI;
				int incJ;
				List<DVector2> collidingPoint = new List<DVector2>();
				int convexShape1Count = convexShape1.GetVertices.Count;
				int convexShape2Count = convexShape2.GetVertices.Count;
				List<DVector2> convexShape1Vertices = convexShape1.GetVertices;
				List<DVector2> convexShape2Vertices = convexShape2.GetVertices;
				DVector2 minCollisionPoint = new DVector2();
				DVector2 maxCollisionPoint = new DVector2();
				double sumLength = 0;
				for (int i = 0; i < convexShape1Count; i++)
				{

					incI = (i + 1) % convexShape1Count;
					for (int j = 0; j < convexShape2Count; j++)
					{
						incJ = (j + 1) % convexShape2Count;
						//Calculate the Equation for the intersection.
						double d =
						   (convexShape2Vertices[incJ].Y - convexShape2Vertices[j].Y) * (convexShape1Vertices[incI].X - convexShape1Vertices[i].X) -
						   (convexShape2Vertices[incJ].X - convexShape2Vertices[j].X) * (convexShape1Vertices[incI].Y - convexShape1Vertices[i].Y);


						double nominatorA =
						   (convexShape2Vertices[incJ].X - convexShape2Vertices[j].X) * (convexShape1Vertices[i].Y - convexShape2Vertices[j].Y) -
						   (convexShape2Vertices[incJ].Y - convexShape2Vertices[j].Y) * (convexShape1Vertices[i].X - convexShape2Vertices[j].X);

						double nominatorB =
						   (convexShape1Vertices[incI].X - convexShape1Vertices[i].X) * (convexShape1Vertices[i].Y - convexShape2Vertices[j].Y) -
						   (convexShape1Vertices[incI].Y - convexShape1Vertices[i].Y) * (convexShape1Vertices[i].X - convexShape2Vertices[j].X);

						if (d >= 0.1f || d <= -0.1f)
						{
							//Do the division.
							double ua = nominatorA / d;
							double ub = nominatorB / d;

							//Check if the Intersecting point is on the line.
							if (ua >= -0.1f && ua <= 1.1f && ub >= -0.1f && ub <= 1.1f)
							{
								DVector2 collisionLine = (convexShape1Vertices[incI] - convexShape1Vertices[i]);
								DVector2 tmp = convexShape1Vertices[i] + (ua * (collisionLine)) - convexShape1.CentralPosition;
								collidingPoint.Add(tmp);
								sumLength += collisionLine.LengthSquared();
								response.Colliding = true;
							}
						}
					}
				}


				if (collidingPoint.Count > 0)
				{
					response.CollisionPointRelativeToBody1 = DVector2.Zero;
					//Get the intermediate point if  there's more than one Intersection.
					for (int i = 0; i < collidingPoint.Count; i++)
					{
						response.CollisionPointRelativeToBody1 += collidingPoint[i];
					}
					response.CollisionPointRelativeToBody1 /= collidingPoint.Count;
					//response.CollisionPointRelativeToBody1 /= sumLength;

					double distance = double.PositiveInfinity;
					double tempDist = 0;
					for (int i = 0; i < convexShape1Count; ++i )
					{
						incI = (i + 1) % convexShape1Count;
						DVector2 normal = convexShape1Vertices[incI] - convexShape1Vertices[i];
						DVector2 r = response.CollisionPointRelativeToBody1;
						normal = new DVector2(-normal.Y, normal.X);
						tempDist = normal.X * r.X + normal.Y * r.Y;
						if ( tempDist < distance )
						{
							response.CollisionVector = -normal;
							distance = tempDist;
						}
					}

					for (int i = 0; i < convexShape2Count; ++i)
					{
						incI = (i + 1) % convexShape2Count;
						DVector2 normal = convexShape2Vertices[incI] - convexShape2Vertices[i];
						DVector2 r = response.CollisionPointRelativeToBody2;
						normal = new DVector2(-normal.Y, normal.X);
						tempDist = normal.X * r.X + normal.Y * r.Y;
						if (tempDist < distance)
						{
							response.CollisionVector = normal;
							distance = tempDist;
						}
					}
					
					if (collidingPoint.Count == 2)
					{
						response.CollisionType = CollisionType.Intersecting;
					}
					else
					{
						response.CollisionType = CollisionType.Penetrating;
					}
					response.Colliding = true;
				}
				else
				{
					response.Colliding = false;
				}
				//response.CollisionVector = -Vector2.UnitX;

				DebugLine line = new DebugLine(
									   response.CollisionPointRelativeToBody1 + convexShape1.CentralPosition,
										response.CollisionPointRelativeToBody1 + convexShape1.CentralPosition + response.CollisionVector,
									   Color.Red);
				DebugDrawer.AddLine(line);
				line = new DebugLine(
									   response.CollisionPointRelativeToBody1 + convexShape1.CentralPosition,
										response.CollisionPointRelativeToBody1 + convexShape1.CentralPosition - response.CollisionVector,
									   Color.Black);
				DebugDrawer.AddLine(line);
				return response;
			}
			else
			{
				response.Colliding = false;
				return response;
			}
		}

		private static CollisionResponse SingleRectangleCollision(IRectangularBody rectangle1, IRectangularBody rectangle2)
		{
			CollisionResponse response;
			response = new CollisionResponse();
			if ((rectangle1.CentralPosition - rectangle2.CentralPosition).LengthSquared() < rectangle1.Radius * rectangle1.Radius + rectangle2.Radius * rectangle2.Radius)
			{
				rectanglePoints1[0] = new DVector2(
					0,
					0);
				rectanglePoints1[1] = new DVector2(
					rectangle1.Width,
					0);
				rectanglePoints1[2] = new DVector2(
					rectangle1.Width,
					rectangle1.Height);
				rectanglePoints1[3] = new DVector2(
					0,
					rectangle1.Height);
				DVector2 translation = new DVector2(-rectangle1.Width / 2, -rectangle1.Height / 2);
				rectanglePoints2[0] = new DVector2(
					-rectangle2.Width / 2,
					-rectangle2.Height / 2);
				rectanglePoints2[1] = new DVector2(
				rectangle2.Width / 2,
					-rectangle2.Height / 2);
				rectanglePoints2[2] = new DVector2(
					-rectangle2.Width / 2,
					+rectangle2.Height / 2);
				rectanglePoints2[3] = new DVector2(
					+rectangle2.Width / 2,
					+rectangle2.Height / 2);
				DVector2 relativeCenterVector = rectangle2.CentralPosition - rectangle1.CentralPosition;

				int incI;
				int incJ;
				List<DVector2> collidingPoint = new List<DVector2>();
				for (int i = 0; i < 4; i++)
				{
					rectanglePoints2[i] = rectanglePoints2[i].Rotate(rectangle2.Orientation);
					rectanglePoints2[i] = (rectanglePoints2[i] + relativeCenterVector).Rotate(-rectangle1.Orientation) - translation;
				}
				for (int i = 0; i < 4; i++)
				{

					incI = (i + 1) % 4;
					for (int j = 0; j < 4; j++)
					{
						incJ = (j + 1) % 4;
						double d =
						   (rectanglePoints2[incJ].Y - rectanglePoints2[j].Y) * (rectanglePoints1[incI].X - rectanglePoints1[i].X) -
						   (rectanglePoints2[incJ].X - rectanglePoints2[j].X) * (rectanglePoints1[incI].Y - rectanglePoints1[i].Y);


						double n_a =
						   (rectanglePoints2[incJ].X - rectanglePoints2[j].X) * (rectanglePoints1[i].Y - rectanglePoints2[j].Y) -
						   (rectanglePoints2[incJ].Y - rectanglePoints2[j].Y) * (rectanglePoints1[i].X - rectanglePoints2[j].X);

						double n_b =
						   (rectanglePoints1[incI].X - rectanglePoints1[i].X) * (rectanglePoints1[i].Y - rectanglePoints2[j].Y) -
						   (rectanglePoints1[incI].Y - rectanglePoints1[i].Y) * (rectanglePoints1[i].X - rectanglePoints2[j].X);
						response.Colliding = true;
						if (d == 0)
							response.Colliding = false;

						if (response.Colliding)
						{
							// Calculate the intermediate fractional point that the lines potentially intersect.
							double ua = n_a / d;
							double ub = n_b / d;

							// The fractional point will be between 0 and 1 inclusive if the lines
							// intersect.  If the fractional calculation is larger than 1 or smaller
							// than 0 the lines would need to be longer to intersect.
							if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
							{
								collidingPoint.Add((rectanglePoints1[i] + (ua * (rectanglePoints1[incI] - rectanglePoints1[i])) + translation).Rotate(rectangle1.Orientation));

								//response.CollisionPointRelativeToBody1 = (rectanglePoints2[i] + translation).Rotate(rectangle1.Orientation);

								response.Colliding = true;
							}


						}
					}
				}
				if (collidingPoint.Count > 0)
				{
					response.CollisionPointRelativeToBody1 = DVector2.Zero;
					for (int i = 0; i < collidingPoint.Count; i++)
					{
						response.CollisionPointRelativeToBody1 += collidingPoint[i];
					}
					response.CollisionPointRelativeToBody1 /= collidingPoint.Count;
					if (collidingPoint.Count == 1)
					{
						response.CollisionType = CollisionType.Intersecting;
					}
					else
					{
						response.CollisionType = CollisionType.Penetrating;
					}
				}
				else
				{
					response.Colliding = false;
				}
				return response;
			}
			else
			{
				response.Colliding = false;
				return response;
			}
		}

#if DEBUG
		public static void DebugDraw(SpriteBatch spriteBatch,Texture2D tex)
		{
			spriteBatch.Begin();
			for (int i = 0; i < 4; i++)
			{
				spriteBatch.Draw(tex, new Rectangle((int)rectanglePoints1[i].X + 400 - 5,(int) rectanglePoints1[i].Y + 300 - 5, 10, 10), Color.Red);
				spriteBatch.Draw(tex, new Rectangle((int)rectanglePoints2[i].X + 400 - 5, (int)rectanglePoints2[i].Y + 300 - 5, 10, 10), Color.Green);
		
			}
			spriteBatch.End();
		}
#endif
	}
}
