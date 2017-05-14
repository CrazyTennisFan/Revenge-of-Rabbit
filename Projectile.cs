using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _3D_Game
{
    public class Projectile : BasicModel
    {
        // Rotation and movement variables
 
        Vector3 direction;
		Vector3 dropSpeed;
		Vector3 dropAccel;
        float offsetAngle; // To make the carrot spin properly

        public Projectile(Model m, Vector3 Position,
            Vector3 Direction, float yaw, float pitch, float roll)
            : base(m, Position)
        {
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
			dropSpeed = new Vector3(0,0,0);
			dropAccel = new Vector3(0, -.01f, 0);

            // Get angle between the carrot's horizontal direction and (0,0,1), so it always spins vertically
            Vector2 v1 = new Vector2(direction.X, direction.Z);
            Vector2 v2 = new Vector2(0,1);
            float perpDot = v1.X * v2.Y - v1.Y * v2.X;

            offsetAngle = (float)Math.Atan2(perpDot, Vector2.Dot(v1, v2));
        }

        public void Update()
        {
            pitchAngle += 30;

			dropSpeed += dropAccel;
            position += direction + dropSpeed;
      
        }

        public override Matrix GetWorld()
        {
            return Matrix.CreateRotationY(-offsetAngle) * Matrix.CreateRotationX(MathHelper.ToRadians(pitchAngle)) * Matrix.CreateRotationY(offsetAngle) * Matrix.CreateScale(scaleFactor) * Matrix.CreateTranslation(position);
        }
    }
}