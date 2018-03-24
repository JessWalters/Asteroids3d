using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
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
        Model skyboxModel;
        public Ship ship;
        private double lastShot = 0;

        public static Vector3 CameraPosition {
            get;
            private set;
        }

        public static Camera Camera {
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
            Random rnd = new Random();

            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            
            new UniverseWall(this, new BEPUutilities.Vector3(0, 0, 0), 400, 400, 400);

            for (int i = 1; i <= 500; i++) {
                switch (rnd.Next(1, 4)) {
                    case 1:
                        new SmallAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
                        break;
                    case 2:
                        new MediumAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
                        break;
                    case 3:
                        new LargeAsteroid(this, new Vector3(rnd.Next(-80, 80), rnd.Next(-80, 80), rnd.Next(-80, 80)), 2, new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)), new Vector3(rnd.Next(-5, 5), rnd.Next(-5, 5), rnd.Next(-5, 5)));
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

            MouseState mouse = Mouse.GetState();
            GameTime tmp = gameTime;

            if (mouse.LeftButton == ButtonState.Pressed && gameTime.TotalGameTime.TotalMilliseconds - lastShot > 500) {
                new Bullet(this, ship.position + (ship.WorldMatrix.Forward * 5.5f), 2, ship.WorldMatrix.Forward * 200, Vector3.Zero);
                lastShot = gameTime.TotalGameTime.TotalMilliseconds;
            }

            Camera.Update();
            ship.UpdateShip((float)gameTime.ElapsedGameTime.TotalSeconds);
            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            
            base.Draw(gameTime);
        }

    }
}
