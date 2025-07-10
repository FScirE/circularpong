using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace circularpong
{
    internal static class GameHandler
    {
        static List<Player> playerList;
        static List<Ball> ballList;

        static float playerLength, playerThickness, playerSpeed;
        static float ballSize, ballStartSpeed, ballMaxSpeed, ballAcceleration, randomAngleSpread;
        static bool oneBallMode;

        public static int ballBounces { get; private set; }

        public static void Initialize(Circle circle)
        {
            playerList = new List<Player>();
            ballList = new List<Ball>();

            playerLength = MathHelper.Pi / 6;
            playerThickness = 6;
            playerSpeed = MathHelper.Pi / 32;

            ballBounces = 0;
            ballSize = 6;
            ballStartSpeed = circle.radius / 120;
            ballMaxSpeed = ballStartSpeed * 7;
            ballAcceleration = 0.02f;
            randomAngleSpread = MathHelper.Pi / 4;

            oneBallMode = true;
        }

        public static void Reset(bool oneBall, bool playerTwo = false, bool playerThree = false)
        {
            oneBallMode = oneBall;

            playerList.Clear();
            ballList.Clear();

            playerList.Add(new Player(0, playerLength, playerThickness, playerSpeed, Game1.redColor, Keys.Right, Keys.Left));
            if (playerTwo)
                playerList.Add(new Player(MathHelper.Pi, playerLength, playerThickness, playerSpeed, Game1.blueColor, Keys.D, Keys.A));
            if (playerThree)
                playerList.Add(new Player(MathHelper.Pi / 2, playerLength, playerThickness, playerSpeed, Game1.yellowColor, Keys.W, Keys.S));

            int count = 0;
            do
                ballList.Add(new Ball(playerList[count], ballSize, ballStartSpeed, ballMaxSpeed, ballAcceleration, randomAngleSpread, Game1._screenCenter, playerList[count].angle, Game1._circleTexture));
            while (!oneBallMode && ++count < playerList.Count);
        }

        public static bool Update()
        {
            foreach (Ball b in ballList)
            {
                Game1.Impacts result = b.Update();
                if (result == Game1.Impacts.Miss)
                    return false;
                else if (oneBallMode && result == Game1.Impacts.Hit)
                    b.ChangePlayer(playerList[(ballBounces + 1) % playerList.Count]);
            }

            ballBounces = ballList.Sum(e => e.bounces);

            foreach (Player p in playerList)
                p.Update();

            return true;
        }

        public static void Draw(SpriteBatch sb)
        {
            foreach (Ball b in ballList)
                b.Draw(sb);
            foreach (Player p in playerList)
                p.Draw(sb);

            Player.DrawOverlaps(playerList, sb);
        }
    }
}
