using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Asteroids3d {

    public class Camera {

        public Vector3 Position { get; set; }
        float yaw;
        float pitch;

        public float Yaw {
            get {
                return yaw;
            }
            set {
                yaw = MathHelper.WrapAngle(value);
            }
        }

        public float Pitch {
            get {
                return pitch;
            }
            set {
                pitch = MathHelper.Clamp(value, -MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        public Matrix ViewMatrix { get; private set; }

        public Matrix ProjectionMatrix { get; set; }

        public Matrix WorldMatrix { get; private set; }

        public Game1 Game { get; private set; }

        // Constructor that places the camera at a given place
        public Camera(Game1 game) {
            Game = game;
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4f / 3f, .1f, 10000.0f);
        }

        // Updates the state of the camera. Kind of messy, Some code from previous implentation left in just incase it becomes needed. 
        public void Update() {

            // Rotate based off of both the ships yaw and the yaw of the camera **Old implementation, Kept because I was proud of it**
            Vector3 cameraHorizontalOffset = Vector3.Transform(Vector3.Transform(Game.ship.WorldMatrix.Backward * 15, Matrix.CreateRotationY(Yaw)), Matrix.CreateRotationY(-Game.ship.shipYaw));
            Vector3 cameraVerticalOffset = Game.ship.WorldMatrix.Up * 5;
            Position = Game.ship.position + cameraHorizontalOffset + cameraVerticalOffset; 

            //Turn based on mouse input. **Now handled in Ship.cs**
            Yaw = Game.ship.shipYaw;
            Pitch = Game.ship.shipPitch;

            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);

            WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
            ViewMatrix = Matrix.Invert(WorldMatrix);
        }
    }
}