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

        private Vector3 projPosition;
        private Quaternion projRotation;
        public float projSpeed;
        public bool isActive = true;
        //Player cPlayer;
        Vector3 offset;
        Game1 theGame;
      
        //private Boolean fireActive = false;

        AudioEmitter emitter = new AudioEmitter();
        
        public Vector3 Position
        {
            get { return projPosition; }
            set { projPosition = value; }
        }

        public Quaternion Rotation
        {
            get { return projRotation; }
            set { projRotation = value; }
        }


        public Projectile(Game1 game1, Vector3 position, Quaternion rotation, float speed, bool pos, bool isPlayer)
        {
            theGame = game1;
            Load();
            if (isPlayer)
            {
                if (pos)
                {
                    offset = new Vector3(1.5f, 0f, 1f);
                }
                else
                {
                    offset = new Vector3(-1.5f, 0f, 1f);
                }

                theGame.playerFireFX.Play();
            }
            else
            {
                if (pos)
                {
                    offset = new Vector3(0.9f, 0f, 1f);
                }
                else
                {
                    offset = new Vector3(-0.9f, 0f, 1f);
                }

                theGame.enemyFireFX.Play();
            }
            Matrix playerRotation = Matrix.CreateFromQuaternion(rotation);
            Vector3 offset1 = Vector3.Transform(offset, rotation);

            projPosition = position + offset1;
            projRotation = rotation;
            projSpeed = speed;
        }

        public void Load()
        {
            theGame.playerFireFX = theGame.playerFire.CreateInstance();
            theGame.playerFireFX.Apply3D(theGame.listener, emitter);

            theGame.enemyFireFX = theGame.enemyFire.CreateInstance();
            theGame.enemyFireFX.Apply3D(theGame.listener, emitter);
        }

        public void UpdateLaser(GameTime gameTime)
        {
            if (isActive)
            {
                emitter.Position = projPosition;
                Vector3 test = Vector3.Transform(new Vector3(0, 0, 1), projRotation);
                projPosition += test * projSpeed;

                theGame.enemyFireFX.Apply3D(theGame.listener, emitter);
                theGame.playerFireFX.Apply3D(theGame.listener, emitter);
            }
        }
    }
}
