using System;
using System.IO;
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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
#region Instance Variables

        private GraphicsDeviceManager graphics;     //Graphics Device Manager variable
        private SpriteBatch spriteBatch;            //The spritebatch variable
        private SpriteFont fontToUse;               //Variable for the Font
        private BasicEffect effect;                 //BasicEffect applied to models
        private GameTime gTime;                     //Variable to hold the GameTime

        private float screenWidth;                  //variable that holds the current Screen Width
        private float screenHeight;                 //variable that holds the current Screen Height

        private Texture2D empty;                    //Variable for the empty Texture
        private Texture2D gradient;                 //Variable that holds a graident texture, used for the health and boost bars
        private Texture2D crosshair;                //variable that holds the crosshair texture


#region Class Instances
        private Player player;                      //Player Object
        private InputManager input;                 //Input Manager Object
        private Camera mainCamera;                  //Main Camera Object
        private Camera secCamera;                   //Secondary Camera Object
        private Camera selectedCamera;              //Selected Camera Object
#endregion

#region Model Variables
        private Model mPlayer;                                              //Model for the Player
        private Matrix[] mPlayerTransforms;                                 //Matrix for the Player Model Transforms
        private Vector3 playerPosition = new Vector3(10f, 200f, 5f);        //Player Spawn Position
        private Vector3 playerRotation = Vector3.Zero;                      //Player Spawn Rotation
        private float playerScale = 0.01f;                                  //Player Scale

        List<Enemy> enemyList = new List<Enemy>();                          //List that holds all the Enemies
        private Model mEnemy;                                               //Model for the Enemies
        private Matrix[] mEnemyTransforms;                                  //Matrix for the Enemy Model Transforms
        private Vector3 enemyPosition;                                      //Enemy Spawn Position Holder
        private Quaternion enemyRotation = Quaternion.Identity;             //Enemy Spawn Rotation
        private float enemyScale = 0.002f;                                  //Enemy Scale
        private Random random = new Random();                               //Instance of Random for random Spawn Positons
        private const int numEnemies = 10;                                  //Number of Enemies to Spawn

        private Model mLaser;                                               //Model for the Lasers
        private Matrix[] mLaserTransforms;                                  //Matrix for the Laser Model Transforms
        List<Projectile> laserList = new List<Projectile>();                //List that holds all the Lasers

        private Model mEnemyLaser;                                          //Model for the Enemy Lasers
        private Matrix[] mEnemyLaserTransforms;                             //Matrix for the Enemy Laser Model Transforms
        List<Projectile> enLaserList = new List<Projectile>();              //List that holds all the Enemy Lasers

        private float laserScale = 1f;                                      //Enemy Scale
        double lastLaserTime = 0;                                           //Time since the last Laser, zero by start
        bool firePos = true;                                                //Firing Position of the laser

        private Model mTerrain;                                             //Model for the Terrain
        private Matrix[] mTerrainTransforms;                                //Matrix for the Terrain Model Transforms
        private Vector3 terrainPosition = new Vector3(0f, -120f, 0f);       //Terrain Position
        private float terrainScale = 6f;                                    //Terrain Scale

        private Model mSkyBox;                                              //Model for the SkyBox
        private Matrix[] mSkyBoxTransforms;                                 //Matrix for the SkyBox Model Transforms
        private Vector3 skyboxPosition = new Vector3(0f, 100f, 0f);         //SkyBox Position
        private float skyboxScale = 400f;                                   //SkyBox Scale
#endregion

#region Collision Variables
        private List<BoundingBox> boundingArea = new List<BoundingBox>();                                                   //List to hold all the Bounding Volumes

        private BoundingBox frontWall = new BoundingBox(new Vector3(-500f, 0f, 500f), new Vector3(500f, 600f, 800f));       //Bounding Box on the Front Wall of the Level
        private BoundingBox backWall = new BoundingBox(new Vector3(-500f, 0f, -500f), new Vector3(500f, 600f, -800f));      //Bounding Box on the Back Wall of the Level

        private BoundingBox leftWall = new BoundingBox(new Vector3(500f, 0f, -500f), new Vector3(800f, 600f, 500f));        //Bounding Box on the Left Wall of the Level
        private BoundingBox rightWall = new BoundingBox(new Vector3(-500f, 0f, -500f), new Vector3(-800f, 600f, 500f));     //Bounding Box on the Right Wall of the Level

        private BoundingBox topWall = new BoundingBox(new Vector3(-500f, 400f, -500f), new Vector3(500f, 800f, 500f));      //Bounding Box on the Roof of the Level
        private BoundingBox bottomWall = new BoundingBox(new Vector3(-500f, -50f, -500f), new Vector3(500f, 150f, 500f));   //Bounding Box on the Floor of the Level

        private bool outsideBounds = false;             //Is the player outside the bounds of the Level? No at Start
        private string warningMsg;                      //Warning Message to display to the player upon intersection of a level bounding volume
        private double countDown;                       //Count Down started Upon intersection
        private double timer = 5;                       //Value of time for CountDown
        private double counter;                         //Counter Value
