using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids3d {
    internal class SmallAsteroid : DrawableGameComponent {

        private Model model;
        private Texture2D moonTexture;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition {
            get {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }

        public SmallAsteroid(Game game) : base(game) {
            game.Components.Add(this);
        }

        public SmallAsteroid(Game game, Vector3 pos) : this(game) {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public SmallAsteroid(Game game, Vector3 pos, float mass) : this(game, pos) {
            physicsObject.Mass = mass;
        }

        public SmallAsteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass) {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public SmallAsteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum) {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            moonTexture = Game.Content.Load<Texture2D>("moonsurface");
            model = Game.Content.Load<Model>("moon");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;


            base.LoadContent();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime) {
            foreach (var mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Matrix.CreateLookAt(Game1.CameraPosition, Vector3.Forward, Vector3.Up);
                    float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
                    float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                    float nearClipPlane = 1;
                    float farClipPlane = 200;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
