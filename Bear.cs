using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    public class Bear : BasicModel
    {        
        public Vector3 facingDirection{get; private set;}
        public Vector3 movingDirection{get; private set;}

        public static int maxHealth {get; private set;}
        public int curHealth {get; private set;}
		public float legAngle { get; private set; }
        public float bodyAngle { get; private set; }
		private bool legUp = true;
        private bool bodyDown = true;
        private int attackSpeed = 1;
        private float randomWalkTimer = 5000;
        private float randomWalkThreshold = 5000;
        public bool isRandomWalking() { return randomWalkTimer < randomWalkThreshold; }
        private float changeDirTimer = 0;
        private float changeDirThreshold = 2500;
        public Boolean isDying { get; private set; }


        public Bear(Model m, Vector3 Position,
            Vector3 Direction)
            : base(m, Position)
        {
            scale(.5f);
            facingDirection = Vector3.Normalize(Direction);
            maxHealth = 5;
            curHealth = 5;
			legAngle = -45;
            bodyAngle = 0;
            isDying = false;
        }

        // Keep track of direction in which the bear is facing when rotating
        public void rotateY(float angle)
        {
            yawAngle += MathHelper.ToRadians(angle);
            facingDirection = Vector3.Transform(facingDirection, Matrix.CreateRotationY(MathHelper.ToRadians(angle)));
        }

        // Take damage
        public void loseHP (int dmg)
        {
            curHealth -= dmg;
            if (curHealth < 0)
                curHealth = 0;
        }
		
        // Make sure bear faces where it moves
		public void translation(Vector3 direction)
		{
			position += direction;

            // Make bears face its direction of movement
            Vector2 v1 = new Vector2(direction.X, direction.Z);
            Vector2 v2 = new Vector2(facingDirection.X, facingDirection.Z);
            float perpDot = v1.X * v2.Y - v1.Y * v2.X;

            double result = Math.Atan2(perpDot, Vector2.Dot(v1, v2));

            if (Math.Abs(result) > MathHelper.ToRadians(5))
            {
                if (result > 0)
                    rotateY(5);
                else
                    rotateY(-5);
            }
		}

        // Attacking animation
        public void attack()
        {
            if (bodyDown)            
                bodyAngle += attackSpeed;               
            else
                bodyAngle -= attackSpeed;

            if (bodyAngle > 30 || bodyAngle < 0)
                bodyDown = !bodyDown;
        }

        // Animation for bear's legs
		public void moveLegs(float change)
		{
			if (legUp)
				legAngle += change;
			else
				legAngle -= change;

			if (Math.Abs(legAngle) > 45)
				legUp = !legUp;
		}

        // Start the bear random-walking
        public void startRandomWalk()
        {
            randomWalkTimer = 0;
            changeDirTimer = changeDirThreshold;
        }

        // In case we want to forcibly stop a random-walk
        public void stopRandomWalk()
        {
            randomWalkTimer = randomWalkThreshold;
        }
            
        // Change direction of random-walk
        public void changeDir()
        {
            changeDirTimer = changeDirThreshold;
        }

        // Makes the bear walk in random directions
        public Vector3 randomWalk(GameTime gameTime)
        {
            randomWalkTimer += gameTime.ElapsedGameTime.Milliseconds;
            changeDirTimer += gameTime.ElapsedGameTime.Milliseconds;
            Random random = new Random();

            if (changeDirTimer >= changeDirThreshold)
            {
                float x = (float)random.NextDouble() * 2f - 1f;
                float z = (float)random.NextDouble() * 2f - 1f;

                movingDirection = Vector3.Normalize(new Vector3(x, 0, z));
                changeDirTimer = 0;
            }

            return movingDirection;
        }

        // Kills the bear
        public void die()
        {
            isDying = true;
        }

        // Draws the bear
		public override void Draw(Camera camera)
		{
			Matrix[] transforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(transforms);

            // Straighten the bear's back if the rabbit's out of range
            if (Vector3.Distance(camera.cameraPosition, position) > 10)
            {
                if(bodyAngle > 0)
                    bodyAngle -= attackSpeed;
                bodyDown = false;
            }


			List<Matrix> meshWorlds = new List<Matrix>();
            meshWorlds.Add(Matrix.CreateRotationX(MathHelper.ToRadians(bodyAngle)));
            meshWorlds.Add(Matrix.CreateTranslation(0, -5, 0) * Matrix.CreateRotationX(MathHelper.ToRadians(legAngle)) * Matrix.CreateTranslation(0, 5, 0));
            meshWorlds.Add(Matrix.CreateTranslation(0, -5, 0) * Matrix.CreateRotationX(MathHelper.ToRadians(-legAngle)) * Matrix.CreateTranslation(0, 5, 0));

			for (int i=0; i< model.Meshes.Count; i++)
			{
				foreach (BasicEffect be in model.Meshes[i].Effects)
				{
					be.EnableDefaultLighting();
					be.Projection = camera.projection;
					be.View = camera.view;
					be.World = meshWorlds[i]*GetWorld() * model.Meshes[i].ParentBone.Transform;
				}

				model.Meshes[i].Draw();
			}
		}
    }
}
