using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityTesting
{
    public class GameObject
    {
        public string Name { get; set; }

        public Vector2 Position { get; set; }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }
    }
}
