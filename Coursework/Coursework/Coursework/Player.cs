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
        private Vector3 playerPosition;                                 //Player Position
        private Quaternion playerRotation;                              //Player Rotation
        private Boolean boostActive = false;                            //Is the player boosting
        private Game1 theGame;                                          //Game Object
        private Camera controlCamera;                                   //Main Camera Object
        private Camera secCamera;                                       //Secondary Camera Object
        private Vector3 cameraOffset = new Vector3(0f, 1f, -6f);        //Offset Vector for the Main Camera
        private Vector3 secCameraOffset = new Vector3(0f, 1f, 6f);      //Offset Vector for the Secondary Camera
        private Quaternion cameraRotation = Quaternion.Identity;        //Camera Rotation
        private AudioEmitter emitter = new AudioEmitter();              //Audio Emitter for Sounds
        private bool vibrate = false;                                   //Is the controller vibrating
        private int vibCounter = 0;                                        //vibration time

        public float yaw = 2f;                  //Yaw rotation Speed
        public float pitch = 3f;                //Pitch rotation Speed
        public float roll = 5f;                 //Roll rotation Speed
        public Vector3 moveVector;              //Normalized Vector for movement
        public Vector3 rotateVector;            //Normalized Vector for rotations
        public int health = 100;                //Player Health
        public float boostTimer = 100.0f;       //Timer to count how long the player has been boosting
        public float playerSpeed = 7f;          //Speed the Player Moves
        public float speedSound = 0f;           //Used to change the pitch of the engine sound based on the speed

        //Public Player Position member, gets and sets the private member
        public Vector3 Position
        {
            get { return playerPosition; }
            set { playerPosition = value; }
        }

        //Public Player Position member, gets and sets the private member
        public Quaternion Rotation
        {
            get { return playerRotation; }
            set { playerRotation = value; }
        }

        public Player(Game game, Game1 game1, Camera camera, Camera backCamera, Vector3 spawnPosition, Quaternion spawnRotation)
            : base(game)
        {
            /* Set the Parameters to their local counterparts */
            theGame = game1;
            controlCamera = camera;
            secCamera = backCamera;

            MoveTo(spawnPosition, spawnRotation);   //Call the MoveTo method
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;     //Create deltaTime (time since last frame update)
            switch (theGame.gameState)
            {
                case Game1.GameState.MainMenu:
                    {
                        theGame.playerSoundFX.Stop();       //Stop the player movement Sound FX
                        break;
                    }
                case Game1.GameState.Playing:
                    {
                        #region Game Update
                        
                        emitter.Position = playerPosition;                              //Update the emitter position to the projectile position

                        Vector3 yawPitchRoll = new Vector3(pitch, yaw, roll);           //Create the yawPitchRoll variable from the floats yaw, pitch and roll
                        moveVector = Vector3.Zero;                                      //Set the moveVector to zero per frame
                        moveVector.Z = 1;                                               //Set the moveVector Z variable to one per frame, so the player is always moving forward


                        /* On the condition the player is moving */
                        if (moveVector != Vector3.Zero)
                        {
                            moveVector.Normalize();                 //Normalize the moveVector so the applied speed is equally distributed
                            moveVector *= delta * playerSpeed;      //Apply the speed variable (multipled by delta, for consistency in systems) to the move Vector
                            Move(moveVector);                       //Call the Move method
                        }
                        /* On the condition the player is rotating */
                        if (rotateVector != Vector3.Zero)
                        {
                            rotateVector.Normalize();               //Normalize the rotateVector so the applied speed is equally distributed
                            rotateVector *= delta * yawPitchRoll;   //Apply the yaw pitch and roll variables (multipled by delta, for consistency in systems) to the rotate Vectors
                            Rotate(rotateVector);                   //Call the Rotate method
                        }

                        /* On the condition the player is boosting and there is boost available */
                        if (boostActive && boostTimer > 0)
                        {
                            GamePad.SetVibration(PlayerIndex.One, 0, 0.1f);  //Vibrate
                            playerSpeed = 50f;                  //The playerSpeed is 50
                            boostTimer -= 0.5f;                 //The boost available reduces 0.5 per frame
                            cameraOffset.Z -= 0.2f;             //The camera will retract
                            theGame.playerSoundFX.Stop();       //Stop the player movement Sound FX
                            theGame.playerBoostFX.Play();       //Play the player boost Sound FX
                        }
                        else
                        {
                            GamePad.SetVibration(PlayerIndex.One, 0, 0);    //Vibrate
                            cameraOffset.Z += 0.2f;                         //The camera will extend
                            theGame.playerBoostFX.Stop();                   //Stop the player boost Sound FX
                            theGame.playerSoundFX.Play();                   //Play the player movement Sound FX
                            theGame.playerSoundFX.Pitch = speedSound / 8;   //Apply the speedSound to the pitch (over 8), to create the speed based pitch noise
                            playerSpeed = 10f;                              //Reset the player speed to 10
                        }

                        if (!boostActive)
                        {
                            //If the player has stopped trying to Boost - Add 0.5 per frame
                            boostTimer += 0.5f;
                        }

                        boostTimer = MathHelper.Clamp(boostTimer, -1.0f, 100.0f);       //Clamp the boostTimer so it never goes above 100 and below -1
                        cameraOffset.Z = MathHelper.Clamp(cameraOffset.Z, -8f, -6f);    //Clamp the Z-Axis of cameraOffset to keep it attached within reason
                        theGame.playerBoostFX.Apply3D(theGame.listener, emitter);       //Apply this instance to the Game Listener (the player), from the emitter
                        theGame.playerSoundFX.Apply3D(theGame.listener, emitter);       //Apply this instance to the Game Listener (the player), from the emitter
                        theGame.playerExplodeFX.Apply3D(theGame.listener, emitter);     //Apply this instance to the Game Listener (the player), from the emitter

                        if (health <= 0)
                        {
                            Destroy();   //Call the Destroy method if the heath reaches zero
                        }

                        if (vibrate)
                        {
                            vibCounter++;
                            if (vibCounter * delta < 1)
                            {
                                GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
                            }
                            else
                            {
                                vibCounter = 0;
                                vibrate = false;
                            }
                        }
                        else
                        {
                            GamePad.SetVibration(PlayerIndex.One, 0, 0);
                        }

                        AttachCamera();  //Attach the Camera once per frame
                        #endregion
                        break;
                    }
                case Game1.GameState.ControlsMenu:
                    {
                        theGame.playerSoundFX.Stop();       //Stop the player movement Sound FX
                        break;
                    }
                case Game1.GameState.PauseMenu:
                    {
                        theGame.playerSoundFX.Stop();       //Stop the player movement Sound FX
                        break;
                    }
                case Game1.GameState.CompleteScreen:
                    {
                        theGame.playerSoundFX.Stop();       //Stop the player movement Sound FX
                        break;
                    }
            }


        }

        private void MoveTo(Vector3 newPosition, Quaternion newRotation)
        {
            Position = newPosition; //Set the Position to the new Position
            Rotation = newRotation; //Set the Rotation to the new Rotation
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotation = Matrix.CreateFromQuaternion(playerRotation);      //Create a Matrix from the Enemy Rotation
            Vector3 movement = amount;                                          //Create a Vector from the amount parameter
            movement = Vector3.Transform(movement, rotation);                   //Transform the movement from the rotation

            return playerPosition + movement;       //Return the transformed movement by the position
        }

        private Quaternion PreviewRotate(Vector3 amount)
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(amount.X, amount.Y, amount.Z);  //Rotation Created from Vector Amount
            return playerRotation * rotation;      //Return the player rotation by the new rotation
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);   //Call the MoveTo Method
        }

        private void Rotate(Vector3 scale)
        {
            MoveTo(Position, PreviewRotate(scale));     //Call the MoveTo Method
        }

        public void AttachCamera()
        {
            Matrix cameraRot = Matrix.CreateFromQuaternion(cameraRotation);                     //Create a rotation Matrix for the Camera
            Matrix playerRot = Matrix.CreateFromQuaternion(playerRotation);                     //Create a rotation Matrix for the Player
            Vector3 offset = Vector3.Transform(cameraOffset, cameraRot);                        //Transform the Main Camera Offset by the new Camera Matrix so that the offset value is affected by the rotation
            Vector3 secOffset = Vector3.Transform(secCameraOffset, cameraRot);                  //Transform the Secondary Camera Offset by the new Camera Matrix so that the offset value is affected by the rotation
            Vector3 lookatOffset = Vector3.Transform(new Vector3(0f, 0.8f, 0f), cameraRot);     //Create a Look At Offset for the Main Camera and Transform it by the camera rotation, so that the camera is not looking directly at the player
            Vector3 secLookAtOffset = Vector3.Transform(new Vector3(0f, 0.8f, 0f), playerRot);  //Create a Look At Offset for the Secondary Camera and Transform it by the player rotation, so that the camera is not looking directly at the player

            controlCamera.Position = playerPosition + offset;                   //Add the offset to the player position to create the main camera position
            controlCamera.LookAt = playerPosition + lookatOffset;               //Add the LookAt Offset to the Player Position to create a lookat just above the player
            controlCamera.Up = Vector3.Transform(Vector3.Up, playerRotation);   //Transform the Up Vector by the player rotation so the camera rotates with the player
            secCamera.Position = playerPosition + secOffset;                    //Add the offset to the player position to create the secondary camera position
            secCamera.LookAt = playerPosition + secLookAtOffset;                //Add the LookAt Offset to the Player Position to create a lookat just above the player
            secCamera.Up = Vector3.Transform(Vector3.Up, playerRotation);       //Transform the Up Vector by the player rotation so the camera rotates with the player

            cameraRotation = Quaternion.Lerp(cameraRotation, playerRotation, 0.1f);     //Lerp the Camera Rotation by the Player Rotation to add a slight delay to the camera to make it less twitchy
        }

        public void Damage(int damageDone)
        {
            health = health - damageDone;    //Damage Done
            vibrate = true;                 //Vibrate
        }

        public void Boost(Boolean active)
        {
            boostActive = active;       //Set the Boost to the active Parameter
        }

        public void Destroy()
        {
            theGame.playerExplodeFX.Play();                             //play the player explode Sound FX
            vibrate = true;                                             //Vibrate
            playerPosition = new Vector3(0f, 200f, 0f);                 //Reset the Player Position
            playerRotation = Quaternion.Identity;                       //Reset the Player Rotation
            controlCamera.Position = playerPosition + cameraOffset;     //Reset the Main Camera Position
            controlCamera.LookAt = playerPosition;                      //Reset the Main Camera LookAt
            secCamera.Position = playerPosition + secCameraOffset;      //Reset the Secondary Camera Position
            secCamera.LookAt = playerPosition;                          //Reset the Secondary Camera LookAt
            health = 100;                                               //Reset the Health
            boostTimer = 100;                                           //Reset the Boost Timer
        }

    }
}


