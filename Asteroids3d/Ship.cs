using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids3d {
    public class Ship : DrawableGameComponent {

        private Model shipModel;
        private Texture2D shipTexture;
        private Sphere physicsObject;

        public Matrix WorldMatrix { get; private set; }

        public float shipPitch;
        public float shipYaw;
        public static float MAX_VELOCITY = 20f;

        public Game1 Game { get; private set; }

        public float Yaw {
            get {
                return shipYaw;
            }
            set {
                shipYaw = MathHelper.WrapAngle(value);
            }
        }
        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch {
            get {
                return shipPitch;
            }
            set {
                shipPitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        public Vector3 position;
        float rotationSpeed = 1f / 60f;
        float forwardSpeed = 500f / 60f;

        public Ship(Game1 game) : base(game) {
            game.Components.Add(this);
            Game = game;
        }

        public Ship(Game1 game, Vector3 pos) : this(game) {
            physicsObject = new Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = .3f;
            physicsObject.CollisionInformation.Tag = this;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            if (shipModel == null) {
                shipModel = Game.Content.Load<Model>("Models\\p1_wedge");
                physicsObject.Radius = shipModel.Meshes[0].BoundingSphere.Radius * .75f;
            }
            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Ship(Game1 game, Vector3 pos, float mass) : this(game, pos) {
            physicsObject.Mass = mass;
        }

        public Ship(Game1 game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass) {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public Ship(Game1 game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum) {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize() {
            base.Initialize();
        }
        protected override void LoadContent() {
            if (Game != null && physicsObject != null) {
                shipModel = Game.Content.Load<Model>("Models\\p1_wedge");
                physicsObject.Radius = shipModel.Meshes[0].BoundingSphere.Radius * .75f;
            }
            base.LoadContent();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        } 

        public void UpdateShip(float dt) {

            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            MouseState mouseState = Mouse.GetState();
            
            if (keyboardState.IsKeyDown(Keys.W) || (currentState.DPad.Up == ButtonState.Pressed)) {
                Vector3 moveNB = WorldMatrix.Forward;
                BEPUutilities.Vector3 move = new BEPUutilities.Vector3(moveNB.X, moveNB.Y, moveNB.Z);
                move *= 2;
                //physicsObject. = new BEPUutilities.Vector3(move.X, move.Y, move.Z);
                if (physicsObject.LinearVelocity.Length() < MAX_VELOCITY) {
                    physicsObject.ApplyLinearImpulse(ref move);
                }
                Game.driveInstance.Play();
            }
            else if (keyboardState.IsKeyDown(Keys.S) || (currentState.DPad.Down == ButtonState.Pressed)) {
                Vector3 moveNB = WorldMatrix.Backward;
                BEPUutilities.Vector3 move = new BEPUutilities.Vector3(moveNB.X, moveNB.Y, moveNB.Z);
                //physicsObject. = new BEPUutilities.Vector3(move.X, move.Y, move.Z);
                if (physicsObject.LinearVelocity.Length() < MAX_VELOCITY) {
                    physicsObject.ApplyLinearImpulse(ref move);
                }
                Game.driveInstance.Play();
            }
            else {
                if (Game.driveInstance.State == Microsoft.Xna.Framework.Audio.SoundState.Playing) {
                    Game.driveInstance.Stop();
                }
            }

            Yaw += (200 - mouseState.X) * dt * .12f;
            Pitch += (200 - mouseState.Y) * dt * .12f;
            Mouse.SetPosition(200, 200);

            position = new Vector3(physicsObject.Position.X, physicsObject.Position.Y, physicsObject.Position.Z);
            //Game1.Camera.updateCamera(new Vector3(physicsObject.Position.X, physicsObject.Position.Y, physicsObject.Position.Z));
            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, shipPitch) * Matrix.CreateFromAxisAngle(Vector3.Up, shipYaw);
            WorldMatrix *= Matrix.CreateTranslation(new Vector3(physicsObject.Position.X, physicsObject.Position.Y, physicsObject.Position.Z));
        }

        public override void Draw(GameTime gameTime) {

            Matrix[] transforms = new Matrix[shipModel.Bones.Count];
            shipModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in shipModel.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = WorldMatrix;
                    effect.View = Game1.Camera.ViewMatrix;
                    effect.Projection = Game1.Camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            try
            {
                var otherEntityInformation = other as EntityCollidable;

                if (otherEntityInformation.Tag == null)
                {
                    return;
                }

                Vector3 pos = new Vector3(physicsObject.Position.X, physicsObject.Position.Y, physicsObject.Position.Z);


                if (otherEntityInformation.Tag.GetType() == typeof(LargeAsteroid))
                {
                    Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                    Game.Components.Remove((LargeAsteroid)otherEntityInformation.Tag);

                    Random rnd = new Random();
                    new MediumAsteroid(Game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));
                    new MediumAsteroid(Game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));

                    Game.Services.GetService<Space>().Remove(sender.Entity);
                    Game.Components.Remove(this);
                    Game.Lives--;
                    if (Game.Lives == 0) {
                        Game.GameOver(false);
                    }
                    else {
                        Game.ship = new Ship(Game, new Vector3(0, 0, -10), 2, new Vector3(0f, 0f, 5f), Vector3.Zero);
                        Game.boomInstance.Play();

                    }
                }

                if (otherEntityInformation.Tag.GetType() == typeof(MediumAsteroid))
                {
                    Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                    Game.Components.Remove((MediumAsteroid)otherEntityInformation.Tag);

                    Random rnd = new Random();
                    new SmallAsteroid(Game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));

                    Game.Services.GetService<Space>().Remove(sender.Entity);
                    Game.Components.Remove(this);
                    Game.Lives--;
                    if (Game.Lives == 0) {
                        Game.GameOver(false);
                    }
                    else {
                        Game.ship = new Ship(Game, new Vector3(0, 0, -10), 2, new Vector3(0f, 0f, 5f), Vector3.Zero);
                        Game.boomInstance.Play();

                    }
                }

                if (otherEntityInformation.Tag.GetType() == typeof(SmallAsteroid))
                {
                    Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                    Game.Components.Remove((SmallAsteroid)otherEntityInformation.Tag);

                    Game.Services.GetService<Space>().Remove(sender.Entity);
                    Game.Components.Remove(this);
                    Game.Lives--;
                    if (Game.Lives == 0) {
                        Game.GameOver(false);
                    }
                    else {
                        Game.ship = new Ship(Game, new Vector3(0, 0, -10), 2, new Vector3(0f, 0f, 5f), Vector3.Zero);
                        Game.boomInstance.Play();
                    }
                }
            }
            catch (ArgumentException)
            {

            }
        }
    }
}