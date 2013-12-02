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
        private Vector3 playerPosition = new Vector3(10f, 1f, 5f);
        private Vector3 playerRotation = Vector3.Zero;
        private float playerScale = 0.01f;

        Projectile laser;
        private Model mLaser;
        private Matrix[] mLaserTransforms;
        private float laserScale = 1f;

        private Model mTerrain;
        private Matrix[] mTerrainTransforms;
        private Vector3 terrainPosition = Vector3.Zero;
        private float terrainScale = 0.01f;

        private Model mSkyBox;
        private Matrix[] mSkyBoxTransforms;
        private Vector3 skyboxPosition;
        private float skyboxScale = 150f;


        Camera mainCamera;
        Floor floor;
        private BasicEffect effect;
        
        InputManager input;

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = mainCamera.View;
                    effect.Projection = mainCamera.Projection;

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
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.Title = "Gavin Whitehall - Coursework";
            mainCamera = new Camera(this, new Vector3(10f, 1f, 5f), Vector3.Zero, 5f);
            Components.Add(mainCamera);
            player = new Player(this, mainCamera, playerPosition, Quaternion.Identity);
            Components.Add(player);
            input = new InputManager(this, mainCamera, player);
            Components.Add(input);
            laser = new Projectile(this, player);
            Components.Add(laser);
            floor = new Floor(GraphicsDevice, 50, 50);
            effect = new BasicEffect(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\GameFont");

            mPlayer = Content.Load<Model>(".\\Models\\ship");
            mPlayerTransforms = SetupEffectTransformDefaults(mPlayer);
            mLaser = Content.Load<Model>(".\\Models\\laser");
            mLaserTransforms = SetupEffectTransformDefaults(mLaser);
            mTerrain = Content.Load<Model>(".\\Models\\dalek");
            mTerrainTransforms = SetupEffectTransformDefaults(mTerrain);
            mSkyBox = Content.Load<Model>(".\\Skybox\\skybox");
            mSkyBoxTransforms = SetupEffectTransformDefaults(mSkyBox);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Console.Write(player.Position);

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
            //Matrix playerTransform = Matrix.CreateScale(playerScale) * Matrix.CreateTranslation(player.Position);
            //Matrix playerTransform = (Matrix.CreateRotationX(player.Rotation.X) * Matrix.CreateRotationY(player.Rotation.Y) * 
            //    Matrix.CreateRotationZ(player.Rotation.Z)) * Matrix.CreateTranslation(player.Position);
            //Matrix playerTransform = Matrix.CreateRotationY(player.Rotation.Y) * Matrix.CreateTranslation(player.Position);

            //Matrix playerTransform = Matrix.CreateScale(playerScale) * Matrix.CreateFromYawPitchRoll(player.Rotation.X, player.Rotation.Y, player.Rotation.Z) * Matrix.CreateTranslation(player.Position);
            Matrix playerTransform =  Matrix.CreateScale(playerScale) * Matrix.CreateFromQuaternion(player.Rotation) * Matrix.CreateTranslation(player.Position);
            DrawModel(mPlayer, playerTransform, mPlayerTransforms);

            Matrix terrainTransform = Matrix.CreateScale(terrainScale) * Matrix.CreateTranslation(terrainPosition);
            DrawModel(mTerrain, terrainTransform, mTerrainTransforms);            

            Matrix laserTransform = Matrix.CreateScale(laserScale) * Matrix.CreateFromYawPitchRoll(laser.Rotation.X, laser.Rotation.Y, laser.Rotation.Z) * Matrix.CreateTranslation(laser.Position);
            DrawModel(mLaser, laserTransform, mLaserTransforms);

            String boost = player.boostTimer.ToString();
            WriteText(boost, new Vector2(50f,50f), Color.Black);

            base.Draw(gameTime);
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

            Matrix skyboxTrasform = Matrix.CreateScale(skyboxScale) * Matrix.CreateFromYawPitchRoll(0f, 0f, 0f) * Matrix.CreateTranslation(new Vector3(player.Position.X, 0f , player.Position.Z));
            DrawModel(mSkyBox, skyboxTrasform, mSkyBoxTransforms);

            //int i = 0;
            //foreach (ModelMesh mesh in mSkyBox.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.View = mainCamera.View;
            //        effect.Projection = mainCamera.Projection;
            //        effect.World = mSkyBoxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(player.Position);

            //        //effect.CurrentTechnique = effect.Techniques["Textured"];
            //        //effect.Parameters["xWorld"].SetValue(worldMatrix);
            //        //effect.Parameters["xView"].SetValue(viewMatrix);
            //        //effect.Parameters["xProjection"].SetValue(projectionMatrix);
            //        //effect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
            //    }
            //    mesh.Draw();
            //}

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = dss;
        }
    }
}
