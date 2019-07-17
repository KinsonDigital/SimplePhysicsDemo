using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePhysicsDemo
{
    /// <summary>
    /// Represents a 2D line with a start and stop vector.
    /// </summary>
    public struct Line
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Line"/>.
        /// </summary>
        /// <param name="start">The starting <see cref="Vector"/> of the line.</param>
        /// <param name="stop">The ending <see cref="Vector"/> of the line.</param>
        public Line(Vector2 start, Vector2 stop)
        {
            Start = start;
            Stop = stop;
        }
        #endregion

        #region Props
        /// <summary>
        /// The starting <see cref="Vector"/> of the line.
        /// </summary>
        public Vector2 Start { get; set; }

        /// <summary>
        /// The ending <see cref="Vector"/> of the line.
        /// </summary>
        public Vector2 Stop { get; set; }

        /// <summary>
        /// Returns a valud indicating if the line has a length;
        /// </summary>
        public bool HasLength => Start.X != Stop.X || Start.Y != Stop.Y;

        /// <summary>
        /// Moves the entire line to the right.
        /// </summary>
        /// <param name="amount">The amount to move the line from its current position.</param>
        public void MoveRight(int amount)
        {
            Start = new Vector2(Start.X + amount, Start.Y);
            Stop = new Vector2(Stop.X + amount, Stop.Y);
        }

        /// <summary>
        /// Moves the entire line to the left.
        /// </summary>
        /// <param name="amount">The amount to mvoe the line from its current position.</param>
        public void MoveLeft(int amount)
        {
            MoveRight(amount * -1);
        }

        /// <summary>
        /// Moves the entire line down.
        /// </summary>
        /// <param name="amount">The amount to mvoe the line from its current position.</param>
        public void MoveDown(int amount)
        {
            Start = new Vector2(Start.X, Start.Y + amount);
            Stop = new Vector2(Stop.X, Stop.Y + amount);
        }

        /// <summary>
        /// Moves the entire line up.
        /// </summary>
        /// <param name="amount">The amount to mvoe the line from its current position.</param>
        public void MoveUp(int amount)
        {
            Start = new Vector2(Start.X, Start.Y - amount);
            Stop = new Vector2(Stop.X, Stop.Y - amount);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the <see cref="Line"/> as a string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Start.ToString()} - {Stop.ToString()}";
        }
        #endregion

        #region Static Methods
        public static Line CreateLine(Vector2 v1, Vector2 v2)
        {
            return new Line(v1, v2);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Overloaded Operators
        /// <summary>
        /// Returns a boolean value indicating if 2 lines are equal.
        /// </summary>
        /// <param name="lineA">The first line in the comparison.</param>
        /// <param name="lineB">The second line in the comparison.</param>
        /// <returns></returns>
        public static bool operator ==(Line lineA, Line lineB)
        {
            return lineA.Start == lineB.Start && lineA.Stop == lineB.Stop;
        }

        /// <summary>
        /// Returns a boolean value indicating if the 2 lines are not equal.
        /// </summary>
        /// <param name="lineA">The first line in the comparison.</param>
        /// <param name="lineB">The second line in the comparison.</param>
        /// <returns></returns>
        public static bool operator !=(Line lineA, Line lineB)
        {
            return !(lineA == lineB);
        }
        #endregion
    }
}
