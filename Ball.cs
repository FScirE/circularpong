using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace circularpong
{
    internal class Ball
    {
        Player player;
        float radius;
        float startSpeed;
        float maxSpeed;
        float speed;
        float acceleration;
        float randomSpread;
        Vector2 pos;
        float angle;
        Texture2D texture;
        Color color;

        public int bounces {  get; private set; }

        public Ball(Player player, float radius, float startSpeed, float maxSpeed, float acceleration, float randomSpread, Vector2 startPos, float startAngle, Texture2D texture)
        {
            ChangePlayer(player);   
            this.radius = radius;
            this.startSpeed = startSpeed;
            this.maxSpeed = maxSpeed;
            speed = startSpeed;
            this.acceleration = acceleration;
            this.randomSpread = randomSpread;
            pos = startPos;
            angle = startAngle;
            this.texture = texture;
        }

        public Game1.Impacts Update()
        {
            Game1.Impacts impact = Game1.Impacts.None;

            Vector2 ballDirVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
            pos += ballDirVector;

            Vector2 toCenter = pos - Game1._screenCenter;
            float toCenterAngle = (float)Math.Atan2(toCenter.Y, toCenter.X);
            if (toCenterAngle < 0)
                toCenterAngle += MathHelper.Pi * 2;
            else if (toCenterAngle > MathHelper.Pi * 2)
                toCenterAngle -= MathHelper.Pi * 2;

            float centerDistance = Game1._circle.radius - radius;
            if (Vector2.Distance(pos, Game1._screenCenter) > centerDistance) // bounce
            {
                if ((toCenterAngle <= player.circle.endAngle && toCenterAngle >= player.circle.startAngle) ||
                    (toCenterAngle - MathHelper.Pi * 2 <= player.circle.endAngle && toCenterAngle - MathHelper.Pi * 2 >= player.circle.startAngle) ||
                    (toCenterAngle + MathHelper.Pi * 2 <= player.circle.endAngle && toCenterAngle + MathHelper.Pi * 2 >= player.circle.startAngle))
                {
                    bounces++;
                    impact = Game1.Impacts.Hit;
                    pos = Game1._screenCenter + new Vector2((float)Math.Cos(toCenterAngle), (float)Math.Sin(toCenterAngle)) * centerDistance;
                    float offset = (Game1._random.NextSingle() - 0.5f) * 2 * randomSpread;
                    angle = toCenterAngle + MathHelper.Pi + offset;
                    speed = startSpeed + maxSpeed * (1 - (float)Math.Pow(MathHelper.E, -bounces * acceleration));
                }
                else
                    impact = Game1.Impacts.Miss;
            }

            return impact;
        }

        public void ChangePlayer(Player player)
        {
            this.player = player;
            color = this.player.circle.color;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, pos - new Vector2(radius), null, color, 0, Vector2.Zero, (2 * radius) / texture.Width, SpriteEffects.None, 0);
        }
    }
}
