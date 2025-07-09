using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace circularpong
{
    public class Circle
    {
        public Vector2 pos { get; private set; }
        public float radius { get; private set; }
        public float thickness { get; private set; }
        public float startAngle { get; private set; }
        public float endAngle { get; private set; }
        public int points { get; private set; }
        public bool radial { get; private set; }
        public Color color { get; private set; }

        List<Line> lines;

        public Circle(Vector2 pos, float radius, float thickness, Color color, int points, float startAngle = 0, float endAngle = MathHelper.Pi * 2, bool radial = false) 
        {
            Modify(pos, radius, thickness, color, points, startAngle, endAngle, radial);
        }

        private void SetLines()
        {
            float step = (endAngle - startAngle) / points;
            lines = new List<Line>();
            for (int i = 0; i < points; i++)
            {
                float angleA = startAngle + step * i;
                float angleB = angleA + step;
                lines.Add(new Line(
                    pos + (!radial ? new Vector2(radius * (float)Math.Cos(angleA), radius * (float)Math.Sin(angleA)) : Vector2.Zero),
                    pos + new Vector2(radius * (float)Math.Cos(angleB), radius * (float)Math.Sin(angleB)),
                    thickness,
                    color
                ));
            }
        }

        public void Modify(Vector2 pos, float radius, float thickness, Color color, int points, float startAngle, float endAngle, bool radial)
        {
            this.pos = pos;
            this.radius = radius;
            this.thickness = thickness;
            this.color = color;
            this.points = points;
            this.startAngle = startAngle;
            this.endAngle = endAngle;
            this.radial = radial;
            SetLines();
        }

        public void SetCircleSector(float spanAngle, float directionAngle)
        {
            startAngle = directionAngle - spanAngle / 2;
            endAngle = directionAngle + spanAngle / 2;
            Modify(pos, radius, thickness, color, points, startAngle, endAngle, radial);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Line line in lines)
                line.Draw(sb);           
        }
    }
}
