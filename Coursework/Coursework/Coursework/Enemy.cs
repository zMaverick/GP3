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
    public class Enemy : Microsoft.Xna.Framework.GameComponent
    {
        private Vector3 enemyPosition;                      //Enemy Position
        private Quaternion enemyRotation;                   //Enemy Rotation
        private float enemySpeed = 12f;                     //Speed the Enemy Moves
        private Game1 theGame;                              //Instance of the Game1 Class
        private Player targetPlayer;                        //Instance of the Player Object
        private Vector3 moveVector;                         //Normalized Vector for movement
        private float playerDistance;                       //The current distance of the player from the enemy
        private float attackDistance = 100f;                //The initial distance the player must be within to attack
        private bool firePos = true;                        //The Fire position, used to determine which gun the laser is fired from
        private double lastLaserTime = 0;                   //Time since the last laser was fired, zero by default

        public bool isActive = true;                        //Is this Enemy is Active
        public AudioEmitter emitter = new AudioEmitter();   //Audio Emitter for Sounds

        //Public Enemy Position member, gets and sets the private member
        public Vector3 Position
        {
            get { return enemyPosition; }
            set { enemyPosition = value; }
        }

        //Public Enemy Rotation member, gets and sets the private member
        public Quaternion Rotation
        {
            get { return enemyRotation; }
            set { enemyRotation = value; }
        }

        public Enemy(Game game, Game1 game1, Vector3 spawnPosition, Quaternion spawnRotation, Player player)
            : base(game)
        {
            /* Set the Parameters to their local counterparts */
            enemyPosition = spawnPosition;
            enemyRotation = spawnRotation;
            targetPlayer = player;
            theGame = game1;

            LoadSounds();   //Call the load sounds method
        }


        public void LoadSounds()
        {
            theGame.enemySound = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\enemy-move.wav"));     //This is loaded here to avoid null reference exceptions caused by the load content running after the intilize method
            theGame.enemySoundFX = theGame.enemySound.CreateInstance(); //Create an instance of the Sound FX (For every Enemy created)
            theGame.enemySoundFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
        }

        public override void Update(GameTime gameTime)
        {
            //If this Enemy is active and not destroyed already
            if (isActive)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;     //Create deltaTime (time since last frame update)
                switch (theGame.gameState)
                {
                    case Game1.GameState.MainMenu:
                        {
                            break;
                        }
                    case Game1.GameState.Playing:
                        {
                            #region Game Update
                            emitter.Position = enemyPosition;                               //Update the emitter position to the projectile position
                
                            playerDistance = Vector3.Distance(enemyPosition, targetPlayer.Position);    //Calculate the distance between the enemy position and the player position)

                            /* This condition is based on the player being within the attack distance */
                            if (playerDistance <= attackDistance)
                            {
                                theGame.enemySoundFX.Play();    //play the enemy movement Sound FX
                                theGame.enemySoundFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
                    
                                Attack(gameTime);       //Call the Attack Method
                                moveVector.Z = 1;       //Move the Enemy
                                attackDistance = 200f;  //Set a new attack Distance, so the enemy follows for longer
                            }
                            else if (playerDistance >= attackDistance)
                            {
                                attackDistance = 100f;  //Reset the attack distance to default
                            }

                            /* This condition is based on the enemy moving */
                            if (moveVector != Vector3.Zero)
                            {
                                moveVector.Normalize();             //Normalize the rotateVector so the applied speed is equally distributed
                                moveVector *= delta * enemySpeed;   //Apply the speed variable (multipled by delta, for consistency in systems) to the rotate Vector
                                Move(moveVector);                   //Call the Move method
                            }
                            #endregion
                            break;
                        }
                    case Game1.GameState.ControlsMenu:
                        {
                            break;
                        }
                    case Game1.GameState.PauseMenu:
                        {
                            break;
                        }
                    case Game1.GameState.CompleteScreen:
                        {
                            break;
                        }
                }

            }
            base.Update(gameTime);
        }

        public void Attack(GameTime gTime)
        {
            double currentTime = gTime.TotalGameTime.TotalMilliseconds;     //Create a variable to store the time in milliseconds
            enemyRotation = Follow(enemyPosition, targetPlayer.Position);   //Set the enemy rotation to the output of the Follow method

            /* This condition is based on the time between lasers being more than 1000 milliseconds */
            if (currentTime - lastLaserTime > 1000)
            {
                firePos = !firePos;                 //Invert the firePos Boolean
                theGame.EnemyFire(enemyPosition, enemyRotation, firePos);   //Call the Fire method
                lastLaserTime = currentTime;        //Set the last laser time to the current
            }
        }

        public Quaternion Follow(Vector3 position, Vector3 lookat)
        {
            Matrix rotation = new Matrix();     //Create a new rotation Matrix
            Matrix playerRot = Matrix.CreateFromQuaternion(targetPlayer.Rotation);  //Create Rotation Matrix for the player

            rotation.Forward = -Vector3.Normalize(lookat - position);               //The new Rotation Matrix's Forward is set to a normalized inverted Vector created from the difference between the lookat and positon parameters
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, playerRot.Up));  //The new Rotation Matrix's Right is set to a normalized cross product of the Rotation's Forward and Player Matrix's Up
            rotation.Up = Vector3.Normalize(Vector3.Cross(rotation.Right, rotation.Forward));   //The new Rotation Matrix's Up is set to a normalized cross product of the right and up

            Quaternion newRot = Quaternion.CreateFromRotationMatrix(rotation);  //Create a new Quaternion rotation from the new rotation Matrix

            return newRot;      //Return the new Rotation
        }

        private void MoveTo(Vector3 newPosition, Quaternion newRotation)
        {
            Position = newPosition; //Set the Position to the new Position
            Rotation = newRotation; //Set the Rotation to the new Rotation
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotation = Matrix.CreateFromQuaternion(enemyRotation);   //Create a Matrix from the Enemy Rotation
            Vector3 movement = amount;                                      //Create a Vector from the amount parameter
            movement = Vector3.Transform(movement, rotation);               //Transform the movement from the rotation

            return enemyPosition + movement;    //Return the transformed movement by the position
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);   //Call the MoveTo Method
        }

    }
}
