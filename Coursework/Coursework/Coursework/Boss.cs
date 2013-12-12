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

    public class Boss : Microsoft.Xna.Framework.GameComponent
    {
        private Vector3 bossPosition;           //Boss Position
        private Quaternion bossRotation;        //Boss Rotation
        private float bossScale;                //Boss Scale
        private float bossSpeed = 12f;          //Speed the Boss Moves
        private Game1 theGame;                  //Instance of the Game1 Class
        private Player targetPlayer;            //Instance of the Player Object

        public int health = 100;                    //Boss Health           
        private Quaternion gunRotation;             //Rotation of the Laser Canon
        private Vector3 gunPosition;                //Position of the Laser Canon
        private Vector3 canonOffset = new Vector3(-100,-50,0); //Offset for the Cannon
        private double lastLaserTime = 0;           //Time since the last laser was fired, zero by default

        //Public Enemy Position member, gets and sets the private member
        public Vector3 Position
        {
            get { return bossPosition; }
            set { bossPosition = value; }
        }

        //Public Enemy Rotation member, gets and sets the private member
        public Quaternion Rotation
        {
            get { return bossRotation; }
            set { bossRotation = value; }
        }
        //Public Enemy Boss member, gets and sets the private member
        public float Scale
        {
            get { return bossScale; }
            set { bossScale = value; }
        }


        public Boss(Game game, Game1 game1, Player player, Vector3 spawnPosition, Vector3 spawnRotation, float scale)
            : base(game)
        {
            /* Set the Parameters to their local counterparts */
            bossPosition = spawnPosition;
            bossRotation = Quaternion.CreateFromYawPitchRoll(spawnRotation.X, spawnRotation.Y, spawnRotation.Z);
            bossScale = scale;
            targetPlayer = player;
            theGame = game1;
            MediaPlayer.Play(theGame.bossMusic);
        }


        public override void Update(GameTime gameTime)
        {
            switch (theGame.gameState)
            {
                case Game1.GameState.MainMenu:
                    {
                        break;
                    }
                case Game1.GameState.Playing:
                    {
                        gunPosition = bossPosition + canonOffset;

                        Attack(gameTime);       //Call the Attack Method

                        if (health <= 0)
                        {
                            theGame.gameState = Game1.GameState.CompleteScreen;
                            MediaPlayer.Play(theGame.completedMusic);
                        }

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
            base.Update(gameTime);
        }

        public void Attack(GameTime gTime)
        {
            double currentTime = gTime.TotalGameTime.TotalMilliseconds;     //Create a variable to store the time in milliseconds
            gunRotation = Follow(gunPosition, targetPlayer.Position);   //Set the enemy rotation to the output of the Follow method

            /* This condition is based on the time between lasers being more than 1000 milliseconds */
            if (currentTime - lastLaserTime > 2000)
            {
                theGame.BossFire(gunPosition, gunRotation, true);   //Call the Fire method
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

        public void Damage(int damageDone)
        {
            health = health - damageDone;    //Damage Done
        }

    }
}