#endregion

#region Sound Variables
        public SoundEffect enemySound;                  //Variable that Loads the Enemy Movement Sound
        public SoundEffectInstance enemySoundFX;        //Public Instance of the Enemy Movement Sound FX

        public SoundEffect playerSound;                 //Variable that Loads the Player Movement Soun
        public SoundEffectInstance playerSoundFX;       //Public Instance of the Player Movement Sound FX

        public SoundEffect boostSound;                  //Variable that Loads the Player Boost Sound
        public SoundEffectInstance playerBoostFX;       //Public Instance of the Player Boost Sound FX

        public SoundEffect enemyFire;                   //Variable that Loads the Enemy Firing Sound
        public SoundEffectInstance enemyFireFX;         //Public Instance of the Enemy Firing Sound FX

        public SoundEffect playerFire;                  //Variable that Loads the Player Firing Sound
        public SoundEffectInstance playerFireFX;        //Public Instance of the Player Firing Sound FX

        public SoundEffect enemyExplode;                //Variable that Loads the Enemy Explosion Sound
        public SoundEffectInstance enemyExplodeFX;      //Public Instance of the Enemy Explosion Sound FX

        public SoundEffect playerExplode;               //Variable that Loads the Player Explosion Sound
        public SoundEffectInstance playerExplodeFX;     //Public Instance of the Player Explosion Sound FX

        private Song backgroundMusic;                   //Variable that Loads the Theme Music

        private AudioEmitter emitter = new AudioEmitter();      //Default emitter for sounds
        public AudioListener listener = new AudioListener();    //Listens for sounds
#endregion

#endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);     //Set the Graphics Device Manager
            Content.RootDirectory = "Content";              //Set the Content Directory
            ScreenSize(false);                              //Call the ScreenSize Method (false)
        }

