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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        private String songInfo;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffectInstance tardisSoundInstance;
        private SoundEffect tardisSound;
        private SoundEffect explosionSound;
        private SoundEffect firingSound;

        // Set the 3D model to draw.
        private Model mdlTardis;
        private Matrix[] mdlTardisTransforms;

        // The aspect ratio determines how to scale 3d to 2d projection.
        //private float aspectRatio;

        // Set the position of the model in world space, and set the rotation.
        private Vector3 mdlPosition = Vector3.Zero;
        private float mdlRotation = 0.0f;
        private Vector3 mdlVelocity = Vector3.Zero;

        // create an array of enemy daleks
        private Model mdlDalek;
        private Matrix[] mdDalekTransforms;
        private Daleks[] dalekList = new Daleks[GameConstants.NumDaleks];

        // create an array of laser bullets
        private Model mdlLaser;
        private Matrix[] mdlLaserTransforms;
        private Laser[] laserList = new Laser[GameConstants.NumLasers];

        private Random random = new Random();

        private KeyboardState lastState;
        private int hitCount;

        // Set the position of the camera in world space, for our view matrix.
        private Vector3 cameraPosition = new Vector3(0.0f, 3.0f, 300.0f);
        //private Matrix viewMatrix;
        //private Matrix projectionMatrix;
        private Camera mainCamera;

        //private void InitializeTransform()
        //{
        //    aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

        //    viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

        //    projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
        //        MathHelper.ToRadians(45), aspectRatio, 1.0f, 350.0f);

        //}

        private void MoveModel()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Create some velocity if the right trigger is down.
            Vector3 mdlVelocityAdd = Vector3.Zero;

            // Find out what direction we should be thrusting, using rotation.
            mdlVelocityAdd.X = -(float)Math.Sin(mdlRotation);
            mdlVelocityAdd.Z = -(float)Math.Cos(mdlRotation);

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // Rotate left.
                mdlRotation -= -1.0f * 0.10f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // Rotate right.
                mdlRotation -= 1.0f * 0.10f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                // Rotate left.
                // Create some velocity if the right trigger is down.
                // Now scale our direction by how hard the trigger is down.
                mdlVelocityAdd *= 0.05f;
                mdlVelocity += mdlVelocityAdd;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                // Rotate left.
                // Now scale our direction by how hard the trigger is down.
                mdlVelocityAdd *= -0.05f;
                mdlVelocity += mdlVelocityAdd;
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                mdlVelocity = Vector3.Zero;
                mdlPosition = Vector3.Zero;
                mdlRotation = 0.0f;
                tardisSoundInstance.Play();
            }
            
            //are we shooting?
            if (keyboardState.IsKeyDown(Keys.Space) || lastState.IsKeyDown(Keys.Space))
            {
                //add another bullet.  Find an inactive bullet slot and use it
                //if all bullets slots are used, ignore the user input
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (!laserList[i].isActive)
                    {
                        Matrix tardisTransform = Matrix.CreateRotationY(mdlRotation);
                        laserList[i].direction = tardisTransform.Forward;
                        laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                        laserList[i].position = mdlPosition + laserList[i].direction;
                        laserList[i].isActive = true;
                        firingSound.Play();
                        break; //exit the loop     
                    }
                }
            }
            lastState = keyboardState;

        }

        private void ResetDaleks()
        {
            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                dalekList[i].position = new Vector3(xStart, 0.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                dalekList[i].direction.X = -(float)Math.Sin(angle);
                dalekList[i].direction.Z = (float)Math.Cos(angle);
                dalekList[i].speed = GameConstants.DalekMinSpeed +
                   (float)random.NextDouble() * GameConstants.DalekMaxSpeed;
                dalekList[i].isActive = true;
            }

        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = mainCamera.projectionMatrix;
                    effect.View = mainCamera.viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            Window.Title = "Gavin Whitehall - Coursework";
            hitCount = 0;
            //InitializeTransform();
            mainCamera = new Camera(cameraPosition, Vector3.Forward, Vector3.Up, 1.777777777777778f, 45);
            //mainCamera.Initialize();
            ResetDaleks();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //-------------------------------------------------------------
            // added to load font
            //-------------------------------------------------------------
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\DrWho");
            //-------------------------------------------------------------
            // added to load Song
            //-------------------------------------------------------------
            bkgMusic = Content.Load<Song>(".\\Audio\\DoctorWhotheme11");
            //MediaPlayer.Play(bkgMusic);
            //MediaPlayer.IsRepeating = true;
            songInfo = "Song: " + bkgMusic.Name + " Song Duration: " + bkgMusic.Duration.Minutes + ":" + bkgMusic.Duration.Seconds;
            //-------------------------------------------------------------
            // added to load Model
            //-------------------------------------------------------------
            mdlTardis = Content.Load<Model>(".\\Models\\tardismodel");
            mdlTardisTransforms = SetupEffectTransformDefaults(mdlTardis);
            mdlDalek = Content.Load<Model>(".\\Models\\dalek");
            mdDalekTransforms = SetupEffectTransformDefaults(mdlDalek);
            mdlLaser = Content.Load<Model>(".\\Models\\laser");
            mdlLaserTransforms = SetupEffectTransformDefaults(mdlLaser);
            //-------------------------------------------------------------
            // added to load SoundFX's
            //-------------------------------------------------------------
            tardisSound = Content.Load<SoundEffect>("Audio\\tardisEdit");
            explosionSound = Content.Load<SoundEffect>("Audio\\explosion2");
            firingSound = Content.Load<SoundEffect>("Audio\\shot007");
            tardisSoundInstance = tardisSound.CreateInstance();
            tardisSoundInstance.Play();


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            MoveModel();
            
            // Add velocity to the current position.
            mdlPosition += mdlVelocity;

            // Bleed off velocity over time.
            mdlVelocity *= 0.95f;

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                dalekList[i].Update(timeDelta);
            }

            for (int i = 0; i < GameConstants.NumLasers; i++)
            {
                if (laserList[i].isActive)
                {
                    laserList[i].Update(timeDelta);
                }
            }

            BoundingSphere TardisSphere =
              new BoundingSphere(mdlPosition,
                       mdlTardis.Meshes[0].BoundingSphere.Radius *
                             GameConstants.ShipBoundingSphereScale);

            //Check for collisions
            for (int i = 0; i < dalekList.Length; i++)
            {
                if (dalekList[i].isActive)
                {
                    BoundingSphere dalekSphereA =
                      new BoundingSphere(dalekList[i].position, mdlDalek.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.DalekBoundingSphereScale);

                    for (int k = 0; k < laserList.Length; k++)
                    {
                        if (laserList[k].isActive)
                        {
                            BoundingSphere laserSphere = new BoundingSphere(
                              laserList[k].position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);
                            if (dalekSphereA.Intersects(laserSphere))
                            {
                                explosionSound.Play();
                                dalekList[i].isActive = false;
                                laserList[k].isActive = false;
                                hitCount++;
                                break; //no need to check other bullets
                            }
                        }
                        if (dalekSphereA.Intersects(TardisSphere)) //Check collision between Dalek and Tardis
                        {
                            explosionSound.Play();
                            dalekList[i].direction *= -1.0f;
                            //laserList[k].isActive = false;
                            break; //no need to check other bullets
                        }

                    }
                }
            }
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                mainCamera.Move(50.0f);
            }
            //mainCamera.UpdateCamera();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                if (dalekList[i].isActive)
                {
                    Matrix dalekTransform = Matrix.CreateScale(GameConstants.DalekScalar) * Matrix.CreateTranslation(dalekList[i].position);
                    DrawModel(mdlDalek, dalekTransform, mdDalekTransforms);
                }
            }
            for (int i = 0; i < GameConstants.NumLasers; i++)
            {
                if (laserList[i].isActive)
                {
                    Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laserList[i].position);
                    DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                }
            }

            Matrix modelTransform = Matrix.CreateRotationY(mdlRotation) * Matrix.CreateTranslation(mdlPosition);
            DrawModel(mdlTardis, modelTransform, mdlTardisTransforms);

            //writeText("Tardis Vs Daleks", new Vector2(50, 10), Color.Yellow);
           // writeText("Instructions\nPress The Arrow keys to move the Tardis\nSpacebar to fire!\nR to Reset", new Vector2(50, 50), Color.Black);

           // writeText(songInfo, new Vector2(50, 125), Color.AntiqueWhite);

            base.Draw(gameTime);
        }
    }
}
