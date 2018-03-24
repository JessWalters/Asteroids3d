using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroids3d {

    internal class Bullet : DrawableGameComponent {

        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;

        public Bullet(Game game) : base(game) {
            game.Components.Add(this);
        }

        public Bullet(Game game, Vector3 pos) : this(game) {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Tag = this;
            if (model != null) {
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public Bullet(Game game, Vector3 pos, float mass) : this(game, pos) {
            physicsObject.Mass = mass;
        }

        public Bullet(Game game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass) {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public Bullet(Game game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum) {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize() {
            base.Initialize();
        }
        
        protected override void LoadContent() {
            model = Game.Content.Load<Model>("Rockfbx");
            if (physicsObject != null) {
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;
                physicsObject.CollisionInformation.Tag = this;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            }
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
                    effect.View = Game1.Camera.ViewMatrix;
                    effect.Projection = Game1.Camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            try {
                Game.Services.GetService<Space>().Remove(sender.Entity);
                Game.Components.Remove(this);

                var otherEntityInformation = other as EntityCollidable;

                if (otherEntityInformation.Tag == null) {
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
                }

                if (otherEntityInformation.Tag.GetType() == typeof(MediumAsteroid)) {
                    Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                    Game.Components.Remove((MediumAsteroid) otherEntityInformation.Tag);

                    Random rnd = new Random();
                    new SmallAsteroid(Game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));
                }

                if (otherEntityInformation.Tag.GetType() == typeof(SmallAsteroid))
                {
                    Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                    Game.Components.Remove((SmallAsteroid)otherEntityInformation.Tag);
                }
            }
            catch (ArgumentException) {

            }
        }
    }
}