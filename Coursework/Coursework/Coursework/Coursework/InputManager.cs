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
        GamePadState oldPadState;
        GamePadState newPadState;
        KeyboardState keyboard;
        Game theGame;

        public InputManager(Game game, Camera camera, Player player)
            : base(game)
        {
            controlCamera = camera;
            controlPlayer = player;
            theGame = game;
        }
        public override void Update(GameTime gameTime)
        {
            keyboard = Keyboard.GetState();
            newPadState = GamePad.GetState(PlayerIndex.One);

            if (newPadState.IsConnected)
            {
                if (newPadState.Triggers.Left > 0f)
                {
                    controlPlayer.Boost(true);
                }
                if (newPadState.Triggers.Left == 0f && oldPadState.Triggers.Left > 0f)
                {
                    controlPlayer.Boost(false);
                }
                if (newPadState.Triggers.Right > 0f)
                {
                    controlPlayer.Fire(true);
                }
                if (newPadState.Triggers.Right == 0f && oldPadState.Triggers.Right > 0f)
                {
                    controlPlayer.Fire(false);
                }

                if (newPadState.ThumbSticks.Left.X > 0f)
                {
                    controlPlayer.Rotation = new Vector3(controlPlayer.Rotation.X * newPadState.ThumbSticks.Left.X, controlPlayer.Rotation.Y, controlPlayer.Rotation.Z);
                }
                //Exit the Game
                if (newPadState.Buttons.Back == ButtonState.Pressed)
                    theGame.Exit();
            }

            oldPadState = newPadState;

            base.Update(gameTime);
        }
    }
}
