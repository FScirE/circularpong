using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace circularpong
{
    internal class Player
    {
        float angle;
        float length;
        float thickness;
        float speed;
        Color color;
        Keys clockwise;
        Keys counterClockwise;

        public Circle circle {  get; private set; }

        public Player(float angle, float length, float thickness, float speed, Color color, Keys clockwise, Keys counterClockwise)
        {
            this.angle = angle;
            this.length = length;
            this.thickness = thickness;
            this.speed = speed;
            this.color = color;
            this.clockwise = clockwise;
            this.counterClockwise = counterClockwise;

            circle = new Circle(Game1._screenCenter, Game1._circle.radius + this.thickness / 4, this.thickness, this.color, 50);
            circle.SetCircleSector(this.length, this.angle);
        }

        public void Update()
        {
            if (KeyMouseReader.keyState.IsKeyDown(clockwise))
                Rotate(speed);
            if (KeyMouseReader.keyState.IsKeyDown(counterClockwise))
                Rotate(-speed);
        }

        public void Rotate(float amt)
        {
            angle += amt;
            if (angle < 0)
                angle += MathHelper.Pi * 2;
            else if (angle > MathHelper.Pi * 2)
                angle -= MathHelper.Pi * 2;
            circle.SetCircleSector(length, angle);
        }

        public static float ClampAngle(float angle)
        {
            while (angle < 0)
                angle += MathHelper.Pi * 2;
            while (angle > MathHelper.Pi * 2)
                angle -= MathHelper.Pi * 2;
            return angle;
        }

        public void Draw(SpriteBatch sb)
        {
            circle.Draw(sb);
        }

        public static void DrawOverlaps(List<Player> playerList, SpriteBatch sb)
        {
            List<Circle> overlaps = new List<Circle>();

            for (int i = 0; i < playerList.Count; i++)
            {
                for (int j = i + 1; j < playerList.Count; j++)
                {
                    Player p1 = playerList[i];
                    Player p2 = playerList[j];

                    float p1start = ClampAngle(p1.circle.startAngle);
                    float p2start = ClampAngle(p2.circle.startAngle);
                    float p1end = ClampAngle(p1.circle.endAngle);  
                    float p2end = ClampAngle(p2.circle.endAngle);

                    bool p1wrap = false;
                    bool p2wrap = false;

                    if (p1start > p1end)
                        p1wrap = true;
                    if (p2start > p2end)
                        p2wrap = true;

                    if (p1wrap && p2wrap)
                    {
                        p1start -= MathHelper.Pi * 2;
                        p2start -= MathHelper.Pi * 2;
                    }
                    else if (!p1wrap && p2wrap)
                    {
                        if (p2end < p1start)
                            p2end += MathHelper.Pi * 2;
                        else
                            p2start -= MathHelper.Pi * 2;
                    }
                    else if (p1wrap && !p2wrap)
                    {
                        if (p1end < p2start)
                            p1end += MathHelper.Pi * 2;
                        else
                            p1start -= MathHelper.Pi * 2;
                    }

                    if (p1start < p2end && p2start < p1end)
                    {
                        float overlapStart;
                        float overlapEnd;

                        if (p1start < p2start)
                            overlapStart = p2start;
                        else
                            overlapStart = p1start;
                        if (p1end > p2end) 
                            overlapEnd = p2end;
                        else
                            overlapEnd = p1end;

                        float overlapAngle = (overlapEnd + overlapStart) / 2;
                        float overlapLength = overlapEnd - overlapStart;

                        Color overlapColor = new Color(
                            (float)(p1.color.R + p2.color.R) / 510,
                            (float)(p1.color.G + p2.color.G) / 510,
                            (float)(p1.color.B + p2.color.B) / 510,
                            0.5f
                        );

                        Circle overlap = new Circle(p1.circle.pos, p1.circle.radius, p1.circle.thickness, overlapColor, p1.circle.points);
                        overlap.SetCircleSector(overlapLength, overlapAngle);
                        overlaps.Add(overlap);
                    }
                }
            }

            foreach (Circle c in overlaps)
            {
                BlendState blend = new BlendState()
                {
                    ColorSourceBlend = Blend.SourceColor,
                    AlphaSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.InverseDestinationColor,
                    AlphaDestinationBlend = Blend.DestinationAlpha
                };

                sb.End();
                sb.Begin(blendState: blend);

                c.Draw(sb);

                sb.End();
                sb.Begin();
            }
        }
    }
}
