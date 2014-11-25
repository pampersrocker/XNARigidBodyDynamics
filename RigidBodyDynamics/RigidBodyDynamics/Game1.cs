using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using RigidBodyDynamics.PhysicElementals;

namespace RigidBodyDynamics
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;
		RectanglePhysicBody body;
		//HexagonalRigidBody body;
		RectanglePhysicBody body2, body3, body4,body5,body6;

		//RenderHexagonBody rbody;
		RenderPhysicBody rbody ;
		RenderPhysicBody rbody2,rbody3, rbody4,rbody5,rbody6;

		RectanglePhysicBody[] bodies;
		RenderPhysicBody[] rbodies;
		
		List<RectanglePhysicBody> sbodies;
		List<RenderPhysicBody> srbodies;

		Texture2D tex;
		BasicEffect bEffect;
		Camera camera;

		Keys pauseKey = Keys.Space;
		Keys stepKey = Keys.S;

		MouseState oldState;
		KeyboardState oldKeyState;
		bool runStepAnimation = true;
		bool runStep = false;
		int amountBoxLines      = 7;
		int amountBoxesColumns  = 7;
		int boxWidth = 50;
		int boxRandom = 90;
		double boxWeight = 2.0;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			//this.IsFixedTimeStep = false;
			//graphics.SynchronizeWithVerticalRetrace = false;
			graphics.PreferredBackBufferHeight = 600;
			graphics.PreferredBackBufferWidth = 800;
			camera = new Camera(800, 600,new Vector2(400,300));
			camera.Scale = 0.5f;
			this.IsMouseVisible = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
			bEffect = new BasicEffect(GraphicsDevice);
			bEffect.VertexColorEnabled = true;
			bEffect.Projection = Matrix.CreateOrthographicOffCenter
			   (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
				graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
				0, 1);
			bEffect.TextureEnabled = false;

			oldState = Mouse.GetState();
		}

		

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		/// 
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			PhysicElementals.PhysicWorldParameters parameters = new PhysicElementals.PhysicWorldParameters();
			parameters.Gravity = 981f;
			//parameters.Gravity = 0;
			//body = new RectanglePhysicBody(new DVector2(550, 150),25 , 100,100, parameters);
			
			
			body = new RectanglePhysicBody(new DVector2(550, 150), 2f, 100, 50, parameters);
			//body = new HexagonalRigidBody(new DVector2(550, 150), 100f, 25, parameters);
			body.angularVelocity = 3;
			body2 = new RectanglePhysicBody(new DVector2(225, 600), double.PositiveInfinity, 700, 50, parameters);
			body2.Orientation = 0.1;
			body2.PhyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			body3 = new RectanglePhysicBody(new DVector2(0, 400), double.PositiveInfinity, 50, 300, parameters);
			
			body3.PhyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			body4 = new RectanglePhysicBody(new DVector2(700, 100), double.PositiveInfinity, 50, 300, parameters);
			body4.PhyBodyType = PhysicBody.PhysicBodyType.StaticBody;

			body5 = new RectanglePhysicBody(new DVector2(0, 200), double.PositiveInfinity, 50, 200, parameters);
			body5.orientation = -0.7;
			body5.PhyBodyType = PhysicBody.PhysicBodyType.StaticBody;

			body6 = new RectanglePhysicBody(new DVector2(750, 200), double.PositiveInfinity, 50, 200, parameters);
			body6.orientation = 0.7;
			body6.PhyBodyType = PhysicBody.PhysicBodyType.StaticBody;


			body.Orientation = -0.0f;
			
			rbody = new RenderPhysicBody(body);
			//rbody = new RenderHexagonBody(body);
			//rbody = new RenderPhysicBody(body);
			rbody2 = new RenderPhysicBody(body2);
			tex = Content.Load<Texture2D>("Bitmap1");
			DebugDrawer.Init(tex);
			font = Content.Load<SpriteFont>("font");
			rbody.Initialize(Content.Load<Texture2D>("Bitmap1"), font);
			rbody2.Initialize(Content.Load<Texture2D>("Bitmap1"), font);
			rbody3 = new RenderPhysicBody(body3);
			rbody3.Initialize(tex, font);
			rbody4 = new RenderPhysicBody(body4);
			rbody4.Initialize(tex, font);
			rbody5 = new RenderPhysicBody(body5);
			rbody5.Initialize(tex, font);

			rbody6 = new RenderPhysicBody(body6);
			rbody6.Initialize(tex, font);
			bodies = new RectanglePhysicBody[amountBoxesColumns * amountBoxLines];
			rbodies = new RenderPhysicBody[amountBoxesColumns * amountBoxLines];
			Random rand = new Random((int)DateTime.Now.Ticks);
			for (int i = 0; i < amountBoxesColumns; i++)
			{
				for (int j = 0; j < amountBoxLines; j++)
				{
					double tmpwidth = (boxRandom * (rand.NextDouble() - 0.5)) + boxWidth;
					double tmpheight = (boxRandom * (rand.NextDouble() - 0.5)) + boxWidth;
					double bweighttmp = boxWeight * (tmpheight*tmpwidth/(boxWidth*boxWidth));
					bweighttmp = MathHelper.Clamp((float)bweighttmp, 1, 9000000f);

					bodies[i * amountBoxLines + j] = new RectanglePhysicBody(new DVector2(1000 + i * 1.1 * boxWidth,  j * 1.1 * boxWidth), bweighttmp,(int)tmpwidth,(int)( tmpheight), parameters);
					bodies[i * amountBoxLines + j].Orientation = 0f;
					rbodies[i * amountBoxLines + j] = new RenderPhysicBody(bodies[i * amountBoxLines + j]);
					rbodies[i * amountBoxLines + j].Initialize(tex, font);
					//bodies[i * 10 + j].angularVelocity = 2;
				}
			}

			sbodies = new List<RectanglePhysicBody>();
			srbodies = new List<RenderPhysicBody>();

			RectanglePhysicBody tmp;
			RenderPhysicBody rtmp;

			tmp = new RectanglePhysicBody(new DVector2(300, 1700), double.PositiveInfinity, 500, 100, parameters);
			tmp.Orientation = 0.6f;
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);

			//ground
			tmp = new RectanglePhysicBody(new DVector2(1000,2000),double.PositiveInfinity , 4000, 500, parameters);
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);


			//left
			tmp = new RectanglePhysicBody(new DVector2(-1000, 0), double.PositiveInfinity, 4000, 500, parameters);
			tmp.Orientation = Math.PI / 2;
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);

			//right
			tmp = new RectanglePhysicBody(new DVector2(2000, 0), double.PositiveInfinity, 4000, 500, parameters);
			tmp.Orientation = Math.PI / 2;
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);

			tmp = new RectanglePhysicBody(new DVector2(1000, 1000), double.PositiveInfinity, 1000, 100, parameters);
			tmp.Orientation = 0.6f;
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);

			tmp = new RectanglePhysicBody(new DVector2(1500, 1525), double.PositiveInfinity, 1000, 100, parameters);
			tmp.Orientation = -0.6f;
			tmp.phyBodyType = PhysicBody.PhysicBodyType.StaticBody;
			rtmp = new RenderPhysicBody(tmp);
			rtmp.Initialize(tex, font);
			sbodies.Add(tmp);
			srbodies.Add(rtmp);

			oldKeyState = Keyboard.GetState();
			//body.angularVelocity = 10;
			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			//gameTime = new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, 0,1));
			MouseState state;
			state = Mouse.GetState();
			KeyboardState keyState= Keyboard.GetState();
			if (state.RightButton == ButtonState.Pressed)
			{
				Vector2 delta = new Vector2(oldState.X - state.X, oldState.Y - state.Y)/0.5f;
				camera.update(gameTime, delta, delta, true);
			}

			if (keyState.IsKeyDown(pauseKey) && !oldKeyState.IsKeyDown(pauseKey))
			{
				runStepAnimation = !runStepAnimation;
			}
			runStep = !oldKeyState.IsKeyDown(stepKey) && keyState.IsKeyDown(stepKey);

			
			CollisionDetection.CollisionResponse response;
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();

			oldState = state;
			oldKeyState = keyState;
			
			if (!runStepAnimation && !runStep)
			{
				return;
			}

			if (!(!runStepAnimation && !runStep))
			{
				DebugDrawer.Clear();
			}

			body.Update(gameTime.ElapsedGameTime.TotalSeconds);

			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				body.ApplyForce(new DVector2(-2000 * body.mass, 0));
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				body.ApplyForce(new DVector2(2000 * body.mass, 0));
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				body.ApplyForce(new DVector2(0, -2000 * body.mass));
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				body.ApplyForce(new DVector2(0, 2000 * body.mass));
			}
			//body.Orientation += (float)gameTime.ElapsedGameTime.TotalSeconds;
			//body2.Orientation -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			//body3.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			double elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
			double lastCollisionTime = elapsedTime;

			response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body.GetUpdatedPoints(elapsedTime), body2.GetUpdatedPoints(elapsedTime));
			body.ApplyImpulse(response, body2, elapsedTime);
			response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body.GetUpdatedPoints(elapsedTime), body3.GetUpdatedPoints(elapsedTime));

			body.ApplyImpulse(response, body3, elapsedTime);
			response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body.GetUpdatedPoints(elapsedTime), body4.GetUpdatedPoints(elapsedTime));

			body.ApplyImpulse(response, body4, elapsedTime);

			response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body.GetUpdatedPoints(elapsedTime), body5.GetUpdatedPoints(elapsedTime));

			body.ApplyImpulse(response, body5, elapsedTime);

			response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body.GetUpdatedPoints(elapsedTime), body6.GetUpdatedPoints(elapsedTime));

			body.ApplyImpulse(response, body6, elapsedTime);

			for (int i = 0; i < sbodies.Count; i++)
			{
				response = CollisionDetection.CollisionDetection.CheckForConvexCollision(body, sbodies[i]);
				body.ApplyImpulse(response, sbodies[i], elapsedTime);
			}


			//body2.Update((float)gameTime.ElapsedGameTime.TotalSeconds);




			for (int i = 0; i < amountBoxesColumns * amountBoxLines; i++)
			{

				bodies[i].Update(gameTime.ElapsedGameTime.TotalSeconds);


				//if (bodies[i].gPosition.Y < 0)
				//{
				//	bodies[i].gPosition.Y = 600;
				//}
				//if (bodies[i].gPosition.Y > 600)
				//{
				//	bodies[i].gPosition.Y = 0;
				//}
				//if (bodies[i].gPosition.X < 0)
				//{
				//	bodies[i].gPosition.X = 800;
				//}
				//if (bodies[i].gPosition.X > 800)
				//{
				//	bodies[i].gPosition.X = 0;
				//}


				CollisionDetection.CollisionResponse res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body, elapsedTime);
				res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body2.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body2, elapsedTime);
				res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body3.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body3, elapsedTime);
				res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body4.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body4, elapsedTime);
				res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body5.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body5, elapsedTime);
				res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), body6.GetUpdatedPoints(elapsedTime));
				bodies[i].ApplyImpulse(res, body6, elapsedTime);
				for (int j = i + 1; j < amountBoxesColumns * amountBoxLines; j++)
				{
					res = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), bodies[j].GetUpdatedPoints(elapsedTime));
					bodies[i].ApplyImpulse(res, bodies[j], elapsedTime);
				}

				for (int k = 0; k < sbodies.Count; k++)
				{
					response = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[i].GetUpdatedPoints(elapsedTime), sbodies[k]);
					bodies[i].ApplyImpulse(response, sbodies[k], elapsedTime);
				}


			}


			// TODO: Add your update logic here
			//if (body.gPosition.Y < 0)
			//{
			//	body.gPosition.Y = 600;
			//}
			//if (body.gPosition.Y > 600)
			//{
			//	body.gPosition.Y = 0;
			//}
			//if (body.gPosition.X < 0)
			//{
			//	body.gPosition.X = 800;
			//}
			//if (body.gPosition.X > 800)
			//{
			//	body.gPosition.X = 0;
			//}
			//
			//if (body2.gPosition.Y < 0)
			//{
			//	body2.gPosition.Y = 600;
			//}
			//if (body2.gPosition.Y > 600)
			//{
			//	body2.gPosition.Y = 0;
			//}
			//if (body2.gPosition.X < 0)
			//{
			//	body2.gPosition.X = 800;
			//}
			//if (body2.gPosition.X > 800)
			//{
			//	body2.gPosition.X = 0;
			//}
			//} 

			

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//CollisionDetection.CollisionResponse response = CollisionDetection.CollisionDetection.CheckForConvexCollision(bodies[99], bodies[98]);

			//DVector2 body1Velocity = body.velocity + new DVector2(-response.CollisionPointRelativeToBody1.Y, response.CollisionPointRelativeToBody1.X) * body.angularVelocity;
			//DVector2 body2Velocity = body2.velocity + new DVector2(-response.CollisionPointRelativeToBody2.Y, response.CollisionPointRelativeToBody2.X) * body2.angularVelocity;
			//if (DVector2.Dot(body.gPosition - body2.gPosition, body2Velocity - body1Velocity) >= 0f)
			//{
			////	rbody.Color = Color.Red;
			////	rbody2.Color = Color.Red;

			//}
			//else
			//{
			//    rbody.Color = Color.White;
			//    rbody2.Color = Color.White;
			//}


			rbody.Draw(spriteBatch,camera.getMatrix);
			rbody2.Draw(spriteBatch, camera.getMatrix);
			rbody3.Draw(spriteBatch, camera.getMatrix);
			rbody4.Draw(spriteBatch, camera.getMatrix);
			rbody5.Draw(spriteBatch, camera.getMatrix);
			rbody6.Draw(spriteBatch, camera.getMatrix);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.getMatrix);
			
			
			for (int i = 0; i < amountBoxLines * amountBoxesColumns; i++)
			{
				rbodies[i].DrawSingle(spriteBatch);//fix
			}
			for (int i = 0; i < srbodies.Count; i++)
			{
				srbodies[i].DrawSingle(spriteBatch);
			}
			spriteBatch.End();
			//List<DVector2> points = new List<DVector2>();
			//points.AddRange(body.GetVertices);
			//points.AddRange(body2.GetVertices);
			//IConvexShape vertices = ConvexShape.getConvexShape(points);
			//VertexPositionColor[] posVertCol = new VertexPositionColor[vertices.GetVertices.Count+1];
			//for (int i = 0; i < vertices.GetVertices.Count; i++)
			//{
			//    posVertCol[i] = new VertexPositionColor(new Vector3(vertices.GetVertices[i], 0), Color.Green);
			//}
			//posVertCol[posVertCol.Length - 1] = new VertexPositionColor(new Vector3(vertices.GetVertices[0], 0), Color.Green);
			//bEffect.CurrentTechnique.Passes[0].Apply();
			//GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, posVertCol, 0, vertices.GetVertices.Count );

