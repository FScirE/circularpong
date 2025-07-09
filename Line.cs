using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace circularpong
{
    internal class Line
    {
        public Vector2 a, b;
        private float width;
        private Color color;

        public Line(Vector2 a, Vector2 b, float width, Color color)
        {
            this.a = a;
            this.b = b;
            this.width = width;
            this.color = color;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 diff = b - a;
            Vector2 mid = (a + b) / 2;
            float length = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
            float angle = (float)Math.Atan2(diff.Y, diff.X);

            sb.Draw(Game1._pixelTexture, mid, null, color, angle, new Vector2(0.5f, 0.5f), new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
