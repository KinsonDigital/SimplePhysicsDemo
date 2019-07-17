using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePhysicsDemo
{
    public static class Extensions
    {
        /// <summary>
        /// Converts the given list of <paramref name="vertices"/> to an array of <see cref="Vector"/>s.
        /// </summary>
        /// <param name="vertices">The list of <see cref="Vector"/>s to convert.</param>
        /// <returns></returns>
        public static Line[] ToLines(this Vector2[] vertices)
        {
            var result = new Line[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                var nextVerticesIndex = i < vertices.Length - 1 ? i + 1 : 0;

                result[i] = new Line(vertices[i], vertices[nextVerticesIndex]);
            }

            return result;
        }
    }
}
