using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids3d {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Vector3 CameraPosition {
            get;
            private set;
        }

        public static Vector3 CameraDirection {
            get;
            private set;
        }

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());

            new MediumAsteroid(this, new Vector3(1, 5, -20), 2, new Vector3(0.2f, -3.9f, 5.0f), new Vector3(0.1f, 0.0f, 0.0f));
            new MediumAsteroid(this, new Vector3(10, 4, -5), 2, new Vector3(0.2f, 0.6f, -6f), new Vector3(0.9f, 0.0f, 3f));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(0,0,-22), 20, 20, 1));
            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(0, 0, 0), 20, 20, 1));
            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(0, 0, 0), 1, 20, 20));
            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(20, 0, -22), 1, 20, 20));
            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(0, 0, -10), 20, 1, 20));
            Services.GetService<Space>().Add(new Box(new BEPUutilities.Vector3(0, 0, -22), 20, 20, 1));

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

            // TODO: Add your update logic here
            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
