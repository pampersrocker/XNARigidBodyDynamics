using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RigidBodyDynamics
{
    public static class DebugDrawer
    {
        private static List<DebugLine> lines = new List<DebugLine>();
       private static Texture2D tex;


        public static void Clear()
        {
            lines.Clear();
        }

        public static void Init(Texture2D tex2d)
        {
            tex = tex2d;
        }

        public static void AddLine(DebugLine line)
        {
            lines.Add(line);
        }

        public static void Draw(SpriteBatch spriteBatch, Matrix transformation)
        {
            if (lines.Count != 0)
            {
                Rectangle rect = new Rectangle();
                rect.Height = 2;
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transformation);

                foreach (DebugLine dbgLine in lines)
                {
                    rect.X = (int)dbgLine.Start.X;
                    rect.Y = (int)dbgLine.Start.Y-1;
                    float rotation = (float)(dbgLine.Finish - dbgLine.Start).GetRotation();
                    rect.Width = (int)((dbgLine.Finish - dbgLine.Start).Length());
                    rect.Width = 20;
                    spriteBatch.Draw(tex, rect, null, dbgLine.Color, rotation, Vector2.Zero, SpriteEffects.None, 0);
                }

                spriteBatch.End();
            }
        }
    }
}