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
        private Player controlPlayer;           //Instance of the Player
        private Game1 theGame;                  //Instance of the Game

        private bool fullScreen = false;        //Boolean for switching to Fullscreen
        private bool soundOn = false;           //Boolean for switching sound on and off

        private GamePadState newPadState;       //The current frame pad state
        private GamePadState oldPadState;       //The previous frame pad state
        
        private Vector3 mouseRotBuffer;         //Rotation Buffer
        private MouseState curMouseState;       //The current frame mouse state
        private MouseState preMouseState;       //The current frame mouse state

        private KeyboardState newKeyState;      //The current frame keyboard state
        private KeyboardState oldKeyState;      //The current frame keyboard state

        private float delta;                    //Hold the time since the last update

        public InputManager(Game game, Game1 game1, Player player)
            : base(game)
        {
            //Set the Instances upon Initialize
            controlPlayer = player;
            theGame = game1;
        }

        public override void Update(GameTime gameTime)
        {
            //Set the delta variable
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Set the current pad state
            newPadState = GamePad.GetState(PlayerIndex.One);
            newKeyState = Keyboard.GetState();

            switch (theGame.gameState)
            {
                case Game1.GameState.MainMenu:
                    {

                        if (newKeyState.IsKeyDown(Keys.Enter) && oldKeyState != newKeyState)
                        {
                            theGame.gameState = Game1.GameState.Playing;        //Play the Game
                            MediaPlayer.Play(theGame.backgroundMusic);          //Set the Music
                        }
                        if (newKeyState.IsKeyDown(Keys.C) && oldKeyState != newKeyState)
                        {
                            theGame.gameState = Game1.GameState.ControlsMenu;       // Show the Controls
                        }
                        if (newKeyState.IsKeyDown(Keys.Escape) && oldKeyState != newKeyState)
                        {
                            theGame.Exit();     //Exit the Game
                        }

                        if (newPadState.Buttons.A == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.gameState = Game1.GameState.Playing;        //Play the Game
                            MediaPlayer.Play(theGame.backgroundMusic);          //Set the Music
                        }
                        if (newPadState.Buttons.Y == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.gameState = Game1.GameState.ControlsMenu;       // Show the Controls
                        }
                        if (newPadState.Buttons.B == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.Exit();     //Exit the Game
                        }

                        break;
                    }
                case Game1.GameState.Playing:
                    {
                        
                        controlPlayer.speedSound = newPadState.ThumbSticks.Left.Y;      //Adjust the speedSound variable (engine pitch) based on the left thumbstick Y
                        controlPlayer.rotateVector = Vector3.Zero;                      //Reset the Rotate Vector

                        if (newKeyState.IsKeyDown(Keys.Escape) && oldKeyState != newKeyState)
                        {
                            theGame.CreateTexture();                        //Create a Texture from the current frame for the Pause Menu to use
                            theGame.gameState = Game1.GameState.PauseMenu;  //Pause the Game
                        }

                        if (newPadState.Buttons.Start == ButtonState.Pressed && oldPadState.Buttons.Start == ButtonState.Released)
                        {
                            theGame.CreateTexture();                        //Create a Texture from the current frame for the Pause Menu to use
                            theGame.gameState = Game1.GameState.PauseMenu;  //Pause the Game
                        }

                        KeyboardInput();                //Call the Keyboard Input Handler
                        MouseInput();                   //Call the Mouse Input Handler
                        if (newPadState.IsConnected)
                        {
                            ControllerInput();          //If there is a Controller Conected: Call the Controller Input Handler
                        }

                        break;
                    }
                case Game1.GameState.ControlsMenu:
                    {
                        if (newKeyState.GetPressedKeys().Length > 0 && oldKeyState != newKeyState)
                        {
                            theGame.gameState = Game1.GameState.MainMenu;   //Return to Menu
                        }

                        if (newPadState.Buttons.B == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.gameState = Game1.GameState.MainMenu;   //Return to Menu
                        }
                        

                        break;
                    }
                case Game1.GameState.PauseMenu:
                    {
                        if (newKeyState.IsKeyDown(Keys.Enter) && oldKeyState != newKeyState)
                        {
                            theGame.gameState = Game1.GameState.Playing;   //Play the Game
                        }
                        if (newKeyState.IsKeyDown(Keys.Escape) && oldKeyState != newKeyState)
                        {
                            theGame.Exit();     //Exit the Game
                        }

                        if (newPadState.Buttons.Start == ButtonState.Pressed && oldPadState.Buttons.Start == ButtonState.Released)
                        {
                            theGame.gameState = Game1.GameState.Playing;   //Play the Game
                        }
                        if (newPadState.Buttons.B == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.Exit();     //Exit the Game
                        }

                        break;
                    }
                case Game1.GameState.CompleteScreen:
                    {
                        if (newKeyState.GetPressedKeys().Length > 0 && oldKeyState != newKeyState)
                        {
                            theGame.Exit();     //Exit the Game
                        }
                        if (newPadState.Buttons.B == ButtonState.Pressed && newPadState != oldPadState)
                        {
                            theGame.Exit();     //Exit the Game
                        }

                        break;
                    }
            }

            if (newKeyState.IsKeyDown(Keys.M) && oldKeyState != newKeyState)
            {
                soundOn = !soundOn;         //Invert the sound boolean (on/off)
                theGame.Mute(soundOn);      //Call the Mute Method with this boolean
            }
            if (newKeyState.IsKeyDown(Keys.F11) && oldKeyState != newKeyState)
            {
                fullScreen = !fullScreen;           //Invert the FullScreen boolean (on/off)
                theGame.ScreenSize(fullScreen);     //Call the ScreenSize Method with this boolean
            }

            if ((newPadState.DPad.Down == ButtonState.Pressed || newPadState.DPad.Up == ButtonState.Pressed || newPadState.DPad.Left == ButtonState.Pressed || newPadState.DPad.Right == ButtonState.Pressed) && newPadState != oldPadState)
            {
                soundOn = !soundOn;         //Invert the sound boolean (on/off)
                theGame.Mute(soundOn);      //Call the Mute Method with this boolean
            }
            if (newPadState.Buttons.Back == ButtonState.Pressed && newPadState != oldPadState)
            {
                fullScreen = !fullScreen;           //Invert the FullScreen boolean (on/off)
                theGame.ScreenSize(fullScreen);     //Call the ScreenSize Method with this boolean
            }
            oldKeyState = newKeyState;      //At the end of each frame set the current KeyState to the previous keystate
            oldPadState = newPadState;      //At the end of each frame set the current PadState to the previous PadState
            
            base.Update(gameTime);
        }

