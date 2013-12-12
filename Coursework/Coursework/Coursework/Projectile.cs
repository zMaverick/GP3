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
    public class Projectile
    {

        private Vector3 projPosition;                       //Projectile Position
        private Quaternion projRotation;                    //Projectile Rotation
        private float projScale;                            //Projectile Scale
        public float projSpeed;                             //Speed the Projectile Moves
        public bool isActive = true;                        //Is this Projectile is Active
        private bool bossSpawned = false;                   //has the boss spawned
        private bool isPlayer;                              //Is the Player creating this Projectile
        private Vector3 offset;                             //Offset used to spawn the Projectile at different Guns of the Ships
        private Game1 theGame;                              //Instance of the Game1 Class
        private AudioEmitter emitter = new AudioEmitter();  //Audio Emitter for Sounds

        //Public Projectile Position member, gets and sets the private member
        public Vector3 Position
        {
            get { return projPosition; }
            set { projPosition = value; }
        }

        //Public Projectile Rotation member, gets and sets the private member
        public Quaternion Rotation
        {
            get { return projRotation; }
            set { projRotation = value; }
        }
        //Public Projectile Scale member, gets and sets the private member
        public float Scale
        {
            get { return projScale; }
            set { projScale = value; }
        }
        public Projectile(Game1 game1, Vector3 position, Quaternion rotation, float speed, bool pos, bool player, bool boss)
        {
            theGame = game1;    //Set the Game1 Instance to the Passed in varient
            isPlayer = player;  //Set the isPlayer boolean
            bossSpawned = boss;     //is the boss creating this
            Load();     //Call the Load Method

            /* if boss is firing */
            if (boss)
            {
                offset = new Vector3(0, 0, 0);
                projScale = 50f;
                theGame.bossFireFX.Play();
            }
            else
            {
                projScale = 1f;
                /* This condition is based on the boolean player, which is used to determine whether the firing ship is the Player or an Enemy Ship as the Offset variables differ */
                if (player)
                {
                    /* This condition is based on the boolean pos which is changed every time a new projectile is created by the host (player or enemy), it alternates the position of the offset vector to simulate firing out of each gun alternately */
                    if (pos)
                    {
                        //set the offset
                        offset = new Vector3(1.5f, 0f, 1f);
                    }
                    else
                    {
                        //set the offset
                        offset = new Vector3(-1.5f, 0f, 1f);
                    }
                    //Play the fire SoundFX
                    theGame.playerFireFX.Play();
                }
                else
                {
                    if (pos)
                    {
                        //set the offset
                        offset = new Vector3(0.9f, 0f, 1f);
                    }
                    else
                    {
                        //set the offset
                        offset = new Vector3(-0.9f, 0f, 1f);
                    }
                    //Play the fire SoundFX
                    theGame.enemyFireFX.Play();
                }
            }
            Vector3 newOffset = Vector3.Transform(offset, rotation);        //Transform the offset vector by the rotation parameter

            projPosition = position + newOffset;        //Set the start position to the position parameter and the new offset transform vector
            projRotation = rotation;                    //Set the start rotation to the rotation parameter
            projSpeed = speed;                          //Set the Projectile speed to the input speed parameter
        }

        public void Load()
        {

            if (bossSpawned)
            {
                theGame.bossFireFX = theGame.bossFire.CreateInstance();
                theGame.bossFireFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
            }
            else
            {
                /* This condition is based on the boolean isPlayer, which is used to determine whether the firing ship is the Player or an Enemy Ship as the loaded Sound FX differs */
                if (isPlayer)
                {
                    theGame.playerFireFX = theGame.playerFire.CreateInstance(); //Create an instance of the Sound FX (For every projectile created)
                    theGame.playerFireFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
                }
                else
                {
                    theGame.enemyFireFX = theGame.enemyFire.CreateInstance();  //Create an instance of the Sound FX (For every projectile created)
                    theGame.enemyFireFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
                }
            }
            
        }

        public void UpdateLaser(GameTime gameTime)
        {
            //If the Projectile is active and not destroyed upon a collision
            if (isActive)
            {
                emitter.Position = projPosition;                //Update the emitter position to the projectile position
                Vector3 direction = Vector3.Transform(Vector3.UnitZ, projRotation);   //Transform  the forward direction (Z-Axis) by the rotation
                projPosition += direction * projSpeed;          //apply the speed of the projectile to the direction and add to the current projectile position

                /* This condition is based on the boolean isPlayer, which is used to determine whether the firing ship is the Player or an Enemy Ship as the loaded Sound FX differs */
                
                if (bossSpawned)
                {
                    theGame.bossFireFX.Apply3D(theGame.listener, emitter);    //Apply this instance to the Game Listener (the player), from the emitter
                }
                else
                {
                    if (isPlayer)
                    {
                        //Apply this instance to the Game Listener (the player), from the emitter
                        theGame.playerFireFX.Apply3D(theGame.listener, emitter);
                    }
                    else
                    {
                        //Apply this instance to the Game Listener (the player), from the emitter
                        theGame.enemyFireFX.Apply3D(theGame.listener, emitter);
                    }

                }
            }
        }
    }
}
