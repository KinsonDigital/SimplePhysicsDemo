using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePhysicsDemo
{
    public class RectObject
    {
        private Vector2[] _shapeVertices;
        private Vector2[] _worldVertices;
        private Vector2 _position;
        private Line[] _sides;
        private float _scale = 1f;

        public RectObject(Vector2[] vertices, Vector2 position)
        {
            if (vertices.Length < 4 || vertices.Length > 4)
                throw new Exception("Must have only 4 vertices");

            _position = position;
            _shapeVertices = vertices;
            _worldVertices = new Vector2[vertices.Length];
            _sides = new Line[vertices.Length];

            UpdateVertices(Vector2.Zero);
        }

        public string Name { get; set; }

        public float Width
        {
            get
            {
                return Radius * 2;
            }
        }

        public float Height
        {
            get
            {
                return Radius * 2;
            }
        }

        public Vector2[] WorldVertices => _worldVertices;

        public Line[] Sides => _sides;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                var amount = _position - value;

                _position = value;

                UpdateVertices(amount);
            }
        }

        public Vector2 Velocity { get; set; }

        public float Angle { get; set; }//Radians

        public float AngularVelocity { get; set; }

        public float AngularAcceleration { get; set; }

        public float AngularForce { get; set; }

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

        private void UpdateVertices(Vector2 positionDelta)
        {
            //Update the position of all the vertices
            if (positionDelta.X != 0 && positionDelta.Y != 0)
            {
                for (int i = 0; i < _worldVertices.Length; i++)
                {
                    _worldVertices[i] += positionDelta;
                }
            }

            //Calculate the scale of the polygon
            var scaledLines = Util.Scale(_shapeVertices.ToLines(), _scale);

            var scaledVertices = scaledLines.ToVertices();

            //Calculate the world vertices
            var unrotatedWorldVertices = Util.ConvertToWorldVertices(scaledVertices, _position);

            for (int i = 0; i < _worldVertices.Length; i++)
            {
                _worldVertices[i] = Util.RotateAround(unrotatedWorldVertices[i], _position, Angle);
            }

            //Create the sides of the polygon
            for (int i = 0; i < _sides.Length; i++)
            {
                _sides[i].Start = _worldVertices[i];
                _sides[i].Stop = _worldVertices[i < _worldVertices.Length - 1 ? i + 1 : 0];
            }
        }
    }
}