#region Playing Input
        public void KeyboardInput()
        {
            if (newKeyState.IsKeyDown(Keys.A))
            {
                //Roll the player Left
                controlPlayer.rotateVector.Z = -1;
            }
            if (newKeyState.IsKeyDown(Keys.D))
            {
                //Roll the player Right
                controlPlayer.rotateVector.Z = 1;
            }
            if (newKeyState.IsKeyDown(Keys.S))
            {
                //Slow slightly
                controlPlayer.playerSpeed -= 3f;
                //Adjust the speedSound variable (engine pitch)
                controlPlayer.speedSound = -1f;
            }
            if (newKeyState.IsKeyDown(Keys.W))
            {
                //Speed Up slightly
                controlPlayer.playerSpeed += 3f;
                //Adjust the speedSound variable (engine pitch)
                controlPlayer.speedSound = 1f;
            }
        }

        public void MouseInput()
        {
            //Set the current MouseState
            curMouseState = Mouse.GetState();

            float deltaX;   //Mouse Position X
            float deltaY;   //Mouse Position Y

            if (curMouseState != preMouseState)
            {
                //Cache the Mouse Location (X and Y)
                deltaX = curMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = curMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

                //Calculate the Rotation buffer based on how much the mouse has moved per frame
                mouseRotBuffer.X -= 0.1f * deltaX * delta;
                mouseRotBuffer.Y -= 0.1f * deltaY * delta;

                //Set the Player rotation to the created Rotation Buffer 
                controlPlayer.Rotation = Quaternion.CreateFromYawPitchRoll(mouseRotBuffer.X, -mouseRotBuffer.Y, controlPlayer.Rotation.Z);

                //Reset Delta X and Y to zero
                deltaX = 0; 
                deltaY = 0;
            }
            //Reset the Mouse to the Centre of the Screen
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            if (curMouseState.LeftButton == ButtonState.Pressed)
            {
                //Call the Projectile Fire Method
                theGame.Fire();
            }

            if (curMouseState.RightButton == ButtonState.Pressed)
            {
                //Call the Player Boost Method, the Player is boosting
                controlPlayer.Boost(true);
            }

            if (curMouseState.RightButton == ButtonState.Released && preMouseState.RightButton == ButtonState.Pressed)
            {
                //Call the Player Boost Method, the Player has stopped boosting
                controlPlayer.Boost(false);
            }

            if (curMouseState.MiddleButton == ButtonState.Pressed)
            {
                //Set the Camera to true (Secondary Camera)
                theGame.SetCamera(true);
            }
            if (curMouseState.MiddleButton == ButtonState.Released && preMouseState.MiddleButton == ButtonState.Pressed)
            {
                //Set the Camera to false (Main Camera)
                theGame.SetCamera(false);
            }
           
            preMouseState = curMouseState;      // At the end of each frame set the current MouseState to the previous MouseState
        }

        public void ControllerInput()
        {
            //Reset the Yaw, Pitch and Roll Variables once a frame
            controlPlayer.yaw = 2f;
            controlPlayer.pitch = 2f;
            controlPlayer.roll = 3f;
            
            if (newPadState.ThumbSticks.Left.Y > 0f)
            {
                //Speed Up Slightly
                controlPlayer.playerSpeed += (newPadState.ThumbSticks.Left.Y * 3f);
            }

            if (newPadState.ThumbSticks.Left.Y < 0f)
            {
                //Slow Slightly
                controlPlayer.playerSpeed += (newPadState.ThumbSticks.Left.Y * 3f);
                
            }
            if (newPadState.Triggers.Right > 0f)
            {
                //Call the Projectile Fire Method
                theGame.Fire();
            }

            if (newPadState.Triggers.Left > 0f)
            {
                //Call the Player Boost Method, the Player is boosting
                controlPlayer.Boost(true);
            }
            if (newPadState.Triggers.Left == 0f && oldPadState.Triggers.Left > 0f)
            {
                //Call the Player Boost Method, the Player has stopped boosting
                controlPlayer.Boost(false);
            }

            if (newPadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                //Set the Camera to true (Secondary Camera)
                theGame.SetCamera(true);
            }
            if (newPadState.Buttons.LeftShoulder == ButtonState.Released && oldPadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                //Set the Camera to false (Main Camera)
                theGame.SetCamera(false);
            }

            /*------------------Roll------------------*/
            if (newPadState.ThumbSticks.Right.X > 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.roll *= newPadState.ThumbSticks.Right.X;
                //Rotate Around the Z Axis (Roll)
                controlPlayer.rotateVector.Z = 1;
            }
            if (newPadState.ThumbSticks.Right.X < 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.roll *= -newPadState.ThumbSticks.Right.X;
                //Rotate Around the Z Axis (Roll)
                controlPlayer.rotateVector.Z = -1;
            }
            /*------------------Roll------------------*/

            /*-------------------Yaw------------------*/
            if (newPadState.ThumbSticks.Left.X < 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.pitch *= -newPadState.ThumbSticks.Left.X;
                //Rotate Around the X Axis (Roll)
                controlPlayer.rotateVector.X = 1;
            }
            if (newPadState.ThumbSticks.Left.X > 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.pitch *= newPadState.ThumbSticks.Left.X;
                //Rotate Around the X Axis (Roll)
                controlPlayer.rotateVector.X = -1;
            }
            /*-------------------Yaw--------------------*/

            /*------------------Pitch-----------------*/
            if (newPadState.ThumbSticks.Right.Y > 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.yaw *= newPadState.ThumbSticks.Right.Y;
                //Rotate Around the Y Axis (Roll)
                controlPlayer.rotateVector.Y = 1;
            }
            if (newPadState.ThumbSticks.Right.Y < 0f)
            {
                //Set the Speed of the Roll
                controlPlayer.yaw *= -newPadState.ThumbSticks.Right.Y;
                //Rotate Around the Y Axis (Roll)
                controlPlayer.rotateVector.Y = -1;
            }
            /*------------------Pitch-----------------*/
        }

        
#endregion

    }
}
