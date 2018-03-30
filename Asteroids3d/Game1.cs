using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Asteroids3d {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont endFont;
        SpriteFont scoreFont;
        Model skyboxModel;
        Texture2D liveShip;
        SoundEffect pew;
        SoundEffectInstance pewInstance;
        SoundEffect win;
        SoundEffectInstance winInstance;
        SoundEffect rip;
        SoundEffectInstance ripInstance;
        SoundEffect drive;

        SoundEffect boom;

        public Ship ship;
        private double lastShot = 0;
        private short NUM_ASTEROIDS = 100;

        // 0 = playing, 1 = win, 2 = lose
        private int gameState = 0;

        // The total asteroid score at the current moment
        // Initial refers to the starting value;
        private short initialAsteroidScore = 0;
        public short totalAsteroidScore = 0;

        // Number of lives the player has
        public short Lives = 3;

        // Chance asteroid to asteroid collision results in break (1-100)
        public short collisionChance = 50;

        // Amount of asteroids that need to be destroyed
        public float amount = .1f;
        
        public bool playing {
            get;
            private set;
        }

        public static Vector3 CameraPosition {
            get;
            private set;
        }

        public static Camera Camera {
            get;
            private set;
        }

        public SoundEffectInstance boomInstance {
            get;
            private set;
        }

        public SoundEffectInstance driveInstance {
            get;
            private set;
        }

        public object MouseState { get; internal set; }

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            playing = true;
            Random rnd = new Random();

            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            
            new UniverseWall(this, new BEPUutilities.Vector3(0, 0, 0), 400, 400, 400);

            for (int i = 1; i <= NUM_ASTEROIDS; i++) {
                switch (rnd.Next(1, 4)) {
                    case 1:
                        new SmallAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-10, 10), rnd.Next(-10, 10), rnd.Next(-10, 10)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
                        initialAsteroidScore++;
                        totalAsteroidScore++;
                        break;
                    case 2:
                        new MediumAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-10, 10), rnd.Next(-10, 10), rnd.Next(-10, 10)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
                        initialAsteroidScore += 2;
                        totalAsteroidScore += 2;
                        break;
                    case 3:
                        new LargeAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-10, 10), rnd.Next(-10, 10), rnd.Next(-10, 10)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
                        initialAsteroidScore += 2;
                        totalAsteroidScore += 2;
                        break;
                    default:
                        break;
                }
            }

            ship = new Ship(this, new Vector3(0, 0, -10), 2, new Vector3(0f, 0f, 5f), Vector3.Zero);

            Camera = new Camera(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            liveShip = Content.Load<Texture2D>("lives");
            scoreFont = Content.Load<SpriteFont>("score");
            endFont = Content.Load<SpriteFont>("Gameover");
            pew = Content.Load<SoundEffect>("pew");
            pewInstance = pew.CreateInstance();

            win = Content.Load<SoundEffect>("win");
            winInstance = win.CreateInstance();

            rip = Content.Load<SoundEffect>("rip");
            ripInstance = rip.CreateInstance();

            drive = Content.Load<SoundEffect>("drive");
            driveInstance = drive.CreateInstance();
        
            boom = Content.Load<SoundEffect>("boom");
            boomInstance = boom.CreateInstance();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (initialAsteroidScore - totalAsteroidScore > initialAsteroidScore * amount) {
                GameOver(true);
            }

            if (playing) {
                MouseState mouse = Mouse.GetState();
                GameTime tmp = gameTime;

                if (mouse.LeftButton == ButtonState.Pressed && gameTime.TotalGameTime.TotalMilliseconds - lastShot > 500) {
                    new Bullet(this, ship.position + (ship.WorldMatrix.Forward * 5.5f), 2, ship.WorldMatrix.Forward * 200, ship.Pitch, ship.Yaw);
                    lastShot = gameTime.TotalGameTime.TotalMilliseconds;
                    pewInstance.Volume = 1.0f;
                    pewInstance.Play();
                }

                Camera.Update();
                ship.UpdateShip((float)gameTime.ElapsedGameTime.TotalSeconds);
                Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);

            spriteBatch.Begin(0, null, null);
            for (int i = 0; i < Lives; i++) {
                Vector2 pos = new Vector2(10, 10 + 70 * i);
                spriteBatch.Draw(liveShip, pos);
            }
            spriteBatch.DrawString(scoreFont, "Your Score score: " + (initialAsteroidScore - totalAsteroidScore), new Vector2(100,100), Color.White);
            spriteBatch.DrawString(scoreFont, "Goal score: " + initialAsteroidScore * amount, new Vector2(100, 150), Color.White);

            if (gameState == 1) {
                spriteBatch.DrawString(endFont, "You win", new Vector2(700, 400), Color.White);
            }
            if (gameState == 2) {
                spriteBatch.DrawString(endFont, "You lose", new Vector2(700, 400), Color.White);
            }

            spriteBatch.End();
        }

        internal void GameOver(bool win) {
            playing = false;
            gameState = win ? 1 : 2;
            if (win)
                winInstance.Play();
            else
                ripInstance.Play();
        }
    }
}