#if DEBUG
			CollisionDetection.CollisionDetection.DebugDraw(spriteBatch, tex);

#endif
			//if (response.Colliding)
			//{
			//    spriteBatch.Begin();
			//    spriteBatch.Draw(tex, new Rectangle((int)bodies[99].CentralPosition.X + (int)response.CollisionPointRelativeToBody1.X - 5, (int)bodies[99].CentralPosition.Y + (int)response.CollisionPointRelativeToBody1.Y - 5, 10, 10), Color.Blue);
			//    spriteBatch.Draw(tex, new Rectangle((int)bodies[98].CentralPosition.X + (int)response.CollisionPointRelativeToBody2.X - 5, (int)bodies[98].CentralPosition.Y + (int)response.CollisionPointRelativeToBody2.Y - 5, 10, 10), Color.Orange);
			//    spriteBatch.Draw(tex, new Rectangle((int)(response.CollisionVector.X * 100) + (int)bodies[99].CentralPosition.X + (int)response.CollisionPointRelativeToBody1.X - 5, (int)(response.CollisionVector.Y * 100) + (int)bodies[99].CentralPosition.Y + (int)response.CollisionPointRelativeToBody1.Y - 5, 10, 10), Color.Black);
			//    spriteBatch.DrawString(font, DVector2.Dot(body.gPosition - body2.gPosition, body2Velocity - body1Velocity).ToString(), new DVector2(), Color.Black);
			//    spriteBatch.End(); 
			//}

			spriteBatch.Begin();
			spriteBatch.DrawString(font, gameTime.ElapsedGameTime.TotalMilliseconds.ToString("0.00") + "ms \n" +
				(1.0 / gameTime.ElapsedGameTime.TotalSeconds).ToString("0") + " FPS",
				new Vector2(10, 10),
				Color.Red);
			spriteBatch.End();

			//spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, bEffect);
			//DebugDrawer.Draw(spriteBatch, camera.getMatrix);
			//spriteBatch.End();

			

			base.Draw(gameTime);
		}
	}
}
