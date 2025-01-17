using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroids3d {
    internal class MediumAsteroid : DrawableGameComponent {

        private Game1 game;
        private Model model;
        private BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition {
            get {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }

        public MediumAsteroid(Game1 game) : base(game) {
            this.game = game;
            game.Components.Add(this);
        }

        public MediumAsteroid(Game1 game, Vector3 pos) : this(game) {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Tag = this;
            if (model != null)
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .75f;

            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public MediumAsteroid(Game1 game, Vector3 pos, float mass) : this(game, pos) {
            physicsObject.Mass = mass;
        }

        public MediumAsteroid(Game1 game, Vector3 pos, float mass, Vector3 linMomentum) : this(game, pos, mass) {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }

        public MediumAsteroid(Game1 game, Vector3 pos, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, mass, linMomentum) { 
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }

        public override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            model = Game.Content.Load<Model>("moon");
            if (physicsObject != null) {
                physicsObject.CollisionInformation.Tag = this;
                physicsObject.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
                physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius * .75f;
            }

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
        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair) {
            try {
                Random rnd = new Random();

                var otherEntityInformation = other as EntityCollidable;

                if (otherEntityInformation.Tag == null) {
                    return;
                }

                Vector3 pos = new Vector3(physicsObject.Position.X, physicsObject.Position.Y, physicsObject.Position.Z);

                if (otherEntityInformation.Tag.GetType() == typeof(LargeAsteroid)) {

                    if (rnd.Next(0, 100) < game.collisionChance) {
                        new MediumAsteroid(game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));
                        new MediumAsteroid(game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10)));
                        Game.Components.Remove((LargeAsteroid)otherEntityInformation.Tag);
                        Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                        Game.Components.Remove(this);
                        Game.Services.GetService<Space>().Remove(sender.Entity);

                        // -1 med -1 lg +2 med, total score -3
                        game.totalAsteroidScore -= 3;
                        game.boomInstance.Play();
                    }
                }

                if (otherEntityInformation.Tag.GetType() == typeof(MediumAsteroid)) {

                    if (rnd.Next(0, 100) < game.collisionChance) {
                        new SmallAsteroid(game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10))); Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove((MediumAsteroid)otherEntityInformation.Tag);
                        Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                        Game.Components.Remove(this);
                        Game.Services.GetService<Space>().Remove(sender.Entity);

                        // -2 med +1 sm, total score -3
                        game.totalAsteroidScore -= 3;
                        game.boomInstance.Play();
                    }
                }

                if (otherEntityInformation.Tag.GetType() == typeof(SmallAsteroid)) {
                    if (rnd.Next(0, 100) < game.collisionChance) {
                        new SmallAsteroid(game, pos, 1, new Vector3(rnd.Next(0, 10), rnd.Next(0, 10), rnd.Next(0, 10))); Game.Services.GetService<Space>().Remove(sender.Entity);
                        Game.Components.Remove((SmallAsteroid)otherEntityInformation.Tag);
                        Game.Services.GetService<Space>().Remove(otherEntityInformation.Entity);
                        Game.Components.Remove(this);
                        Game.Services.GetService<Space>().Remove(sender.Entity);

                        // -1 small -1 med +1 sm, total score -2
                        game.totalAsteroidScore -= 2;
                        game.boomInstance.Play();
                    }

                }
            }
            catch (ArgumentException) {

            }
        }

    }
}
