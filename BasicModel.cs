using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    public class BasicModel
    {
        public Model model { get; protected set; }
        public float yawAngle{ get; protected set; }
        public float pitchAngle{ get; protected set; } 
        public float rollAngle { get; protected set; }
        public Vector3 scaleFactor {get; protected set;} 
        public Vector3 position { get; protected set; }

        // Basic constructor that takes a Model and Position. Defaults rotation angles to 0 and scaleFactor to (1,1,1)
        public BasicModel(Model m, Vector3 pos)
        {
            model = m;
            position = pos;
            scaleFactor = new Vector3(1,1,1);
            yawAngle = 0;
            pitchAngle = 0;
            rollAngle = 0;
        }

        // This simply draws each model's mesh with default lighting. This is virtual because Bear will override it.
        public virtual void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

        // This returns a sequence of Matrix multiplications using the 3 angles (rotation matrices), scaleFactor (scale matrix), and position (translation matrix).
        public virtual Matrix GetWorld()
        {
            return Matrix.CreateRotationX(pitchAngle) * Matrix.CreateRotationY(yawAngle)
                    * Matrix.CreateRotationZ(rollAngle) * Matrix.CreateScale(scaleFactor) * Matrix.CreateTranslation(position);
        }

        // Returns true if model intersects with target, false otherwise
        public bool CollidesWith(BasicModel target)
        {
			// Preliminary check for to see if the objects are even close
			if (Vector3.Distance(position, target.position) > 10)
				return false;
            // Loop through each ModelMesh in both objects and compare
            // all bounding spheres for collisions
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh otherModelMeshes in target.model.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(
                        GetWorld()).Intersects(
                        otherModelMeshes.BoundingSphere.Transform(target.GetWorld())))
                        return true;
                }
            }
            return false;
        }

        // Rotate by angle about X-axis
        public void rotationX(float angle)
        {
            pitchAngle += (float)(angle * Math.PI / 180);
        }

        // Rotate by angle about Y-axis
        public void rotationY(float angle)
        {
            yawAngle += (float)(angle * Math.PI / 180);
        }

        // Rotate by angle about Z-axis
        public void rotationZ(float angle)
        {
            rollAngle += (float)(angle * Math.PI / 180);
        }

        // Returns projection of the position vector on the XZ plane
        public Vector3 xzPos()
        {
            return new Vector3(position.X, 0, position.Z);
        }

        // Scales the model by different values on different axes
        public void scale(float x, float y, float z)
        {
            scaleFactor = new Vector3(x, y, z);
        }

        // Scales the model by the same value on all axes
        public void scale(float scale)
        {
            scaleFactor = new Vector3(scale, scale, scale);
        }
    }
}
