using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GravityTesting
{
    /// <summary>
    /// Represents a game world of <see cref="GameObject"/>s with physics with the world having gravity, air/fluid density, and size.
    /// </summary>
    public class World
    {
        private List<GameObject> _gameObjects = new List<GameObject>();

        public List<GameObject> GameObjects => _gameObjects;

        /// <summary>
        /// This is the amount(constant) of gravitational pull that earth has.
        /// This number represents the rate that objects accelerate towards earth at
        /// a rate of 9.807 m/s^2(meters/second squared) due to the force of gravity.
        /// </summary>
        public Vector2 Gravity{ get; set; }

        /// <summary>
        /// Density of air/fluid. Try 1000 for water.
        /// </summary>
        public float Density { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public void SetGravity(float x, float y)
        {
            Gravity = new Vector2(x, y);
        }

        public void AddGameObject(GameObject obj)
        {
            _gameObjects.Add(obj);
        }

        public GameObject GetGameObject(string name)
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                if(_gameObjects[i].Name == name)
                    return _gameObjects[i];
            }

            return null;
        }
    }
}
