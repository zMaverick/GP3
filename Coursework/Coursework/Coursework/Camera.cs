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
    public class Camera : GameComponent
    {
        private Vector3 cameraPosition;     //Cameras position 
        private Vector3 cameraLookAt;       // What the Camera is looking at
        private Vector3 cameraUp;           // The Up Vector

        public Matrix viewMatrix;           //View Matrix Variable

        //Public Camera Position member, gets and sets the private member
        public Vector3 Position
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                //Create and return the View Matix
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraUp);
            }
        }

        // What the Camera is Looking At
        public Vector3 LookAt
        {
            get { return cameraLookAt; }
            set { cameraLookAt = value; }
        }

        // What the Cameras Up Vector is  
        public Vector3 Up
        {
            get { return cameraUp; }
            set { cameraUp = value; }
        }


        public Camera(Game game, Vector3 position, Vector3 rotation)
            : base(game)
        {
            // When a new Camera is Created, Set the Projection Matrix
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                Game.GraphicsDevice.Viewport.AspectRatio,
                0.05f,
                1500.0f);
        }  
    }
}
