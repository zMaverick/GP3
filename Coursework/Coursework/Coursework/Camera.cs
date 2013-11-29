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
    public class Camera
    {
        //Variables
        //GraphicsDeviceManager graphics;
        

       // private Vector3 cameraPosition;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        private int fov;
        private float ratio;
        private float avatarYaw;

        private float deltaTime;
        public Vector3 cameraPosition;
        public Vector3 Up { get; private set; }
        public Vector3 Forward { get; private set; }

        public Camera(Vector3 position, Vector3 forward, Vector3 up, float aspectRatio, int fieldofView)
        {
            cameraPosition = position;
            Forward = forward;
            Up = up;

            ratio = aspectRatio;
            fov = fieldofView;
            //UpdateCamera();
        }

        //Translate
        public void Move(float speed)
        {
            //Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
            //Vector3 v = new Vector3(0, 0, speed);
            //v = Vector3.Transform(v, forwardMovement);
            //cameraPosition.Z += v.Z;
            //cameraPosition.X += v.X;
        }

        public void Update(float delta)
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(Forward, rotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookAt = cameraPosition + transformedReference;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), ratio, 1.0f, 350.0f);

            deltaTime = delta;
        }


        //public Matrix getViewMatrix()
        //{
        //    return viewMatrix;
        //}

        //public Matrix getProjectionMatrix()
        //{
        //    return projectionMatrix;
        //}


    }
}
