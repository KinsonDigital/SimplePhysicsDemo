using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePhysicsDemo
{
    public class PhysicsEngine
    {
        private World _world;

        public void SetWorld(World world)
        {
            _world = world;
        }

        public void Update(GameTime gameTime)
        {
            UpdatePhysics(gameTime);

            CheckCollision();
        }

        private void UpdatePhysics(GameTime gameTime)
        {
            foreach (var obj in _world.GameObjects)
            {
                var allForces = new Vector2();//Total forces.  Gravity + air/fluid drag + etc....

                //Add the weight force, which only affects the y-direction (because that's the direction gravity is pulling from)
                //https://www.wikihow.com/Calculate-Force-of-Gravity
                allForces += obj.Mass * _world.Gravity;

                /*Add the air resistance force. This would affect both X and Y directions, but we're only looking at the y-axis in this example.
                    Things to note:
                    1. Multiplying 0.5 is the same as dividing by 2.  The original well known equation in the link below divides by 2 instead of \
                       multiplying by 0.5.
                    2. Mutiplying the -1 constant is to represent the opposite direction that the wind is traveling compared to the direction 
                       the object is traveling
                    3. Multiplying _velocityY * _velocityY is the same thing as _velocity^2 which is in the well known equation in the link below
                */
                http://www.softschools.com/formulas/physics/air_resistance_formula/85/
                allForces += Util.CalculateDragForceOnObject(_world.Density, obj.Drag, obj.SurfaceArea, obj.Velocity);

                //Clamp the total forces
                allForces = Util.Clamp(allForces, -10f, 10f);

                /* Verlet integration for the y-direction
                 * This is the amount the ball will be moving in this frame based on the ball's current velocity and acceleration. 
                 * Part 1: https://www.youtube.com/watch?v=3HjO_RGIjCU
                 * Part 2: https://www.youtube.com/watch?v=pBMivz4rIJY
                 * Refer to C++ code sample and the velocity_verlet() function
                 *      https://leios.gitbooks.io/algorithm-archive/content/chapters/physics_solvers/verlet/verlet.html
                */
                var predictedDelta = Util.IntegrateVelocityVerlet(obj.Velocity, (float)gameTime.ElapsedGameTime.TotalSeconds, obj.Acceleration);

                // The following calculation converts the unit of measure from cm per pixel to meters per pixel
                obj.Position += predictedDelta * 100f;

                /*Update the acceleration in the Y direction to take in effect all of the added forces as well as the mass
                 Find the new acceleration of the object in the Y direction by solving for A(Accerlation) by dividing all
                 0f the net forces by the mass of the object.  This is one way to find out the acceleration.
                 */
                var newAcceleration = allForces / obj.Mass;

                var averageAcceleration = Util.Average(new[] { newAcceleration, obj.Acceleration });

                obj.Velocity += averageAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                obj.Velocity = Util.Clamp(obj.Velocity, -2f, 2f);
            }
        }

        /// <summary>
        /// Checks collision with the edges of the screen.
        /// </summary>
        private void CheckCollision()
        {
            foreach (var obj in _world.GameObjects)
            {
                //Let's do very simple collision detection for the left of the screen
                if (obj.Position.X < 0 && obj.Velocity.X < 0)
                {
                    // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                    obj.SetVelocity(obj.Velocity.X * obj.Restitution, obj.Velocity.Y);

                    // Move the ball back a little bit so it's not still "stuck" in the wall
                    //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                    obj.SetPosition(0, obj.Position.Y);
                }

                //Let's do very simple collision detection for the right of the screen
                if (obj.Position.X + (obj.Radius * 2) > _world.Width && obj.Velocity.X > 0)
                {
                    // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                    obj.SetVelocity(obj.Velocity.X * obj.Restitution, obj.Velocity.Y);

                    // Move the ball back a little bit so it's not still "stuck" in the wall
                    //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                    obj.SetPosition(_world.Width - (obj.Radius * 2), obj.Position.Y);
                }

                //Let's do very simple collision detection for the top of the screen
                if (obj.Position.Y < 0 && obj.Velocity.Y < 0)
                {
                    // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                    obj.SetVelocity(obj.Velocity.X, obj.Velocity.Y * obj.Restitution);

                    // Move the ball back a little bit so it's not still "stuck" in the wall
                    //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                    obj.SetPosition(obj.Position.X, obj.Position.Y);
                }

                //Let's do very simple collision detection for the bottom of the screen
                if (obj.Position.Y + (obj.Radius * 2) > _world.Height && obj.Velocity.Y > 0)
                {
                    // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                    obj.SetVelocity(obj.Velocity.X, obj.Velocity.Y * obj.Restitution);

                    // Move the ball back a little bit so it's not still "stuck" in the wall
                    //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                    obj.SetPosition(obj.Position.X, _world.Height - (obj.Radius * 2));
                }
            }
        }
    }
}
