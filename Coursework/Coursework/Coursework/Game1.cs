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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private SpriteFont fontToUse;

        Player player;
        private Model mPlayer;
        private Matrix[] mPlayerTransforms;
        private Vector3 playerPosition = new Vector3(10f, 200f, 5f);
        private Vector3 playerRotation = Vector3.Zero;
        private float playerScale = 0.01f;

        List<Enemy> enemyList = new List<Enemy>();
        private Model mEnemy;
        private Matrix[] mEnemyTransforms;
        private Vector3 enemyPosition = new Vector3(0f, 200f, 0f);
        private Quaternion enemyRotation = Quaternion.Identity;
        private float enemyScale = 0.002f;

        private const int numEnemies = 10;

        Random random = new Random();

        //Projectile laser;
        private Model mLaser;
        private Matrix[] mLaserTransforms;
        private float laserScale = 1f;
        List<Projectile> laserList = new List<Projectile>();
        List<Projectile> enLaserList = new List<Projectile>(); 
        double lastLaserTime = 0;
        bool firePos = true;


        private Model mEnemyLaser;
        private Matrix[] mEnemyLaserTransforms;

        private Model mTerrain;
        private Matrix[] mTerrainTransforms;
        private Vector3 terrainPosition = new Vector3(0f, -120f, 0f);
        private float terrainScale = 6f;

        private Model mSkyBox;
        private Matrix[] mSkyBoxTransforms;
        private Vector3 skyboxPosition = new Vector3(0f, 100f, 0f);
        private float skyboxScale = 400f;

        Camera mainCamera;
        Camera secCamera;

        Camera selectedCamera;

        private BasicEffect effect;

        GameTime gTime;

        InputManager input;

        private List<BoundingBox> boundingArea = new List<BoundingBox>();

        private BoundingBox frontWall = new BoundingBox(new Vector3(-500f, 0f, 500f), new Vector3(500f, 600f, 800f));
        private BoundingBox backWall = new BoundingBox(new Vector3(-500f, 0f, -500f), new Vector3(500f, 600f, -800f));

        private BoundingBox leftWall = new BoundingBox(new Vector3(500f, 0f, -500f), new Vector3(800f, 600f, 500f));
        private BoundingBox rightWall = new BoundingBox(new Vector3(-500f, 0f, -500f), new Vector3(-800f, 600f, 500f));

        private BoundingBox topWall = new BoundingBox(new Vector3(-500f, 400f, -500f), new Vector3(500f, 800f, 500f));
        private BoundingBox bottomWall = new BoundingBox(new Vector3(-500f, -50f, -500f), new Vector3(500f, 150f, 500f));

        private bool outsideBounds = false;
        private string warningMsg;
        private double countDown;
        private double timer = 5;
        private double counter;

        Texture2D empty;
        Texture2D gradient;
        Texture2D crosshair;

        float screenWidth;
        float screenHeight;

        //SoundEffect enemyFire;
        //public SoundEffectInstance enemyFireFX;

        public SoundEffect enemySound;
        public SoundEffectInstance enemySoundFX;

        public SoundEffect playerSound;
        public SoundEffectInstance playerSoundFX;

        public SoundEffect boostSound;
        public SoundEffectInstance playerBoostFX;

        public SoundEffect enemyFire;
        public SoundEffectInstance enemyFireFX;

        public SoundEffect playerFire;
        public SoundEffectInstance playerFireFX;

        public SoundEffect enemyExplode;
        public SoundEffectInstance enemyExplodeFX;

        public SoundEffect playerExplode;
        public SoundEffectInstance playerExplodeFX;

        Song backgroundMusic;

        public AudioEmitter emitter = new AudioEmitter();
        public AudioListener listener = new AudioListener();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ScreenSize(false);

            //this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "Gavin Whitehall - Coursework";

            mainCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero);
            Components.Add(mainCamera);

            secCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero);
            Components.Add(secCamera);

            player = new Player(this, this, mainCamera, secCamera, playerPosition, Quaternion.Identity);
            Components.Add(player);

            selectedCamera = mainCamera;
            effect = new BasicEffect(GraphicsDevice);
            input = new InputManager(this, this, mainCamera, player);
            Components.Add(input);

            for (int i = 0; i < numEnemies; i++)
            {
                enemyPosition.X = (float)random.Next(-400,400);
                enemyPosition.Y = (float)random.Next(200, 350);
                enemyPosition.Z = (float)random.Next(-400, 400);

                Enemy enemy = new Enemy(this, this, enemyPosition, enemyRotation, player);
                enemyList.Add(enemy);
                Components.Add(enemy);
            }

            boundingArea.Add(frontWall); 
            boundingArea.Add(backWall);
            boundingArea.Add(leftWall);
            boundingArea.Add(rightWall);
            boundingArea.Add(topWall);
            boundingArea.Add(bottomWall);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\GameFont");

            mPlayer = Content.Load<Model>(".\\Models\\ship");
            mPlayerTransforms = SetupEffectTransformDefaults(mPlayer, true);
            mLaser = Content.Load<Model>(".\\Models\\laser1");
            mLaserTransforms = SetupEffectTransformDefaults(mLaser, true);
            mTerrain = Content.Load<Model>(".\\Models\\terrain");
            mTerrainTransforms = SetupEffectTransformDefaults(mTerrain, true);
            mSkyBox = Content.Load<Model>(".\\Skybox\\skybox");
            mSkyBoxTransforms = SetupEffectTransformDefaults(mSkyBox, false);

            mEnemy = Content.Load<Model>(".\\Models\\enemy");
            mEnemyTransforms = SetupEffectTransformDefaults(mEnemy, true);
            mEnemyLaser = Content.Load<Model>(".\\Models\\EnemyLaser");
            mEnemyLaserTransforms = SetupEffectTransformDefaults(mEnemyLaser, true);

            empty = Content.Load<Texture2D>(".\\GUI\\empty");
            gradient = Content.Load<Texture2D>(".\\GUI\\grad");
            crosshair = Content.Load<Texture2D>(".\\GUI\\crosshair");
            LoadSound();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            for (int i = 0; i < laserList.Count; i++)
            {
                laserList[i].UpdateLaser(gameTime);
            }

            for (int i = 0; i < enLaserList.Count; i++)
            {
                enLaserList[i].UpdateLaser(gameTime);
            }
            //Collisions
            BoundingSphere playerSphere = new BoundingSphere(player.Position, 5f);

            for (int i = 0; i < laserList.Count; i++)
            {
                BoundingSphere laserSphere = new BoundingSphere(laserList[i].Position, 1f);

                for (int j = 0; j < boundingArea.Count; j++)
                {
                    if (laserSphere.Intersects(boundingArea[j]))
                    {
                        laserList[i].isActive = false;
                        laserList.Remove(laserList[i]);
                    }
                }
                for (int e = 0; e < enemyList.Count; e++)
                {
                    BoundingSphere enemySphere = new BoundingSphere(enemyList[e].Position, mEnemy.Meshes[0].BoundingSphere.Radius * enemyScale);

                    if (laserSphere.Intersects(enemySphere))
                    {
                        enemyExplodeFX = enemyExplode.CreateInstance();
                        enemyExplodeFX.Apply3D(listener, enemyList[e].emitter);
                        enemyExplodeFX.Play();
                        enemyExplodeFX.Apply3D(listener, enemyList[e].emitter);

                        enemyList[e].isActive = false;
                        enemyList.Remove(enemyList[e]);
                        laserList[i].isActive = false;
                        laserList.Remove(laserList[i]);
                    }
                }



            }

            for (int i = 0; i < enLaserList.Count; i++)
            {
                BoundingSphere enLaserSphere = new BoundingSphere(enLaserList[i].Position, 1f);

                for (int j = 0; j < boundingArea.Count; j++)
                {
                    if (enLaserSphere.Intersects(boundingArea[j]))
                    {
                        enLaserList[i].isActive = false;
                        enLaserList.Remove(enLaserList[i]);
                    }
                }

                if (enLaserSphere.Intersects(playerSphere))
                {
                    player.Damage();
                    enLaserList[i].isActive = false;
                    enLaserList.Remove(enLaserList[i]);
                }

                

            }

            outsideBounds = false;
            for (int i = 0; i < boundingArea.Count; i++)
            {
                if (playerSphere.Intersects(boundingArea[i]))
                {
                    OutsideBounds(boundingArea[i]);
                    outsideBounds = true;
                }
            }
            
            if (!outsideBounds)
                counter = 0;

            gTime = gameTime;
            Matrix playerRot = Matrix.CreateFromQuaternion(player.Rotation);
            listener.Position = selectedCamera.Position;
            listener.Forward = playerRot.Forward;
            listener.Up = playerRot.Up;
                  
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            //graphics.GraphicsDevice.RasterizerState = rs;

            DrawSkyBox();

            Matrix playerTransform =  Matrix.CreateScale(playerScale) * Matrix.CreateFromQuaternion(player.Rotation) * Matrix.CreateTranslation(player.Position);
            DrawModel(mPlayer, playerTransform, mPlayerTransforms);

            Matrix terrainTransform = Matrix.CreateScale(terrainScale) * Matrix.CreateTranslation(terrainPosition);
            DrawModel(mTerrain, terrainTransform, mTerrainTransforms);

            for (int i = 0; i < enemyList.Count; i++)
            {
                Matrix enemyTransform = Matrix.CreateScale(enemyScale) * Matrix.CreateFromQuaternion(enemyList[i].Rotation) * Matrix.CreateTranslation(enemyList[i].Position);
                DrawModel(mEnemy, enemyTransform, mEnemyTransforms);
            }

            DrawLaser();

            double boostTimer = Math.Round(player.boostTimer);
            int booster = (int)player.boostTimer;

            DrawGUI(gradient, new Vector2(screenWidth / 1.3f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5), Color.DarkGray, false);
            DrawGUI(gradient, new Vector2(screenWidth / 1.3f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5 * (boostTimer / 100)), Color.DeepSkyBlue, false);

            DrawGUI(gradient, new Vector2(screenWidth / 30f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5), Color.DarkGray, false);
            DrawGUI(gradient, new Vector2(screenWidth / 30f, screenHeight / 20f), (int)(screenHeight / 35), (int)(screenWidth / 5 * ((double)player.health / 100)), Color.DarkRed, false);

            DrawGUI(empty, new Vector2(screenWidth / 2f, screenHeight / 16f), (int)(screenHeight / 10), (int)(screenWidth / 10), Color.DarkGray, true);
            WriteText(enemyList.Count.ToString(), new Vector2(screenWidth / 2f, screenHeight / 16f), Color.White, true);

            if (outsideBounds)
            {
                WriteText(warningMsg + Math.Round(countDown).ToString() + " SECONDS!", new Vector2(screenWidth / 2, screenHeight /3), Color.Green, true);
            }

            if (selectedCamera == mainCamera)
            {
                DrawGUI(crosshair, new Vector2(screenWidth / 2, screenHeight / 2), (int)(screenWidth * 0.04), (int)(screenHeight * 0.07), Color.Red, true);
            }

            base.Draw(gameTime);
        }

        public void LoadSound()
        {
                                 
            playerSound = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-move3.wav"));
            playerSoundFX = playerSound.CreateInstance();
            playerSoundFX.IsLooped = true;
            playerSoundFX.Apply3D(listener, emitter);

            boostSound = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-move.wav"));
            playerBoostFX = boostSound.CreateInstance();
            playerBoostFX.IsLooped = false;
            playerBoostFX.Apply3D(listener, emitter);

            enemyExplode = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\enemy-explode.wav"));

            playerExplode = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-explode.wav"));
            playerExplodeFX = playerExplode.CreateInstance();
            playerExplodeFX.Apply3D(listener, emitter);

            enemyFire = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\enemy-fire.wav"));

            playerFire = SoundEffect.FromStream(TitleContainer.OpenStream(@"Content\\Sounds\\player-fire.wav"));

            backgroundMusic = Content.Load<Song>(".\\Sounds\\music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.IsRepeating = true;
        }
            
        public void OutsideBounds(BoundingBox boxHit)
        {
            counter += gTime.ElapsedGameTime.TotalSeconds;

            if (counter >= timer)
            {
                //Destroy
                player.Destroy();
                outsideBounds = false;
                counter = 0;
            }
            countDown = timer - counter;
            outsideBounds = true;

            if (boxHit == bottomWall)
            {
                warningMsg = "YOU ARE TOO LOW! INCREASE ALTITUDE WITHIN ";
            }
            else if (boxHit == topWall)
            {
                warningMsg = "YOU ARE TOO HIGH! REDUCE ALTITUDE WITHIN ";
            }
            else
            {
                warningMsg = "YOU ARE LEAVING THE BATTLEFIELD! RETURN WITHIN ";
            }
        }

        public void ScreenSize(bool fullscreen)
        {
            if (!fullscreen)
            {
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
                graphics.ApplyChanges();
            }
            else
            {
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.ApplyChanges();
            }
        }

        public void Fire()
        {
            double currentTime = gTime.TotalGameTime.TotalMilliseconds;

            if (currentTime - lastLaserTime > 200)
            {
                firePos = !firePos;
                Projectile newLaser = new Projectile(this, player.Position, player.Rotation, 1f, firePos, true);
                laserList.Add(newLaser);
                lastLaserTime = currentTime;
                
            }

        }
        public void EnemyFire(Vector3 position, Quaternion rotation, bool firePos)
        {
            Projectile newLaser = new Projectile(this, position, rotation, 1f, firePos, false);
            enLaserList.Add(newLaser);
        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel, bool lighting)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (lighting)
                    {
                        effect.EnableDefaultLighting();
                    }
                    effect.View = selectedCamera.View;
                    effect.Projection = selectedCamera.Projection;
                }
            }
            return absoluteTransforms;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = selectedCamera.View;
                    effect.Projection = selectedCamera.Projection;
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
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

            if (centre)
            {
                TexturePos = position - TextureOrigin;
            }
            else
            {
                TexturePos = position;
            }

            Rectangle rect = new Rectangle((int)TexturePos.X, (int)TexturePos.Y, width, height);
            spriteBatch.Draw(image, rect, colour);
            spriteBatch.End();
        }

        public void DrawLaser()
        {
            for (int i = 0; i < laserList.Count; i++)
            {
                Matrix laserTransform = Matrix.CreateScale(laserScale) * Matrix.CreateFromQuaternion(laserList[i].Rotation) * Matrix.CreateTranslation(laserList[i].Position);
                DrawModel(mLaser, laserTransform, mLaserTransforms);
            }

            for (int i = 0; i < enLaserList.Count; i++)
            {
                Matrix enLaserTransform = Matrix.CreateScale(laserScale) * Matrix.CreateFromQuaternion(enLaserList[i].Rotation) * Matrix.CreateTranslation(enLaserList[i].Position);
                DrawModel(mEnemyLaser, enLaserTransform, mEnemyLaserTransforms);
            }
        }

        public void DrawSkyBox()
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            GraphicsDevice.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            GraphicsDevice.DepthStencilState = dss;

            Matrix skyboxTrasform = Matrix.CreateScale(skyboxScale) * Matrix.CreateFromYawPitchRoll(0f, 0f, 0f) * Matrix.CreateTranslation(player.Position.X, skyboxPosition.Y, player.Position.Z);
            DrawModel(mSkyBox, skyboxTrasform, mSkyBoxTransforms);

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;
        }

        public void SetCamera(bool secSelected)
        {
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
            if (isMuted)
            {
                SoundEffect.MasterVolume = 0f;
                MediaPlayer.IsMuted = true;
            }
            else
            {
                SoundEffect.MasterVolume = 1f;
                MediaPlayer.IsMuted = false;
            }
        }
    }
}
