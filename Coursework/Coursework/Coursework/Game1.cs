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

        //Projectile laser;
        private Model mLaser;
        private Matrix[] mLaserTransforms;
        private float laserScale = 1f;
        List<Projectile> laserList = new List<Projectile>(); 
        double lastLaserTime = 0;
        bool firePos = true;

        private Model mTerrain;
        private Matrix[] mTerrainTransforms;
        private Vector3 terrainPosition = new Vector3(0f, -120f, 0f);
        private float terrainScale = 6f;

        private Model mEnemy;
        private Matrix[] mEnemyTransforms;
        private Vector3 enemyPosition = new Vector3(0f, 200f, 0f);
        private float enemyScale = 0.002f;

        private Model mSkyBox;
        private Matrix[] mSkyBoxTransforms;
        private Vector3 skyboxPosition = new Vector3(0f, -0f, 0f);
        private float skyboxScale = 400f;

        Camera mainCamera;
        Floor floor;
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
            mainCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero, 5f);
            Components.Add(mainCamera);
            player = new Player(this, mainCamera, playerPosition, Quaternion.Identity);
            Components.Add(player);
            
            floor = new Floor(GraphicsDevice, 50, 50);
            effect = new BasicEffect(GraphicsDevice);
            input = new InputManager(this, this, mainCamera, player);
            Components.Add(input);
            base.Initialize();

            boundingArea.Add(frontWall); 
            boundingArea.Add(backWall);
            boundingArea.Add(leftWall);
            boundingArea.Add(rightWall);
            boundingArea.Add(topWall);
            boundingArea.Add(bottomWall);
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
            mEnemyTransforms = SetupEffectTransformDefaults(mEnemy, false);

            empty = Content.Load<Texture2D>(".\\GUI\\empty");
            gradient = Content.Load<Texture2D>(".\\GUI\\grad");
            crosshair = Content.Load<Texture2D>(".\\GUI\\crosshair");

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

            //Collisions
            BoundingSphere playerSphere = new BoundingSphere(player.Position, mPlayer.Meshes[0].BoundingSphere.Radius * playerScale);
            BoundingBox floorBox = new BoundingBox (Vector3.Zero, new Vector3(40,40,40));

            for (int i = 0; i < laserList.Count; i++)
            {
                BoundingSphere laserSphere = new BoundingSphere(laserList[i].Position, 1f);

                for (int j = 0; j < boundingArea.Count; j++)
                {
                    if (laserSphere.Intersects(boundingArea[j]))
                    {
                        laserList.Remove(laserList[i]);
                    }
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

            Console.Write(laserList.Count+" ");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            //graphics.GraphicsDevice.RasterizerState = rs;

            DrawSkyBox();

            floor.Draw(mainCamera, effect);

            Matrix playerTransform =  Matrix.CreateScale(playerScale) * Matrix.CreateFromQuaternion(player.Rotation) * Matrix.CreateTranslation(player.Position);
            DrawModel(mPlayer, playerTransform, mPlayerTransforms);

            Matrix terrainTransform = Matrix.CreateScale(terrainScale) * Matrix.CreateTranslation(terrainPosition);
            DrawModel(mTerrain, terrainTransform, mTerrainTransforms);

            Matrix enemyTransform = Matrix.CreateScale(enemyScale) * Matrix.CreateFromQuaternion(Quaternion.Identity) * Matrix.CreateTranslation(enemyPosition);
            DrawModel(mEnemy, enemyTransform, mEnemyTransforms);

            DrawLaser();

            double boostTimer = Math.Round(player.boostTimer);
            String boost = boostTimer.ToString();
            int booster = (int)player.boostTimer;

            DrawGUI(empty, new Vector2(screenWidth / 1.3f, screenHeight / 20), (int)(screenHeight / 35), (int)(screenWidth / 5), Color.DarkGray, false);
            DrawGUI(gradient, new Vector2(screenWidth / 1.3f, screenHeight /20), (int)(screenHeight / 35), (int)(screenWidth / 5 * (boostTimer / 100)), Color.DeepSkyBlue, false);
           

            WriteText(boost, new Vector2(screenWidth / 1.3f, screenHeight / 20), Color.White);

            if (outsideBounds)
            {
                WriteText(warningMsg + Math.Round(countDown).ToString() + " SECONDS!", new Vector2(screenWidth / 2, screenHeight /3), Color.Green);
            }

            DrawGUI(crosshair, new Vector2(screenWidth / 2, screenHeight / 2), (int)(screenWidth * 0.04), (int)(screenHeight * 0.07), Color.Red, true);

            base.Draw(gameTime);
        }

        public void OutsideBounds(BoundingBox boxHit)
        {
            counter += gTime.ElapsedGameTime.TotalSeconds;

            if (counter >= timer)
            {
                //Destroy
                player.Position = playerPosition;
                player.Rotation = Quaternion.Identity;
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

            if (currentTime - lastLaserTime > 50)
            {
                firePos = !firePos;
                Projectile newLaser = new Projectile(player.Position, player.Rotation, 1f, firePos);
                laserList.Add(newLaser);

                lastLaserTime = currentTime;
            }

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
                    effect.View = mainCamera.View;
                    effect.Projection = mainCamera.Projection;
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
                    effect.View = mainCamera.View;
                    effect.Projection = mainCamera.Projection;
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        private void WriteText(string msg, Vector2 msgPos, Color msgColour)
        {
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos - FontOrigin;
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
    }
}