#region Default Methods

        protected override void Initialize()
        {
            Window.Title = "Gavin Whitehall - Coursework";      //Name the Window Title

            mainCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero);      //Initilize the Main Camera
            Components.Add(mainCamera);                                                 //Add the Main Camera to the Game Components

            secCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero);       //Initilize the Secondary Camera
            Components.Add(secCamera);                                                  //Add the Secondary Camera to the Game Components

            player = new Player(this, this, mainCamera, secCamera, playerPosition, Quaternion.Identity);    //Initilize the Player
            Components.Add(player);                                                                         //Add the Player Camera to the Game Components

            selectedCamera = mainCamera;                                //Set the Selected Camera to the Main Camera on Start
            effect = new BasicEffect(GraphicsDevice);                   //Set Up Basic Effect
            input = new InputManager(this, this, mainCamera, player);   //Initilize the Input Manager
            Components.Add(input);                                      //Add the Input Manager to the Game Components

            /* For each of the Enemies */
            for (int i = 0; i < numEnemies; i++)
            {
                /* Set a Random Spawn Position within the Boundries */
                enemyPosition.X = (float)random.Next(-400,400);
                enemyPosition.Y = (float)random.Next(200, 350);
                enemyPosition.Z = (float)random.Next(-400, 400);

                Enemy enemy = new Enemy(this, this, enemyPosition, enemyRotation, player);  //Initilize an Enemy
                enemyList.Add(enemy);                                                       //Add the Enemy to the List of Enemies
                Components.Add(enemy);                                                      //Add the Enemy to the Game Components
            }

            boundingArea.Add(frontWall);    //Add the Front Wall to the Bounding Area List
            boundingArea.Add(backWall);     //Add the Back Wall to the Bounding Area List
            boundingArea.Add(leftWall);     //Add the Left Wall to the Bounding Area List
            boundingArea.Add(rightWall);    //Add the Roof to the Bounding Area List
            boundingArea.Add(bottomWall);   //Add the Floor to the Bounding Area List

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);                                  //Set Up the SpriteBatch
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\GameFont");                     //Load the Game Font

            mPlayer = Content.Load<Model>(".\\Models\\ship");                               //Load Player Model
            mPlayerTransforms = SetupEffectTransformDefaults(mPlayer, true);                //Call the Setup Effect Transform Defaults on the Player Model
            mLaser = Content.Load<Model>(".\\Models\\laser1");                              //Load Player Laser Model
            mLaserTransforms = SetupEffectTransformDefaults(mLaser, true);                  //Call the Setup Effect Transform Defaults on the Player Laser Model

            mTerrain = Content.Load<Model>(".\\Models\\terrain");                           //Load Terrain Model
            mTerrainTransforms = SetupEffectTransformDefaults(mTerrain, true);              //Call the Setup Effect Transform Defaults on the Terrain Model
            mSkyBox = Content.Load<Model>(".\\Skybox\\skybox");                             //Load SkyBox Model
            mSkyBoxTransforms = SetupEffectTransformDefaults(mSkyBox, false);               //Call the Setup Effect Transform Defaults on the SkyBox Model

            mEnemy = Content.Load<Model>(".\\Models\\enemy");                               //Load Enemy Model
            mEnemyTransforms = SetupEffectTransformDefaults(mEnemy, true);                  //Call the Setup Effect Transform Defaults on the Enemy Model
            mEnemyLaser = Content.Load<Model>(".\\Models\\EnemyLaser");                     //Load Enemy Laser Model
            mEnemyLaserTransforms = SetupEffectTransformDefaults(mEnemyLaser, true);        //Call the Setup Effect Transform Defaults on the Enemy Laser Model

            empty = Content.Load<Texture2D>(".\\GUI\\empty");                               //Load Empty Texture
            gradient = Content.Load<Texture2D>(".\\GUI\\grad");                             //Load Gradient Texture
            crosshair = Content.Load<Texture2D>(".\\GUI\\crosshair");                       //Load CrossHair Texture
            LoadSound();                                                                    //Call the LoadSound Method, to Load the Sound FX
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds; //Create deltaTime (time since last frame update)
            screenWidth = graphics.PreferredBackBufferWidth;                //Set the screenWidth to the current screen width per frame 
            screenHeight = graphics.PreferredBackBufferHeight;              //Set the screenHeight to the current screen height per frame 

            /* For all the Lasers in Both Lists */
            for (int i = 0; i < laserList.Count; i++)
            {
                laserList[i].UpdateLaser(gameTime); //Update the update method with the gameTime variable
            }

            for (int i = 0; i < enLaserList.Count; i++)
            {
                enLaserList[i].UpdateLaser(gameTime);   //Update the update method with the gameTime variable
            }

            Collisions();   //Call the Collisions Method
            
            Matrix playerRot = Matrix.CreateFromQuaternion(player.Rotation);    //Create a Matrix from the Player Rotation
            listener.Position = selectedCamera.Position;                        //Set the Listener Position to the Selected Camera Position so it follows the Camera
            listener.Forward = playerRot.Forward;                               //Set the Forward of the listener to the player rotation Matrix Forward so its looking in the direct of the player
            listener.Up = playerRot.Up;                                         //Set the Up of the listener to the player rotation Matrix Up so its rolling with the player

            gTime = gameTime;       //Set gTime to gameTime   
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);      //Set the Background Colour

            RasterizerState rs = new RasterizerState(); //Rasterize Sate
            rs.CullMode = CullMode.CullClockwiseFace;   //Set the Culling

            DrawSkyBox();   //Draw the SkyBox

            /* Apply Scale, Rotations and Translations to the Player, and Draw */
            Matrix playerTransform =  Matrix.CreateScale(playerScale) * Matrix.CreateFromQuaternion(player.Rotation) * Matrix.CreateTranslation(player.Position);
            DrawModel(mPlayer, playerTransform, mPlayerTransforms);

            /* Apply Scale and Translation to the Terrain, and Draw */
            Matrix terrainTransform = Matrix.CreateScale(terrainScale) * Matrix.CreateTranslation(terrainPosition);
            DrawModel(mTerrain, terrainTransform, mTerrainTransforms);

            /* For Each of the Enemies in the List */
            for (int i = 0; i < enemyList.Count; i++)
            {
                /* Apply Scale and Translation to the Enemey, and Draw */
                Matrix enemyTransform = Matrix.CreateScale(enemyScale) * Matrix.CreateFromQuaternion(enemyList[i].Rotation) * Matrix.CreateTranslation(enemyList[i].Position);
                DrawModel(mEnemy, enemyTransform, mEnemyTransforms);
            }

            DrawLaser();    //Draw the Laser(s)

            double boostTimer = Math.Round(player.boostTimer);  //Create a Rounded Boost Timer for the GUI

            DrawGUI(gradient, new Vector2(screenWidth / 1.3f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5), Color.DarkGray, false);                                //Draw a Scalable Background for the Boost Bar
            DrawGUI(gradient, new Vector2(screenWidth / 1.3f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5 * (boostTimer / 100)), Color.DeepSkyBlue, false);        //Draw a Scalable Boost Bar

            DrawGUI(gradient, new Vector2(screenWidth / 30f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5), Color.DarkGray, false);                                 //Draw a Scalable Background for the Health Bar
            DrawGUI(gradient, new Vector2(screenWidth / 30f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5 * ((double)player.health / 100)), Color.DarkRed, false);  //Draw a Scalable Health Bar

            DrawGUI(empty, new Vector2(screenWidth / 2f, screenHeight / 16f), (int)(screenHeight / 10), (int)(screenWidth / 10), Color.DarkGray, true);                                     //Draw a Scalable Background for the Score
            WriteText(enemyList.Count.ToString(), new Vector2(screenWidth / 2f, screenHeight / 16f), Color.White, true);                                                                    //Display the Number of Enemies on the Screen (Score)

            /* On the Condition the Player is outside the Bounds of the Level */
            if (outsideBounds)
            {
                WriteText(warningMsg + Math.Round(countDown).ToString() + " SECONDS!", new Vector2(screenWidth / 2, screenHeight /3), Color.Green, true);   //Display the Scalable Warning Message
            }

            /* On the Condition the Main Camera is Drawing */
            if (selectedCamera == mainCamera)
            {
                DrawGUI(crosshair, new Vector2(screenWidth / 2, screenHeight / 2), (int)(screenWidth * 0.04), (int)(screenHeight * 0.07), Color.Red, true);   //Display the Scalable CrossHair
            }

            base.Draw(gameTime);
        }

