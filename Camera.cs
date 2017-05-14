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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace _3D_Game
{

    public class Camera : Microsoft.Xna.Framework.GameComponent
    {


        Game1.GameState currentGameState;

        //Camera matrices
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        // Camera vectors
        public Vector3 cameraPosition; 
        Vector3 cameraDirection;
        Vector3 cameraUp;


        private Vector3 defaultCameraPosition;
        private Vector3 defaultCameraDirection;

        // Mouse stuff
        MouseState baseMouseState;

        // Max yaw/pitch variables
        float currentYaw = 0;
        float totalPitch = MathHelper.Pi/10 ;
        float currentPitch = 0;

        public Vector3 GetCameraDirection
        {
            get { return cameraDirection; }
        }


        public void setGameState(Game1.GameState state)
        {
            currentGameState = state;
        }


        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            // Build camera view matrix
            cameraUp = up;
            CreateLookAt();

            defaultCameraPosition = pos;
            defaultCameraDirection = target - pos;

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 3000);
        }

        public override void Initialize()
        {            
            // Set mouse position and do initial get state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                Game.Window.ClientBounds.Height / 2);
            baseMouseState = Mouse.GetState();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (currentGameState == Game1.GameState.Initial)
            {
                cameraPosition = defaultCameraPosition;
                cameraDirection = defaultCameraDirection;
                cameraDirection.Normalize();
                currentPitch = 0;
            }

            if (currentGameState == Game1.GameState.Run)
            {
                // Yaw rotation
                float yawAngle = (-MathHelper.PiOver4 / 150) *
                        (Mouse.GetState().X - baseMouseState.X);

                if (yawAngle == 0 && GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < 0)                                           
                {
                    yawAngle = (float)Math.PI / 90;
                }
                else if (yawAngle == 0 && GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > 0)                                      
                {
                    yawAngle = -(float)Math.PI / 90;
                }

                cameraDirection = Vector3.Transform(cameraDirection,
                    Matrix.CreateFromAxisAngle(cameraUp, yawAngle));

                currentYaw += yawAngle;

                // Pitch rotation                
                float pitchAngle = (MathHelper.PiOver4 / 150) *
                    (Mouse.GetState().Y - baseMouseState.Y);

                if (pitchAngle == 0 && GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > 0)
                {
                    pitchAngle = -(float)Math.PI / 90;
                }
                else if (pitchAngle == 0 && GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < 0)                                      
                {
                    pitchAngle = (float)Math.PI / 90;
                }
                 

                
                if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
                {
                    cameraDirection = Vector3.Transform(cameraDirection,
                        Matrix.CreateFromAxisAngle(
                            Vector3.Cross(cameraUp, cameraDirection),
                            pitchAngle));

                    currentPitch += pitchAngle;
                }
			    
                // Reset mouse position
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2,
                    Game.Window.ClientBounds.Height / 2);

                // Recreate the camera view matrix
                CreateLookAt();
            }

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition,
                cameraPosition + cameraDirection, cameraUp);
        }
    }
}