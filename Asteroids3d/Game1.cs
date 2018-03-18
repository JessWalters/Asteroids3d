using BEPUphysics;
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
        Camera camera;
        Model skyboxModel;

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

            new UniverseWall(this, new BEPUutilities.Vector3(0, 0, 0), 80, 80, 80);

            new MediumAsteroid(this, new Vector3(1, 5, -30), 2, new Vector3(0.2f, 10f, 0f), new Vector3(0.1f, 0.0f, 0.0f));
            new MediumAsteroid(this, new Vector3(5, 5, 0), 2, new Vector3(0f, 0f, 15f), new Vector3(0.9f, 0.0f, 3f));

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

            // TODO: Add your update logic here
            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            CameraPosition = new Vector3(CameraPosition.X, CameraPosition.Y, 100);
            CameraDirection = new Vector3(CameraDirection.X + 1, CameraDirection.Y, CameraDirection.Z + 0.05f);
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

        private void DrawSkybox() {
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Clamp;
            samplerState.AddressV = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = samplerState;

            DepthStencilState depthStencil = new DepthStencilState();
            depthStencil.DepthBufferEnable = false;
            GraphicsDevice.DepthStencilState = depthStencil;

            //Matrix[] skyboxTransforms = new Matrix[]
        }
    }
}
