using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace circularpong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        public static int _screenWidth = 1000;
        public static int _screenHeight = 1000;
        public static Vector2 _screenCenter;

        public static Random _random;

        public static Texture2D _pixelTexture, _circleTexture;

        public static Circle _circle, loader;

        private List<Player> playerList;
        private List<Ball> ballList;

        private static Color bgColor, fgColor, redColor, blueColor, yellowColor;

        private static float playerLength, playerThickness, playerSpeed;
        private static float ballStartSpeed, ballMaxSpeed, ballAcceleration;
        private static int ballBounces;     

        public enum Impacts
        {
            None,
            Hit,
            Miss,
        }

        private enum Gamestates
        {
            Menu,
            Countdown,
            Playing,
            Death,
        }
        private Gamestates _gamestate;
        private static int playCountdown, countdownStart;

        private void ResetGame()
        {
            playerList.Clear();
            ballList.Clear();

            playerList.Add(new Player(0, playerLength, playerThickness, playerSpeed, redColor, Keys.Right, Keys.Left));
            //playerList.Add(new Player(MathHelper.Pi, playerLength, playerThickness, playerSpeed, blueColor, Keys.D, Keys.A));
            //playerList.Add(new Player(MathHelper.Pi / 2, playerLength, playerThickness, playerSpeed, yellowColor, Keys.W, Keys.S));

            ballList.Add(new Ball(playerList[0], 6, ballStartSpeed, ballMaxSpeed, ballAcceleration, MathHelper.Pi / 4, _screenCenter, 0, _circleTexture));
            //ballList.Add(new Ball(playerList[1], 6, ballStartSpeed, ballMaxSpeed, ballAcceleration, MathHelper.Pi / 4, _screenCenter, MathHelper.Pi, _circleTexture));
        }

        private void DrawCenteredString(string text, float offsetY = 0)
        {
            Vector2 textSize = _font.MeasureString(text);
            _spriteBatch.DrawString(_font, text, new Vector2(_screenCenter.X - textSize.X / 2, _screenCenter.Y - textSize.Y / 2 + offsetY), fgColor * 1.4f);
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {       
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.ApplyChanges();

            _screenCenter = new Vector2(_screenWidth / 2, _screenHeight / 2);

            _random = new Random();

            bgColor = new Color(14, 18, 18);
            fgColor = new Color(75, 90, 90);
            redColor = new Color(200, 45, 55);
            blueColor = new Color(35, 180, 190);
            yellowColor = new Color(200, 195, 45);

            _circle = new Circle(new Vector2(_screenCenter.X, _screenCenter.Y), _screenHeight / 3, 2, fgColor, 100);

            countdownStart = 149;

            playerList = new List<Player>();
            ballList = new List<Ball>();

            playerLength = MathHelper.Pi / 6;
            playerThickness = 6;
            playerSpeed = MathHelper.Pi / 32;

            ballBounces = 0;
            ballStartSpeed = _circle.radius / 120;
            ballMaxSpeed = ballStartSpeed * 7;
            ballAcceleration = 0.02f;

            //ResetGame();

            _gamestate = Gamestates.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixelTexture = Content.Load<Texture2D>("pixel");
            _circleTexture = Content.Load<Texture2D>("circle");

            _font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyMouseReader.Update();

            if (_gamestate == Gamestates.Playing)
            {
                foreach (Ball b in ballList)
                {
                    Impacts result = b.Update();
                    if (result == Impacts.Miss)
                        _gamestate = Gamestates.Death;
                    //else if (result == Impacts.Hit)
                    //    b.ChangePlayer(playerList[(ballBounces + 1) % playerList.Count]);
                }

                ballBounces = ballList.Sum(e => e.bounces);

                foreach (Player p in playerList)
                    p.Update();
            }
            else if (_gamestate == Gamestates.Menu || _gamestate == Gamestates.Death)
            {
                if (KeyMouseReader.KeyPressed(Keys.Enter))
                {
                    playCountdown = countdownStart;
                    loader = new Circle(_screenCenter, _circle.radius + 2, 6, fgColor, 100); 
                    _gamestate = Gamestates.Countdown;
                }
            }
            else if (_gamestate == Gamestates.Countdown)
            {
                if (playCountdown > 0)
                {
                    playCountdown--;
                    float progressAngle = MathHelper.Pi * ((float)playCountdown / countdownStart);
                    loader.Modify(loader.pos, loader.radius, loader.thickness, loader.color, loader.points, -progressAngle, progressAngle, loader.radial);
                }
                else
                {
                    ResetGame();
                    _gamestate = Gamestates.Playing;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(bgColor);

            _spriteBatch.Begin();

            _circle.Draw(_spriteBatch);

            if (_gamestate == Gamestates.Playing)
            {
                foreach (Ball b in ballList)
                    b.Draw(_spriteBatch);
                foreach (Player p in playerList)
                    p.Draw(_spriteBatch);

                Player.DrawOverlaps(playerList, _spriteBatch);

                _spriteBatch.DrawString(_font, $"Bounces: {ballBounces}", new Vector2(25, 25), fgColor * 1.4f);
            }
            else if (_gamestate == Gamestates.Countdown)
            {
                loader.Draw(_spriteBatch);
                DrawCenteredString($"{playCountdown / 30 + 1}");
            }
            else if (_gamestate == Gamestates.Menu)
            {
                DrawCenteredString($"Press Enter To Play");
            }
            else if (_gamestate == Gamestates.Death)
            {
                DrawCenteredString($"Bounces: {ballBounces}", -20);
                DrawCenteredString($"Press Enter To Play Again", 20);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
