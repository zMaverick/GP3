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
        private Vector3 enemyPosition;
        private Quaternion enemyRotation;

        private float playerDistance;
        private float attackDistance = 100f;

        Game1 theGame;

        private Vector3 moveVector;
        private Vector3 rotateVector;

        private float enemySpeed = 12f;
        private Player targetPlayer;

        public bool isActive = true;

        double lastLaserTime = 0;

        public Vector3 Position
        {
            get { return enemyPosition; }
            set { enemyPosition = value; }
        }

        public Quaternion Rotation
        {
            get { return enemyRotation; }
            set { enemyRotation = value; }
        }

        public Enemy(Game game, Game1 game1, Vector3 spawnPosition, Quaternion spawnRotation, Player player)
            : base(game)
        {
            enemyPosition = spawnPosition;
            enemyRotation = spawnRotation;
            targetPlayer = player;
            theGame = game1;
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerDistance = Vector3.Distance(enemyPosition, targetPlayer.Position);

                if (playerDistance <= attackDistance)
                {
                    Attack(gameTime);
                    moveVector.Z = 1;
                    attackDistance = 200f;
                }
                if (playerDistance >= attackDistance)
                {
                    attackDistance = 100f;
                }

                if (moveVector != Vector3.Zero)
                {
                    moveVector.Normalize();
                    moveVector *= delta * enemySpeed;
                    Move(moveVector);
                }

                base.Update(gameTime);
            }
        }

        public void Attack(GameTime gTime)
        {
            double currentTime = gTime.TotalGameTime.TotalMilliseconds;

            enemyRotation = Follow(enemyPosition, targetPlayer.Position);

            if (currentTime - lastLaserTime > 1000)
            {
                theGame.EnemyFire(enemyPosition, enemyRotation);
                lastLaserTime = currentTime;
            }
        }

        public Quaternion Follow(Vector3 position, Vector3 lookat)
        {
            Matrix rotation = new Matrix();
            Matrix test1 = Matrix.CreateFromQuaternion(targetPlayer.Rotation);

            rotation.Forward = -Vector3.Normalize(lookat - position);
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, test1.Up));
            rotation.Up = Vector3.Normalize(Vector3.Cross(rotation.Right, rotation.Forward));

            Quaternion newRot = Quaternion.CreateFromRotationMatrix(rotation);

            return newRot;
        }

        private void MoveTo(Vector3 newPosition, Quaternion newRotation)
        {
            Position = newPosition;
            Rotation = newRotation;
        }

        private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotation = Matrix.CreateFromQuaternion(enemyRotation);

            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotation);

            return enemyPosition + movement;
        }

        private Quaternion PreviewRotate(Vector3 amount)
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(amount.X, amount.Y, amount.Z);
            return enemyRotation * rotation;
        }

        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }
        private void Rotate(Vector3 scale)
        {
            MoveTo(Position, PreviewRotate(scale));
        }

    }
}
