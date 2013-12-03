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
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        private Camera controlCamera;
        private Player controlPlayer;
        private Game1 theGame;

        private bool fullScreen = false;

        private GamePadState oldPadState;
        private GamePadState newPadState;

        private Vector3 mouseRotBuffer;
        private MouseState curMouseState;
        private MouseState preMouseState;

        private KeyboardState newKeyState;
        private KeyboardState oldKeyState;

        private float delta;

        public InputManager(Game game, Game1 game1, Camera camera, Player player)
            : base(game)
        {
            controlCamera = camera;
            controlPlayer = player;


            theGame = game1;
        }
        public override void Update(GameTime gameTime)
        {
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            newPadState = GamePad.GetState(PlayerIndex.One);
            controlPlayer.rotateVector = Vector3.Zero;

            KeyboardInput();

            MouseInput();

            if (newPadState.IsConnected)
                ControllerInput();
           
            base.Update(gameTime);
        }

        public void KeyboardInput()
        {
            newKeyState = Keyboard.GetState();
                        
            //rotateVector = Vector3.Zero;

            if (newKeyState.IsKeyDown(Keys.A))
            {
                controlPlayer.rotateVector.Z = -1;
            }
            if (newKeyState.IsKeyDown(Keys.D))
            {
                controlPlayer.rotateVector.Z = 1;
            }
            if (newKeyState.IsKeyDown(Keys.S))
            {
                controlPlayer.moveVector.Z = -1;
                controlPlayer.moveVector.Y = -1;
            }
            if (newKeyState.IsKeyDown(Keys.W))
            {
                controlPlayer.moveVector.Z = 1;
                controlPlayer.moveVector.Y = 1;
            }

            if (newKeyState.IsKeyDown(Keys.Space))
            {
                 controlPlayer.Boost(true);
            }
            if (newKeyState.IsKeyUp(Keys.Space) && oldKeyState.IsKeyDown(Keys.Space))
            {
                controlPlayer.Boost(false);
            }


            if (newKeyState.IsKeyDown(Keys.LeftControl))
            {
                controlPlayer.moveVector.Y = -1;
            }

            oldKeyState = newKeyState;
        }

        public void MouseInput()
        {
            curMouseState = Mouse.GetState();

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

                controlPlayer.Rotation = Quaternion.CreateFromYawPitchRoll(mouseRotBuffer.X, -mouseRotBuffer.Y, controlPlayer.Rotation.Z);
                //Quaternion tester1 = Quaternion.CreateFromAxisAngle(Vector3.UnitY, mouseRotBuffer.X);
                //Quaternion tester2 = Quaternion.CreateFromAxisAngle(Vector3.UnitX, mouseRotBuffer.Y);

                deltaX = 0;
                deltaY = 0;

            }
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            preMouseState = curMouseState;
        }

        public void ControllerInput()
        {
            controlPlayer.yaw = 2f;
            controlPlayer.pitch = 2f;
            controlPlayer.roll = 3f;

            if (newPadState.ThumbSticks.Left.Y > 0f)
            {
                controlPlayer.playerSpeed += (newPadState.ThumbSticks.Left.Y * 3f);
            }

            if (newPadState.ThumbSticks.Left.Y < 0f)
            {
                controlPlayer.playerSpeed += (newPadState.ThumbSticks.Left.Y * 3f);
            }

            if (newPadState.Triggers.Right > 0f)
            {
                theGame.Fire();
            }

            //if (newPadState.Triggers.Right == 0f && oldPadState.Triggers.Right > 0f)
            //{
            //    theGame.Fire(false);
            //}

            if (newPadState.Triggers.Left > 0f)
            {
                controlPlayer.Boost(true);
            }
            if (newPadState.Triggers.Left == 0f && oldPadState.Triggers.Left > 0f)
            {
                controlPlayer.Boost(false);
            }

            if (newPadState.Buttons.Start == ButtonState.Pressed && newPadState != oldPadState)
            {
                fullScreen = !fullScreen;
                theGame.ScreenSize(fullScreen);
            }

            if (newPadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                theGame.SetCamera(true);
            }
            if (newPadState.Buttons.LeftShoulder == ButtonState.Released && oldPadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                theGame.SetCamera(false);
            }
            /*------------------Roll------------------*/
            if (newPadState.ThumbSticks.Right.X > 0f)
            {
                controlPlayer.roll *= newPadState.ThumbSticks.Right.X;
                controlPlayer.rotateVector.Z = 1;
            }
            if (newPadState.ThumbSticks.Right.X < 0f)
            {
                controlPlayer.roll *= -newPadState.ThumbSticks.Right.X;
                controlPlayer.rotateVector.Z = -1;
            }
            /*------------------Roll------------------*/

            /*------------------Pitch-----------------*/
            if (newPadState.ThumbSticks.Left.X < 0f)
            {
                controlPlayer.pitch *= -newPadState.ThumbSticks.Left.X;
                controlPlayer.rotateVector.X = 1;
            }
            if (newPadState.ThumbSticks.Left.X > 0f)
            {
                controlPlayer.pitch *= newPadState.ThumbSticks.Left.X;
                controlPlayer.rotateVector.X = -1;
            }
            /*------------------Pitch-------------------*/

            /*-------------------Yaw------------------*/
            if (newPadState.ThumbSticks.Right.Y > 0f)
            {
                controlPlayer.yaw *= newPadState.ThumbSticks.Right.Y;
                controlPlayer.rotateVector.Y = 1;
            }
            if (newPadState.ThumbSticks.Right.Y < 0f)
            {
                controlPlayer.yaw *= -newPadState.ThumbSticks.Right.Y;
                controlPlayer.rotateVector.Y = -1;
            }
            /*-------------------Yaw------------------*/

            //Exit the Game
            if (newPadState.Buttons.Back == ButtonState.Pressed)
                theGame.Exit();



            oldPadState = newPadState;
        }
        
    }
}
