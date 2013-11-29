using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Coursework
{
    struct Daleks
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isActive;

        public void Update(float delta)
        {
            position += direction * speed *
                        GameConstants.DalekSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX)
                position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX)
                position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Z > GameConstants.PlayfieldSizeZ)
                position.Z -= 2 * GameConstants.PlayfieldSizeZ;
            if (position.Z < -GameConstants.PlayfieldSizeZ)
                position.Z += 2 * GameConstants.PlayfieldSizeZ;
        }
    }
}
