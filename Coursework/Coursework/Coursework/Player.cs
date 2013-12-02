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
        private Quaternion playerRotation;

        public Vector3 moveVector;
        public Vector3 rotateVector;

        public float boostTimer = 100.0f;
        private Boolean boostActive = false;

        private Boolean fireActive = false;

        Camera controlCamera;

        private Vector3 mouseRotBuffer;
        private MouseState curMouseState;
        private MouseState preMouseState;

        private Vector3 cameraOffset = new Vector3(0f, 1f, -6f);

        public float yaw = 2f;
        public float pitch = 3f;
        public float roll = 5f;

        public float playerSpeed = 7f;

        public Vector3 Position
        {
            get { return playerPosition; }
            set { playerPosition = value; }
        }

        public Quaternion Rotation
        {
            get { return playerRotation; }
            set { playerRotation = value; }
        }

        public Player(Game game, Camera camera, Vector3 spawnPosition, Quaternion spawnRotation)
            : base(game)
        {
            controlCamera = camera;
            MoveTo(spawnPosition, spawnRotation);
        }

        private void MoveTo(Vector3 newPosition, Quaternion newRotation)
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
            Matrix rotation = Matrix.CreateFromQuaternion(playerRotation);

            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotation);

            return playerPosition + movement;
        }

        private Quaternion PreviewRotate(Vector3 amount)
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(amount.X, amount.Y, amount.Z);
            return playerRotation * rotation;
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
            Matrix rotation = Matrix.CreateFromQuaternion(playerRotation);
            Vector3 offset = Vector3.Transform(cameraOffset, rotation);
            
            controlCamera.Position = playerPosition + offset;
            controlCamera.LookAt = playerPosition;
            controlCamera.Up = Vector3.Transform(Vector3.Up, playerRotation);
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 yawPitchRoll = new Vector3(pitch, yaw, roll);
            moveVector = Vector3.Zero;
            moveVector.Z = 1;

            curMouseState = Mouse.GetState();

            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
                moveVector *= delta * playerSpeed;
                Move(moveVector);
            }
            if (rotateVector != Vector3.Zero)
            {
                rotateVector.Normalize();
                rotateVector *= delta * yawPitchRoll;
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

                /////////////////////////CREATE FROM AXIS ANGLE\\\\\\\\\\\\\\\\\\\\\\\\\
                playerRotation = Quaternion.CreateFromYawPitchRoll(mouseRotBuffer.X, -mouseRotBuffer.Y, playerRotation.Z);

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
