using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    public class Background : BasicModel
    {
            public Background(Model m, Vector3 Position)
            : base(m)
        {    
            position = Position;
                
        }       

        public void scale(float x, float y, float z)
        {
            curRotMatrix = curRotMatrix * Matrix.CreateScale(x, y, z);
        }
    
    }
}
