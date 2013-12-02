using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Coursework
{
    public class Camera : GameComponent
    {
        public Matrix viewMatrix;

        private Vector3 cameraPosition;
        private Vector3 cameraRotation;
        //private Quaternion _cameraRotation;

        private float cameraSpeed;
        private Vector3 cameraLookAt;
        private Vector3 cameraUp;
        private int fov;
        private float ratio;

        //private Vector3 mouseRotBuffer;
        //private MouseState curMouseState;
        //private MouseState preMouseState;


        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRotation; }
            set
            {
                cameraRotation = value;
                UpdateLookAt();
            }
        }

        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraUp);
            }
        }

        public Vector3 LookAt
        {
            get { return cameraLookAt; }
            set { cameraLookAt = value; }
        }
        public Vector3 Up
        {
            get { return cameraUp; }
            set { cameraUp = value; }
        }


        public Camera(Game game, Vector3 position, Vector3 rotation, float speed)
            : base(game)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Game.GraphicsDevice.Viewport.AspectRatio,
                0.05f,
                1500.0f);

            //cameraSpeed = speed;
            //fov = 45;
            //ratio = 1.777777777777778f;
            //MoveTo(position, rotation);
            //preMouseState = Mouse.GetState();
        }


        private void UpdateLookAt()
        {
            //Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);

            //Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            //Vector3 upOffset = Vector3.Transform(Vector3.Up, Matrix.CreateFromYawPitchRoll(cameraRotation.X, cameraRotation.Y, cameraRotation.Z));
            //cameraUp = upOffset;
            //cameraLookAt = cameraPosition + lookAtOffset;

        }

        /*private void MoveTo(Vector3 pos, Vector3 rot)
        {
            Position = pos;
            Rotation = rot;
        }
        
        private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotX = Matrix.CreateRotationX(cameraRotation.X);
            Matrix rotY = Matrix.CreateRotationY(cameraRotation.Y);
            Matrix rotZ = Matrix.CreateRotationZ(cameraRotation.Z);

            Matrix rotation = rotX * rotY * rotZ;

            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotation);

            return cameraPosition + movement;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);          
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            curMouseState = Mouse.GetState();

            Console.Write(Rotation);

            KeyboardState ks = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;

            //if (ks.IsKeyDown(Keys.A))
            //{
            //    moveVector.X = 1;
            //}
            //if (ks.IsKeyDown(Keys.D))
            //{
            //    moveVector.X = -1;
            //}
            //if (ks.IsKeyDown(Keys.S))
            //{
            //    moveVector.Z = -1;
            //}
            //if (ks.IsKeyDown(Keys.W))
            //{
            //    moveVector.Z = 1;
            //}

            //if (ks.IsKeyDown(Keys.Space))
            //{
            //    moveVector.Y = 1;
            //}
            //if (ks.IsKeyDown(Keys.LeftControl))
            //{
            //    moveVector.Y = -1;
            //}

            //if (moveVector != Vector3.Zero)
            //{
            //    moveVector.Normalize();
            //    moveVector *= delta * cameraSpeed;
            //    Move(moveVector);
            //}

            //float deltaX; 
            //float deltaY;

            //if (curMouseState != preMouseState)
            //{
            //    deltaX = curMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
            //    deltaY = curMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

            //    mouseRotBuffer.X -= 0.1f * deltaX * delta;
            //    mouseRotBuffer.Y -= 0.1f * deltaY * delta;

            //    if (mouseRotBuffer.Y < MathHelper.ToRadians(-75.0f))
            //        mouseRotBuffer.Y = mouseRotBuffer.Y - (mouseRotBuffer.Y - MathHelper.ToRadians(-75.0f));
            //    if (mouseRotBuffer.Y > MathHelper.ToRadians(75.0f))
            //        mouseRotBuffer.Y = mouseRotBuffer.Y - (mouseRotBuffer.Y - MathHelper.ToRadians(75.0f));

            //    Rotation = new Vector3(-MathHelper.Clamp(mouseRotBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)), MathHelper.WrapAngle(mouseRotBuffer.X), 0);

            //    deltaX = 0;
            //    deltaY = 0;

            //}
            //Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            //preMouseState = curMouseState;

            base.Update(gameTime);
        }*/
    }
}
