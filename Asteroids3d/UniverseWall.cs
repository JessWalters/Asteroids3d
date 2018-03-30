using BEPUphysics;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids3d
{
    internal class UniverseWall : DrawableGameComponent
    {

        private Model model;
        private Texture2D moonTexture;
        private CompoundBody physicsObject;
        private Vector3 CurrentPosition
        {
            get
            {
                return ConversionHelper.MathConverter.Convert(physicsObject.Position);
            }
        }

        public UniverseWall(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public UniverseWall(Game game, BEPUutilities.Vector3 pos, int dimX, int dimY, int dimZ) : this(game)
        {
            physicsObject = new CompoundBody(new List<CompoundShapeEntry> {
                new CompoundShapeEntry(new BoxShape(10f, dimY, dimZ), new BEPUutilities.Vector3(dimX/2, 0, 0)),
                new CompoundShapeEntry(new BoxShape(10f, dimY, dimZ), new BEPUutilities.Vector3(-dimX/2, 0, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, 10f, dimZ),  new BEPUutilities.Vector3(0, dimY/2, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, 10f, dimZ),  new BEPUutilities.Vector3(0, -dimY/2, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, dimY, 10f),  new BEPUutilities.Vector3(0, 0, dimZ/2)),
                new CompoundShapeEntry(new BoxShape(dimX, dimY, 10f),  new BEPUutilities.Vector3(0, 0, -dimZ/2)),
            });
            physicsObject.CollisionInformation.Tag = this;
            Game.Services.GetService<Space>().Add(physicsObject);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>("skybox");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            physicsObject.CollisionInformation.Tag = this;

            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.PreferPerPixelLighting = false;
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
