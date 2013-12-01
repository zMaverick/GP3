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
        Game theGame;

        GamePadState oldPadState;
        GamePadState newPadState;

        KeyboardState newKeyState;
        KeyboardState oldKeyState;  

        public InputManager(Game game, Camera camera, Player player)
            : base(game)
        {
            controlCamera = camera;
            controlPlayer = player;
            theGame = game;
        }
        public override void Update(GameTime gameTime)
        {
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

        }

        public void ControllerInput()
        {
            controlPlayer.yaw = 2f;
            controlPlayer.pitch = 1f;
            controlPlayer.roll = 3f;
            controlPlayer.playerSpeed = 7f;

            if (newPadState.Triggers.Left > 0f)
            {
                controlPlayer.Boost(true);
            }
            if (newPadState.Triggers.Left == 0f && oldPadState.Triggers.Left > 0f)
            {
                controlPlayer.Boost(false);
            }

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
                controlPlayer.Fire(true);
            }
            if (newPadState.Triggers.Right == 0f && oldPadState.Triggers.Right > 0f)
            {
                controlPlayer.Fire(false);
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
