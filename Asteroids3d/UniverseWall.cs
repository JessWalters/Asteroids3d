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
                new CompoundShapeEntry(new BoxShape(1, dimY, dimZ), new BEPUutilities.Vector3(dimX/2, 0, 0)),
                new CompoundShapeEntry(new BoxShape(1, dimY, dimZ), new BEPUutilities.Vector3(-dimX/2, 0, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, 1, dimZ),  new BEPUutilities.Vector3(0, dimY/2, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, 1, dimZ),  new BEPUutilities.Vector3(0, -dimY/2, 0)),
                new CompoundShapeEntry(new BoxShape(dimX, dimY, 1),  new BEPUutilities.Vector3(0, 0, dimZ/2)),
                new CompoundShapeEntry(new BoxShape(dimX, dimY, 1),  new BEPUutilities.Vector3(0, 0, -dimZ/2)),
            });
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
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
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
