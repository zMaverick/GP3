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
    public class Projectile : Microsoft.Xna.Framework.GameComponent
    {

        private Vector3 projectilePosition;
        private Vector3 direction;
        public float speed;
        public bool isActive;
        Player cPlayer;
        Vector3 offset = new Vector3(0f, 0f, 1f);

        public Vector3 Position
        {
            get { return projectilePosition; }
            set { projectilePosition = value; }
        }

        public Vector3 Rotation
        {
            get { return direction; }
            set { direction = value; }
        }


        public Projectile(Game game, Player player)
            : base(game)
        {
            cPlayer = player;
        }

        public override void Update(GameTime gameTime)
        {
            projectilePosition = cPlayer.Position + offset;
            //direction = cPlayer.Rotation;
            //position += direction * speed *
            //GameConstants.LaserSpeedAdjustment * delta;
            //if (position.X > GameConstants.PlayfieldSizeX ||
            //    position.X < -GameConstants.PlayfieldSizeX ||
            //    position.Z > GameConstants.PlayfieldSizeZ ||
            //    position.Z < -GameConstants.PlayfieldSizeZ)
            //    isActive = false;
            base.Update(gameTime);
        }
    }
}
