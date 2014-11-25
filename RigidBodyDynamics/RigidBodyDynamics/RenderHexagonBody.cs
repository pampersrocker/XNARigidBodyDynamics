using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RigidBodyDynamics.PhysicElementals;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RigidBodyDynamics
{
	class RenderHexagonBody
	{
		HexagonalRigidBody phyBody;
		Texture2D tex;
		Color color = Color.White;
		Color fontColor = Color.Black;
		SpriteFont font;

		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		public RenderHexagonBody(HexagonalRigidBody phyBody)
		{
			this.phyBody = phyBody;
		}

		public void Initialize(Texture2D tex,SpriteFont font)
		{
			this.tex = tex;
			this.font = font;
            if (phyBody.phyBodyType == PhysicBody.PhysicBodyType.StaticBody)
            {
                Color = Color.Green;
            }
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null);
			DrawSingle(spriteBatch);
			spriteBatch.End();
		}

		public void DrawSingle(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				tex,
				new Microsoft.Xna.Framework.Rectangle(
					(int)(phyBody.gPosition.X),
					(int)(phyBody.gPosition.Y),
					(int)phyBody.Radius*2,
					(int)phyBody.Radius*2),
					null, color, (float)phyBody.Orientation, new Vector2((float)tex.Width / 2f, (float)tex.Height / 2f), SpriteEffects.None, 0);
			spriteBatch.Draw(tex, new Rectangle((int)phyBody.gPosition.X - 5, (int)phyBody.gPosition.Y - 5, 10, 10), Color.Yellow);
			//spriteBatch.DrawString(font, phyBody.velocity.Y.ToString(), phyBody.gPosition, fontColor,0,Vector2.Zero,0.5f, SpriteEffects.None, 0);
            //List<DVector2> vertices = phyBody.GetVertices;
            //for (int i = 0; i < vertices.Count; i++)
            //{
            //    spriteBatch.Draw(tex, new Rectangle((int)vertices[i].X-1, (int)vertices[i].Y-1, 2, 2), Color.Green);
            //}
		}
	}
}
