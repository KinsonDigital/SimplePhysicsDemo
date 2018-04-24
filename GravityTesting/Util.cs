using Microsoft.Xna.Framework;

namespace GravityTesting
{
    /// <summary>
    /// Provides simple untility methods.
    /// </summary>
    public static class Util
    {
        public static Point ToPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>
        /// Square the given <paramref name="value"/> and return he result.
        /// </summary>
        /// <param name="value">The value to square.</param>
        /// <returns></returns>
        public static float Square(float value)
        {
            return value * value;
        }


        /// <summary>
        /// Returns an average of all the given <paramref name="values"/> of type <see cref="float"/>.
        /// </summary>
        /// <param name="values">The list of values to average.</param>
        /// <returns></returns>
        public static float Average(float[] values)
        {
            var sum = 0f;

            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }

            return sum / values.Length;
        }


        /// <summary>
        /// Clamps the given <paramref name="value"/> between the given <paramref name="minimum"/> and <paramref name="maximum"/> values.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="minimum">The minimum that the value should be.</param>
        /// <param name="maximum">The maximum that the value should be.</param>
        /// <returns></returns>
        public static float Clamp(float value, float minimum, float maximum)
        {
            value = value < minimum ? minimum : value;
            value = value > maximum ? maximum : value;

            return value;
        }


        /// <summary>
        /// Clamps the given <paramref name="value"/> between the given <paramref name="minimum"/> and <paramref name="maximum"/> values.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="minimum">The minimum that the value should be.</param>
        /// <param name="maximum">The maximum that the value should be.</param>
        /// <returns></returns>
        public static Vector2 Clamp(Vector2 value, float minimum, float maximum)
        {
            value.X = Clamp(value.X, minimum, maximum);
            value.Y = Clamp(value.Y, minimum, maximum);

            return value;
        }


        /// <summary>
        /// Returns an average of all the given <paramref name="values"/> of type <see cref="Vector2"/>.
        /// </summary>
        /// <param name="values">The list of vectors to average.</param>
        /// <returns></returns>
        public static Vector2 Average(Vector2[] values)
        {
            var sum = Vector2.Zero;

            for (int i = 0; i < values.Length; i++)
            {
                sum.X += values[i].X;
                sum.Y += values[i].Y;
            }

            return new Vector2(sum.X / values.Length, sum.Y / values.Length);
        }


        /// <summary>
        /// This performs a verlet velocity integration on a single axis.
        /// </summary>
        /// <param name="velocity">The velocity on a single axis.  Must be the same axis as the <paramref name="acceleration"/> param.</param>
        /// <param name="dt">The delta time in seconds of the current frame.</param>
        /// <param name="acceleration">The current accerlation on a single axis.  Must be the same axis as the <paramref name="velocity"/> param.</param>
        /// <remarks>Refer to links for more information.
        /// Videos:
        ///     Part 1: https://www.youtube.com/watch?v=3HjO_RGIjCU
        ///     Part 2: https://www.youtube.com/watch?v=pBMivz4rIJY
        /// Other: https://leios.gitbooks.io/algorithm-archive/content/chapters/physics_solvers/verlet/verlet.html
        /// </remarks>
        /// <returns></returns>
        public static Vector2 IntegrateVelocityVerlet(Vector2 velocity, float dt, Vector2 acceleration)
        {
            return velocity * dt + (0.5f * acceleration * Square(dt));
        }


        /// <summary>
        /// Calculates the drag force of air/fluid on the surface of an object.
        /// </summary>
        /// <param name="fluidDensity"></param>
        /// <param name="dragCoefficient"></param>
        /// <param name="surfaceAreaInContact"></param>
        /// <param name="velocity"></param>
        /// <remarks>
        /// Refer to links
        /// 1. http://www.softschools.com/formulas/physics/air_resistance_formula/85/ for information.
        /// 2. https://www.khanacademy.org/computing/computer-programming/programming-natural-simulations/programming-forces/a/air-and-fluid-resistance
        /// </remarks>
        /// <returns></returns>
        public static Vector2 CalculateDragForceOnObject(float fluidDensity, float dragCoefficient, float surfaceAreaInContact, Vector2 velocity)
        {
            return -1 * ((fluidDensity * dragCoefficient * surfaceAreaInContact) / 2.0f) * (velocity * velocity);
        }
    }
}
