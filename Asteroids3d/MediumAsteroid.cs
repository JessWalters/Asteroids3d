using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids3d {
    internal class MediumAsteroid : DrawableGameComponent {

        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition {
            get {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }

        public MediumAsteroid(Game game) : base(game) {
            game.Components.Add(this);
        }

        public MediumAsteroid(Game game, Vector3 pos) : this(game) {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            if (model != null)
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public MediumAsteroid(Game game, Vector3 pos, float mass) : this(game, pos) {
            physicsObject.Mass = mass;
        }

        public MediumAsteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass) {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public MediumAsteroid(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum) { 
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            model = Game.Content.Load<Model>("moon");
            if (physicsObject != null)
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;

            base.LoadContent();
        }

        protected override void UnloadContent() {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime) {

            foreach (var mesh in model.Meshes)  {

                foreach (BasicEffect effect in mesh.Effects) {

                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Game1.Camera.ViewMatrix;
                    effect.Projection = Game1.Camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

    }
}
