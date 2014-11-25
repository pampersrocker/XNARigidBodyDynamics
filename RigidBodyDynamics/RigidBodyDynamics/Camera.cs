using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RigidBodyDynamics
{
    public class Camera
    {
        float scale = 1f;
		float rotation = 0;

		public float Rotation
		{
			get { return rotation; }
			set { rotation = value; }
		}
        float offsetmulti = 0.2f;
        int screen_width;
        int screen_height;
        Vector2 offset = new Vector2();
        Matrix matrix;
        Vector2 camoffset = new Vector2();
        Vector2 camdifference = new Vector2();
        private Vector2 position = new Vector2();

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Vector2 Offset
        {
            get { return offset; }
			set { offset = value; }
        }

        #region "Constructors"

        public Camera(int screenwidth, int screenheight)
        {
            screen_width = screenwidth;
            screen_height = screenheight;
            offset = new Vector2(0, offsetmulti * screen_height); 
            CreateMatrix();
        }
        public Camera(int screenwidth, int screenheight, Vector2 startpos)
        {
            screen_width = screenwidth;
            screen_height = screenheight;
            position = new Vector2(-startpos.X, -startpos.Y);

            offset = new Vector2(0, offsetmulti * screen_height); 
            CreateMatrix();
        }
        public Camera(int screenwidth,int screenheight,Vector2 startpos,float new_scale)
        {
            screen_width = screenwidth;
            screen_height = screenheight;
            scale = new_scale;
            position = new Vector2(- startpos.X , - startpos.Y);

            offset = new Vector2(0, (offsetmulti * screen_height)/scale); 
            CreateMatrix();
        }
        public Camera(int screenwidth, int screenheight, Vector2 startpos, float new_scale, float new_rotation)
        {
            screen_width = screenwidth;
            screen_height = screenheight;
            scale = new_scale;
            rotation = new_rotation;
            position = new Vector2(-startpos.X, -startpos.Y);

            offset = new Vector2(0, (offsetmulti * screen_height)/scale); 
            CreateMatrix();
        }
        #endregion

		private void CreateMatrix()
		{
			matrix =
				Matrix.CreateTranslation(new Vector3(offset, 0)) *
				Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
				Matrix.CreateRotationZ(rotation) *
				Matrix.CreateScale(new Vector3(scale, scale, 0)) *
				Matrix.CreateTranslation(new Vector3(screen_width / 2, screen_height / 2, 0));
		}
        
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 CamOffset
        {
            get { return camoffset; }
        }

        public Vector2 CamDifference
        {
            get { return camdifference; }
        }

        public void update(GameTime gametime, Vector2 new_point,Vector2 player_pos,bool direct)
        {
			if (direct)
			{
				position -= new_point;
				camoffset = new_point;
				camdifference = new_point;
			}
			else
			{
				indirectmoving(gametime, player_pos);
			}
        }

        private void indirectmoving(GameTime gameTime,Vector2 player_pos)
        {
            float xspace = (((screen_width / 5f) /100)*50)/scale;
            float yspace = (((screen_height / 5f) / 100) * 50)/scale;
            Vector2 cam_pos = -position;
            Vector2 offset =(cam_pos-player_pos);
            offset.Y *= 2;
            if (offset.X > -xspace && offset.X < xspace)
            {
                offset.X = 0;
            }
            else
            {
                if (offset.X > 0)
                {
                    offset.X -= xspace;
                }
                else
                {
                    offset.X += xspace;
                }
            }
            if (offset.Y > -yspace && offset.Y < yspace)
            {
                offset.Y = 0;
            }
            else
            {
                if (offset.Y > 0)
                {
                    offset.Y -= yspace;
                }
                else
                {
                    offset.Y += yspace;
                }
            }
            camoffset = -offset;
            offset *= (float)gameTime.ElapsedGameTime.TotalSeconds * 8;
            //camoffset = -offset;//(float)gameTime.ElapsedGameTime.TotalSeconds;
            camdifference = -offset;
            position += offset;
			//rotation += (float)(gameTime.ElapsedGameTime.TotalSeconds * Math.PI)/2 + rotation * (float)gameTime.ElapsedGameTime.TotalSeconds;
			//scale -= scale* (float)(gameTime.ElapsedGameTime.TotalSeconds)/10;
        }

        public Matrix getMatrix
        {
            get 
            {
                matrix =
                    Matrix.CreateTranslation(new Vector3(offset,0)) *
                    Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0)) *
                    Matrix.CreateRotationZ(rotation) *
                    Matrix.CreateScale(new Vector3(scale, scale, 0)) *
                    Matrix.CreateTranslation(new Vector3(screen_width / 2, screen_height / 2, 0));
                return matrix; 
            }
        }

        public Matrix getScaleMatrix
        {
            get
            { return Matrix.CreateScale(scale); }
        }
    }
}
