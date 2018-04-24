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

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; }

        /// <summary>
        /// Coefficient of restitution ("bounciness"). Needs to be a negative number for flipping the direction of travel (velocity Y) to move the ball 
        /// in the opposition direction when it hits a surface.This is what simulates the bouncing effect of an object hitting another object.
        /// </summary>
        public float Restitution { get; set; }

        /// <summary>
        /// Frontal area of the ball; divided by 10000 to compensate for the 1px = 1cm relation
        /// frontal area of the ball is the area of the ball as projected opposite of the direction of motion.
        /// In other words, this is the "silhouette" of the ball that is facing the "wind" (since this variable is used for air resistance calculation).
        ///   It is the total area of the ball that faces the wind. In short: this is the area that the air is pressing on.
        /// http://www.softschools.com/formulas/physics/air_resistance_formula/85/
        /// </summary>
        public float SurfaceArea { get; set; }

        /// <summary>
        /// Coeffecient of drag for on a object
        /// </summary>
        public float Drag { get; set; }

        public float Radius { get; set; }

        public float Mass { get; set; }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }


        public void SetVelocity(float x, float y)
        {
            Velocity = new Vector2(x, y);
        }
    }
}
