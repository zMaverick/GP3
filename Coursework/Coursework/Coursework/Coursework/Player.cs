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
    public class Player : Microsoft.Xna.Framework.GameComponent
    {
        private Vector3 playerPosition;
        private Vector3 playerRotation;

        public float boostTimer = 100.0f;
        private Boolean boostActive = false;

        private Boolean fireActive = false;

        Camera controlCamera;

        private Vector3 mouseRotBuffer;
        private MouseState curMouseState;
        private MouseState preMouseState;

        private Vector3 cameraOffset = new Vector3(0f, 1f, -6f);

        private float rotSpeed = 5f;
        public float playerSpeed = 10f;

        public Vector3 Position
        {
            get { return playerPosition; }
            set { playerPosition = value; }
        }

        public Vector3 Rotation
        {
            get { return playerRotation; }
            set { playerRotation = value; }
        }

        public Player(Game game, Camera camera, Vector3 spawnPosition, Vector3 spawnRotation)
            : base(game)
        {
            //playerSpeed = speed;
            controlCamera = camera;

            MoveTo(spawnPosition, spawnRotation);
        }

        private void MoveTo(Vector3 newPosition, Vector3 newRotation)
        {
            Position = newPosition;
            Rotation = newRotation;
        }

        private void UpdateLookAt()
        {
            //Matrix rotationMatrix = Matrix.CreateRotationX(playerPosition.X) * Matrix.CreateRotationY(playerRotation.Y);

            //Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            //cameraLookAt = playerPosition + lookAtOffset;

        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            //Matrix rotX = Matrix.CreateRotationX(playerPosition.X);Matrix rotY = Matrix.CreateRotationY(playerPosition.Y);Matrix rotZ = Matrix.CreateRotationZ(playerPosition.Z);
            //Matrix rotation = rotX * rotY * rotZ;

            Matrix rotation = Matrix.CreateFromYawPitchRoll(playerRotation.X, playerRotation.Y, playerRotation.Z);
            
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotation);

            return playerPosition + movement;
        }

        private Vector3 PreviewRotate(Vector3 amount)
        {
            //Matrix rotY = Matrix.CreateRotationY(playerPosition.Y);
            //Matrix rotX = Matrix.CreateRotationX(playerPosition.X);

            //Matrix rotation = /*rotX * */rotY;
            //movement = Vector3.Transform(movement, rotation);

            Vector3 rotation = new Vector3(amount.X, amount.Y, amount.Z);
            return playerRotation + rotation;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }
        private void Rotate(Vector3 scale)
        {
            MoveTo(Position, PreviewRotate(scale));
        }


        public void attachCamera()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(playerRotation.X, playerRotation.Y, playerRotation.Z);
            Vector3 offset = Vector3.Transform(cameraOffset, rotation);
            
            controlCamera.Position = playerPosition + offset;
            controlCamera.LookAt = playerPosition;
            //controlCamera.Rotation = playerRotation;
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            

            curMouseState = Mouse.GetState();

            KeyboardState ks = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;
            Vector3 rotateVector = Vector3.Zero;
            moveVector.Z = 1;
            if (ks.IsKeyDown(Keys.A))
            {
                rotateVector.Z = -1;
            }
            if (ks.IsKeyDown(Keys.D))
            {
                rotateVector.Z = 1;
            }
            if (ks.IsKeyDown(Keys.S))
            {
                //moveVector.Z = -1;
                //moveVector.Y = -1;
            }
            if (ks.IsKeyDown(Keys.W))
            {
                //moveVector.Z = 1;
                //moveVector.Y = 1;
            }

            if (ks.IsKeyDown(Keys.Space))
            {
                //moveVector.Y = 1;
                playerSpeed = 20f;
            }
            //if (ks.IsKeyUp(Keys.Space))
            //{
            //    //moveVector.Y = 1;
            //    playerSpeed = 10f;
            //}


            if (ks.IsKeyDown(Keys.LeftControl))
            {
                moveVector.Y = -1;
            }

            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
                moveVector *= delta * playerSpeed;
                Move(moveVector);
            }
            if (rotateVector != Vector3.Zero)
            {
                rotateVector.Normalize();
                rotateVector *= delta * rotSpeed;
                Rotate(rotateVector);
            }

            float deltaX;
            float deltaY;

            if (curMouseState != preMouseState)
            {
                deltaX = curMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = curMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                mouseRotBuffer.X -= 0.1f * deltaX * delta;
                mouseRotBuffer.Y -= 0.1f * deltaY * delta;

                /*if (mouseRotBuffer.Y < MathHelper.ToRadians(-75.0f))
                    mouseRotBuffer.Y = mouseRotBuffer.Y - (mouseRotBuffer.Y - MathHelper.ToRadians(-75.0f));
                if (mouseRotBuffer.Y > MathHelper.ToRadians(75.0f))
                    mouseRotBuffer.Y = mouseRotBuffer.Y - (mouseRotBuffer.Y - MathHelper.ToRadians(75.0f));*/

                playerRotation = new Vector3(mouseRotBuffer.X, -mouseRotBuffer.Y, playerRotation.Z);

                deltaX = 0;
                deltaY = 0;

            }
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            preMouseState = curMouseState;


            if (boostActive && boostTimer > 0)
            {
                playerSpeed = 20f;
                boostTimer--;
            }
            else
            { 
                playerSpeed = 10f; 
            }
            if (!boostActive)
            {
                boostTimer += 0.5f;
            }

            boostTimer = MathHelper.Clamp(boostTimer, -1.0f, 100.0f);

            Console.Write(boostTimer+" ");

            attachCamera();

        }

        public void Boost(Boolean active)
        {
            boostActive = active;
        }
        public void Fire(Boolean active)
        {
            fireActive = active;
        }
    }
}
