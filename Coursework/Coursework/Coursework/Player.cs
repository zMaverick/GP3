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
        public int health = 100;
        public float boostTimer = 100.0f;
        private Boolean boostActive = false;
        Game1 theGame;
        Camera controlCamera;
        Camera secCamera;

        private Vector3 cameraOffset = new Vector3(0f, 1f, -6f);
        private Vector3 secCameraOffset = new Vector3(0f, 1f, 6f);
        
        public float yaw = 2f;
        public float pitch = 3f;
        public float roll = 5f;

        public float playerSpeed = 7f;
        Quaternion cameraRotation = Quaternion.Identity;

        AudioEmitter emitter = new AudioEmitter();
        public float speedSound = 0f;

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

        public Player(Game game, Game1 game1, Camera camera, Camera backCamera, Vector3 spawnPosition, Quaternion spawnRotation)
            : base(game)
        {
            theGame = game1;
            controlCamera = camera;
            secCamera = backCamera;
            MoveTo(spawnPosition, spawnRotation);
        }

        private void MoveTo(Vector3 newPosition, Quaternion newRotation)
        {
            Position = newPosition;
            Rotation = newRotation;
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
            Matrix rotation = Matrix.CreateFromQuaternion(cameraRotation);
            Matrix rot = Matrix.CreateFromQuaternion(playerRotation);

            Vector3 offset = Vector3.Transform(cameraOffset, rotation);
            Vector3 secOffset = Vector3.Transform(secCameraOffset, rotation);

            Vector3 lookatOffset = Vector3.Transform(new Vector3(0f, 0.8f, 0f), rotation);
            Vector3 secLookAtOffset = Vector3.Transform(new Vector3(0f, 0.8f, 0f), rot);

            controlCamera.Position = playerPosition + offset;
            controlCamera.LookAt = playerPosition + secLookAtOffset;
            controlCamera.Up = Vector3.Transform(Vector3.Up, playerRotation);

            secCamera.Position = playerPosition + secOffset;
            secCamera.LookAt = playerPosition + lookatOffset;
            secCamera.Up = Vector3.Transform(Vector3.Up, playerRotation);

            cameraRotation = Quaternion.Lerp(cameraRotation, playerRotation, 0.1f);
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            emitter.Position = playerPosition;

            Vector3 yawPitchRoll = new Vector3(pitch, yaw, roll);
            moveVector = Vector3.Zero;
            moveVector.Z = 1;

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
            
            if (boostActive && boostTimer > 0)
            {
                playerSpeed = 50f;
                boostTimer -= 0.5f;
                cameraOffset.Z -= 0.2f;
                theGame.playerSoundFX.Stop();
                theGame.playerBoostFX.Play();
            }
            else
            {
                cameraOffset.Z += 0.2f;
                theGame.playerBoostFX.Stop();
                theGame.playerSoundFX.Play();
                theGame.playerSoundFX.Pitch = speedSound / 8;
                playerSpeed = 10f; 
            }

            if (!boostActive)
            {
                boostTimer += 0.5f;
            }
            boostTimer = MathHelper.Clamp(boostTimer, -1.0f, 100.0f);

            cameraOffset.Z = MathHelper.Clamp(cameraOffset.Z, -8f, -6f);
            theGame.playerBoostFX.Apply3D(theGame.listener, emitter);
            theGame.playerSoundFX.Apply3D(theGame.listener, emitter);

           attachCamera();
           theGame.playerExplodeFX.Apply3D(theGame.listener, emitter);

           if (health <= 0)
           {
               Destroy();
           }
        }

        public void Damage()
        {
            health = health - 5;
        }

        public void Boost(Boolean active)
        {
            boostActive = active;
        }

        public void Destroy()
        {
            theGame.playerExplodeFX.Play();

            playerPosition = new Vector3(0f, 200f, 0f);
            playerRotation = Quaternion.Identity;

            controlCamera.Position = playerPosition + cameraOffset;
            secCamera.Position = playerPosition + secCameraOffset;
            
            health = 100;
            boostTimer = 100;
        }

    }
}
