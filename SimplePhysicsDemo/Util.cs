using Microsoft.Xna.Framework;
using System;

namespace SimplePhysicsDemo
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

        public static float ToRadians(float degrees)
        {
            return degrees * 3.1415926535897931f / 180f;
        }

        /// <summary>
        /// Converts the given radians to degrees.
        /// </summary>
        /// <param name="radians">The radians to convert.</param>
        /// <returns></returns>
        public static float ToDegrees(float radians)
        {
            return radians * 180.0f / 3.1415926535897931f;
        }


        public static Vector2 RotateAround(Vector2 vector, Vector2 origin, float angle, bool clockWise = true)
        {
            //if (angle < 0)
            //    throw new ArgumentOutOfRangeException(nameof(angle), "The angle must be a positive number.");

            angle = clockWise ? angle : angle * -1;

            var radians = ToRadians(angle);

            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);

            var dx = vector.X - origin.X;//The delta x
            var dy = vector.Y - origin.Y;//The delta y

            var tempX = dx * cos - dy * sin;
            var tempY = dx * sin + dy * cos;

            var x = tempX + origin.X;
            var y = tempY + origin.Y;

            return new Vector2(x, y);
        }


        /// <summary>
        /// Converts the given local <paramref name="localVertices"/> to world vertices based on the given <paramref name="origin"/>.
        /// </summary>
        /// <param name="localVertices">The local vertices to translate to world vertices.</param>
        /// <param name="origin">The origin to base the translation from.</param>
        /// <returns></returns>
        public static Vector2[] ConvertToWorldVertices(Vector2[] localVertices, Vector2 origin)
        {
            var worldVertices = new Vector2[localVertices.Length];

            for (int i = 0; i < localVertices.Length; i++)
            {
                worldVertices[i] = origin + localVertices[i];
            }

            return worldVertices;
        }


        /// <summary>
        /// Scales the length of the given <paramref name="line"/> by the given <paramref name="scale"/> amount.
        /// </summary>
        /// <param name="line">The line to to scale.</param>
        /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
        /// <returns></returns>
        public static Line Scale(Line line, float scale)
        {
            line.Start *= scale;
            line.Stop *= scale;

            return line;
        }

        /// <summary>
        /// Scales the given <paramref name="lines"/> by the given <paramref name="scale"/> amount.
        /// </summary>
        /// <param name="lines">The <see cref="Line"/>s to scale by the given <paramref name="scale"/>.</param>
        /// <param name="scale">The amount to scale the line as a percentage. 1 is 100% normal size.</param>
        /// <returns></returns>
        public static Line[] Scale(Line[] lines, float scale)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = Scale(lines[i], scale);
            }

            return lines;
        }


        /// <summary>
        /// Converts the given list of <paramref name="lines"/> to an array of <see cref="Vector"/>s.
        /// </summary>
        /// <param name="lines">The list of <see cref="Line"/>s to convert.</param>
        /// <returns></returns>
        public static Vector2[] ToVertices(this Line[] lines)
        {
            var result = new Vector2[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                result[i] = lines[i].Start;
            }

            return result;
        }


        /// <summary>
        /// Calculates the centroid of the given <see cref="Vector"/>s that make up a polygon.
        /// </summary>
        /// <param name="vertices">The list of <see cref="Vector"/>s of a polygon.</param>
        /// <returns></returns>
        public static Vector2 CalculateCentroid(Vector2[] vertices)
        {
            var sumX = 0f;
            var sumY = 0f;

            for (int i = 0; i < vertices.Length; i++)
            {
                sumX += vertices[i].X;
                sumY += vertices[i].Y;
            }

            return new Vector2(sumX / vertices.Length, sumY / vertices.Length);
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