#endregion

#region Methods

        public void LoadSound()
        {
            /* Load the Sound Effects for the Player, Create an Instance, Set Looping On and Apply it to the Scene */                     
            playerSound = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-move3.wav"));
            playerSoundFX = playerSound.CreateInstance();
            playerSoundFX.IsLooped = true;
            playerSoundFX.Apply3D(listener, emitter);

            /* Load the Sound Effects for the Boost, Create an Instance, Set Looping Off and Apply it to the Scene */ 
            boostSound = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-move.wav"));
            playerBoostFX = boostSound.CreateInstance();
            playerBoostFX.IsLooped = false;
            playerBoostFX.Apply3D(listener, emitter);

            /* Load the Sound Effects for the Player Explode, Create an Instance and Apply it to the Scene */ 
            playerExplode = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-explode.wav"));
            playerExplodeFX = playerExplode.CreateInstance();
            playerExplodeFX.Apply3D(listener, emitter);

            enemyExplode = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\enemy-explode.wav"));    //Load the Enemy Explode Sound
            enemyFire = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\enemy-fire.wav"));          //Load the Enemy Fire Sound
            playerFire = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-fire.wav"));        //Load the Player Fire Sound

            /* Load the Background Music, Play it, Set the Volume and Loop it */ 
            backgroundMusic = Content.Load<Song>(".\\Sounds\\music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.IsRepeating = true;
        }
            
        public void OutsideBounds(BoundingBox boxHit)
        {
            counter += gTime.ElapsedGameTime.TotalSeconds;      //Set the Counter

            /* On the Condition the Player has been outside the bounds for too long */
            if (counter >= timer)
            {
                player.Destroy();       //Destroy the Player
                outsideBounds = false;  //Outside Bounds is now false
                counter = 0;            //Reset Counter
            }

            countDown = timer - counter;    //Begin CountDown
            outsideBounds = true;           //Player is Outside the Level

            /* If the Player is Touching the Appropriate Level Bounding Volume(s) */
            if (boxHit == bottomWall)
            {
                warningMsg = "YOU ARE TOO LOW! INCREASE ALTITUDE WITHIN ";      //Set the Warning Message
            }
            else if (boxHit == topWall)
            {
                warningMsg = "YOU ARE TOO HIGH! REDUCE ALTITUDE WITHIN ";       //Set the Warning Message
            }
            else
            {
                warningMsg = "YOU ARE LEAVING THE BATTLEFIELD! RETURN WITHIN "; //Set the Warning Message
            }
        }

        public void ScreenSize(bool fullscreen)
        {
            /* If the Player Sets the FullScreen */
            if (!fullscreen)
            {
                /* FullScreen is False, Height and Width are Half the Screen */
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
                graphics.ApplyChanges();    //Apply the Above
            }
            else
            {
                /* FullScreen is true, Height and Width are the Size of the Screen Screen */
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.ApplyChanges();    //Apply the Above
            }
        }

        public void Fire()
        {
            double currentTime = gTime.TotalGameTime.TotalMilliseconds;     //The Current Time is

            /* On the Coniditon that 200ms has passed since the last fire */
            if (currentTime - lastLaserTime > 200)
            {
                firePos = !firePos;             //Invert the Fire Pos to Change the Offset
                Projectile newLaser = new Projectile(this, player.Position, player.Rotation, 1f, firePos, true);    //Create a New Laser
                laserList.Add(newLaser);        //Add the new Laser to the List
                lastLaserTime = currentTime;    //Set the Time to the current Time
            }
        }

        public void EnemyFire(Vector3 position, Quaternion rotation, bool firePos)
        {
            Projectile newLaser = new Projectile(this, position, rotation, 1f, firePos, false);     //Create a New Laser
            enLaserList.Add(newLaser);      //Add the new Laser to the List
        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel, bool lighting)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];  //Set the new Matrix to the inputed Models
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);       //Copy the Transofmrs to the new Matrix

            /* As a Model may have multiple meshes */
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    /* If this Model has Lighting Enabled */
                    if (lighting)
                    {
                        effect.EnableDefaultLighting();     //Enable Default Lighting
                    }
                    effect.View = selectedCamera.View;      //The View Matrix for the effect is the Selected Camera
                    effect.Projection = selectedCamera.Projection;  //The Projection Matrix for the effect is the Selected Camera
                }
            }
            return absoluteTransforms;  //Return the transforms
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;   //Set the Texture Wrapping Method

            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = selectedCamera.View;      //Set the view matrix
                    effect.Projection = selectedCamera.Projection;  //Set the View Matrix
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;  //Set the World Matrix 
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        private void WriteText(string msg, Vector2 msgPos, Color msgColour, bool centre)
        {
            spriteBatch.Begin();
            string output = msg;
            Vector2 FontPos;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            
            /* Is the text is to be centred? */
            if (centre)
            {
                FontPos = msgPos - FontOrigin;
            }
            else
            {
                FontPos = msgPos;
            }

            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }

        private void DrawGUI(Texture2D image, Vector2 position, int height, int width, Color colour, bool centre)
        {
            spriteBatch.Begin();
            Vector2 TexturePos;
            Vector2 TextureOrigin = new Vector2(width / 2, height / 2);

            /* Is the texture is to be centred? */
            if (centre)
            {
                TexturePos = position - TextureOrigin;
            }
            else
            {
                TexturePos = position;
            }

            // Draw the Texture
            Rectangle rect = new Rectangle((int)TexturePos.X, (int)TexturePos.Y, width, height);
            spriteBatch.Draw(image, rect, colour);
            spriteBatch.End();
        }

        public void DrawLaser()
        {
            /* Every Laser in each List */
            for (int i = 0; i < laserList.Count; i++)
            {
                /* Apply the Scale, Rotations and Translations to the laser and Draw */
                Matrix laserTransform = Matrix.CreateScale(laserScale) * Matrix.CreateFromQuaternion(laserList[i].Rotation) * Matrix.CreateTranslation(laserList[i].Position);
                DrawModel(mLaser, laserTransform, mLaserTransforms);
            }
            for (int i = 0; i < enLaserList.Count; i++)
            {
                /* Apply the Scale, Rotations and Translations to the laser and Draw */
                Matrix enLaserTransform = Matrix.CreateScale(laserScale) * Matrix.CreateFromQuaternion(enLaserList[i].Rotation) * Matrix.CreateTranslation(enLaserList[i].Position);
                DrawModel(mEnemyLaser, enLaserTransform, mEnemyLaserTransforms);
            }
        }

        public void DrawSkyBox()
        {
            /* Set the Clamping Mode and invert the UV so the texture displays inside the Model */
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            GraphicsDevice.DepthStencilState = dss;

            /* Apply the Scale, Rotations and Translations to the SkyBox and Draw */
            Matrix skyboxTrasform = Matrix.CreateScale(skyboxScale) * Matrix.CreateFromYawPitchRoll(0f, 0f, 0f) * Matrix.CreateTranslation(player.Position.X, skyboxPosition.Y, player.Position.Z);
            DrawModel(mSkyBox, skyboxTrasform, mSkyBoxTransforms);

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;
        }

        public void SetCamera(bool secSelected)
        {
            /* Set the Selected Camera to What the player has selected*/
            if (secSelected)
            {
                selectedCamera = secCamera;
            }
            else
            {
                selectedCamera = mainCamera;
            }
        }

        public void Mute(bool isMuted)
        {
            /* Has the Player Muted the Game? */
            if (isMuted)
            {
                SoundEffect.MasterVolume = 0f;  //Set the volume to Zero
                MediaPlayer.IsMuted = true;     //Mute the Media Player
            }
            else
            {
                SoundEffect.MasterVolume = 1f;  //Set the volume to default
                MediaPlayer.IsMuted = false;     //UnMute the Media Player
            }
        }

        public void Collisions()
        {
            BoundingSphere playerSphere = new BoundingSphere(player.Position, 5f);  //Create a Bounding Sphere per frame for the Player

            /* For the Active Lasers in the Players Laser List */
            for (int i = 0; i < laserList.Count; i++)
            {
                BoundingSphere laserSphere = new BoundingSphere(laserList[i].Position, 1f); //Create a Bounding Sphere

                /* The Bounding Area Volumes */
                for (int j = 0; j < boundingArea.Count; j++)
                {   
                    /* Does a Player Laser Collide */
                    if (laserSphere.Intersects(boundingArea[j]))
                    {
                        laserList[i].isActive = false;  //The Colliding Laser is now inactive
                        laserList.Remove(laserList[i]); //Removed from the Laser List as its inactive
                    }
                }
                /* The Enemy Bounding Volumes */
                for (int e = 0; e < enemyList.Count; e++)
                {
                    BoundingSphere enemySphere = new BoundingSphere(enemyList[e].Position, mEnemy.Meshes[0].BoundingSphere.Radius * enemyScale);    //Create a Bounding Sphere for the Enemies

                    /* If a Player Laser Collides with an Enemy */
                    if (laserSphere.Intersects(enemySphere))
                    {
                        enemyExplodeFX = enemyExplode.CreateInstance();             //Create and Instance of the Enemy Explode Sound FX
                        enemyExplodeFX.Apply3D(listener, enemyList[e].emitter);     //Apply the Sound to the Scene
                        enemyExplodeFX.Play();                                      //Play the Sound
                        enemyExplodeFX.Apply3D(listener, enemyList[e].emitter);     //Reapply the Sound to the Scene

                        enemyList[e].isActive = false;                              //Make the enemy inactive
                        enemyList.Remove(enemyList[e]);                             //Remove the Enemy from its list due to it being destroyed
                        laserList[i].isActive = false;                              //Make the laser inactive
                        laserList.Remove(laserList[i]);                             //Remove the Laser from its list due to it Colliding
                    }
                }
            }

            /* For the Active Lasers in the Enemies Laser List */
            for (int i = 0; i < enLaserList.Count; i++)
            {
                BoundingSphere enLaserSphere = new BoundingSphere(enLaserList[i].Position, 1f); //Create a Bounding Sphere

                /* The Bounding Area Volumes */
                for (int j = 0; j < boundingArea.Count; j++)
                {
                    /* Does an Enemy Laser Collide */
                    if (enLaserSphere.Intersects(boundingArea[j]))
                    {
                        enLaserList[i].isActive = false;  //The Colliding Laser is now inactive
                        enLaserList.Remove(enLaserList[i]); //Removed from the Laser List as its inactive
                    }
                }

                /* If an Enemy Laser Collides with the Player */
                if (enLaserSphere.Intersects(playerSphere))
                {
                    player.Damage();                    // Call the Damage Method
                    enLaserList[i].isActive = false;    //Make the laser inactive
                    enLaserList.Remove(enLaserList[i]); //Remove the Laser from the List
                }
            }

            outsideBounds = false;      //The player is inside the bounds by default

            /* Each of the Level Bounding Volumes */
            for (int i = 0; i < boundingArea.Count; i++)
            {
                /* Upon the player intersecting */
                if (playerSphere.Intersects(boundingArea[i]))
                {
                    OutsideBounds(boundingArea[i]); //Call the OutsideBounds Method
                    outsideBounds = true;           //The player is outside the bounds
                }
            }

            /* the player is inside the bounds */
            if (!outsideBounds)
            {
                counter = 0;    //The Counter is set to default
            }
        }

#endregion
    }
}
