using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GravityTesting
{
    /*Resources/Links:
     1. http://buildnewgames.com/gamephysics/ is the website that this comes from
     2. https://www.youtube.com/channel/UCF6F8LdCSWlRwQm_hfA2bcQ Good videos on coding with math
     */

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        private SettingsManager _settingsManager;
        private ScreenStats _screenStats;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int _screenHeight;
        private int _screenWidth;

        private GameObject _box;
        private Vector2 _velocity = new Vector2(0, 0);
        private Vector2 _acceleration = new Vector2();

        private float _mass = 0.1f;//Ball mass in kg
        private float _radius = 50f;//Ball radius in cm or pixels.

        /*This is the amount(constant) of gravitational pull that earth has.
          This number represents the rate that objects accelerate towards earth at 
          a rate of 9.807 m/s^2(meters/second squared) due to the force of gravity.
         */
        private Vector2 _gravity = new Vector2(0f, 0f);

        /* Coefficient of restitution ("bounciness"). Needs to be a negative number for flipping the direction of travel (velocity Y) to move the ball 
           in the opposition direction when it hits a surface. This is what simulates the bouncing effect of an object hitting another object.
        */
        private float _restitutionCoeffecient = 0f;

        private float _fluidDensity = 0f;//Density of air/fluid. Try 1000 for water.
        private float _dragCoeffecient = 0f;//Coeffecient of drag for on a object

        /* Frontal area of the ball; divided by 10000 to compensate for the 1px = 1cm relation
           frontal area of the ball is the area of the ball as projected opposite of the direction of motion.
           In other words, this is the "silhouette" of the ball that is facing the "wind" (since this variable is used for air resistance calculation).
           It is the total area of the ball that faces the wind. In short: this is the area that the air is pressing on.
           http://www.softschools.com/formulas/physics/air_resistance_formula/85/
        */
        private float _surfaceArea = 0f;


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Main"/>.
        /// </summary>
        public Main()
        {
            //Make this game loop a variable time step
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            _surfaceArea = (float)Math.PI * _radius * _radius / 50000f;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        #endregion


        #region MonoGame Methods
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _box = new GameObject()
            {
                Name = "Box",
                Position = new Vector2(350, 200)
            };

            _graphics.PreferredBackBufferHeight = _graphics.PreferredBackBufferHeight + 200;
            _graphics.ApplyChanges();

            _screenHeight = _graphics.PreferredBackBufferHeight;
            _screenWidth = _graphics.PreferredBackBufferWidth;

            var screenCenterX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            var screenCenterY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            var halfWidth = _graphics.GraphicsDevice.Viewport.Width / 2;
            var halfHeight = _graphics.GraphicsDevice.Viewport.Height / 2;

            Window.Position = new Point(screenCenterX - halfWidth, screenCenterY - halfHeight);

            //Add the various settings to the settings manager
            AddSettings();

            //Create screen stats
            CreateScreenStats();

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdatePhysics(frameTime);

            CheckCollision();

            UpdateStats();

            _settingsManager.Update(gameTime);

            base.Update(gameTime);
        }


        /// <summary>
        /// Updates the stat values on the screen.
        /// </summary>
        private void UpdateStats()
        {
            //If the velocity is infinity, just set text to N/A
            var velX = float.IsInfinity(_velocity.X) ? "Inf" : Math.Round(_velocity.X, 2).ToString();
            var velY = float.IsInfinity(_velocity.Y) ? "Inf" : Math.Round(_velocity.Y, 2).ToString();

            _screenStats.UpdateStat("Gravity", $"X: {Math.Round(_gravity.X, 2)} , Y:{Math.Round(_gravity.Y, 2)}");
            _screenStats.UpdateStat("Velocity", $"X: {velX} , Y:{velY}");
            _screenStats.UpdateStat("Bounciness", $"{_restitutionCoeffecient}");
            _screenStats.UpdateStat("Drag", $"{_dragCoeffecient}");
            _screenStats.UpdateStat("FluidDensity", $"{_fluidDensity}");
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.FillRectangle(_box.Position, new Vector2(100, 100), Color.Orange);

            _screenStats.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion


        #region Event Methods
        /// <summary>
        /// Invoked when the setting has been moved to the next setting.
        /// </summary>
        /// <param name="sender">The event object.</param>
        /// <param name="e">The event args.</param>
        private void _settingsManager_OnNextSetting(object sender, ChangeSettingEventArgs e)
        {
            _screenStats.UnselectAll();
            _screenStats.SelectedStat(e.CurrentSettingGroupName);
        }


        /// <summary>
        /// Invoked when the setting has been moved to the previous setting.
        /// </summary>
        /// <param name="sender">The event object.</param>
        /// <param name="e">The event args.</param>
        private void _settingsManager_OnPreviousSetting(object sender, ChangeSettingEventArgs e)
        {
            _screenStats.UnselectAll();
            _screenStats.SelectedStat(e.CurrentSettingGroupName);
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Adds all of the various settings to be manipulated to the settings manager.
        /// </summary>
        private void AddSettings()
        {
            _settingsManager = new SettingsManager(Keys.End, Keys.Home);
            _settingsManager.OnNextSetting += _settingsManager_OnNextSetting;
            _settingsManager.OnPreviousSetting += _settingsManager_OnPreviousSetting;

            var gravitySettings = new[]
            {
                new Setting()
                {
                    Name = "GravityRight",
                    InvokeActionKey = Keys.Right,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _gravity.X += amount;
                    }
                },
                new Setting()
                {
                    Name = "GravityLeft",
                    InvokeActionKey = Keys.Left,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _gravity.X -= amount;
                    }
                },
                new Setting()
                {
                    Name = "GravityDown",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _gravity.Y += amount;
                    }
                },
                new Setting()
                {
                    Name = "GravityUp",
                    InvokeActionKey = Keys.Up,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _gravity.Y -= amount;
                    }
                }
            };

            _settingsManager.AddSettingGroup("Gravity", gravitySettings);

            var bouncinessSettings = new[]
            {
                new Setting()
                {
                    Name = "IncreaseBounciness",
                    InvokeActionKey = Keys.Up,
                    ChangeAmount = 0.01f,
                    ChangeAction = (float amount) =>
                    {
                        _restitutionCoeffecient = (float)Math.Round(_restitutionCoeffecient + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "DecreaseBounciness",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 0.01f,
                    ChangeAction = (float amount) =>
                    {
                        _restitutionCoeffecient = (float)Math.Round(_restitutionCoeffecient - amount, 2);
                    }
                },
            };

            _settingsManager.AddSettingGroup("Bounciness", bouncinessSettings);

            var dragSettings = new[]
            {
                new Setting()
                {
                    Name = "DragIncrease",
                    InvokeActionKey = Keys.Up,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        _dragCoeffecient = (float)Math.Round(_dragCoeffecient + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "DragDecrease",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        _dragCoeffecient = (float)Math.Round(_dragCoeffecient - amount, 2);
                    }
                }
            };

            _settingsManager.AddSettingGroup("Drag", dragSettings);

            var fluidDensitySettings = new[]
{
                new Setting()
                {
                    Name = "FluidDensityIncrease",
                    InvokeActionKey = Keys.Up,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        _fluidDensity = (float)Math.Round(_fluidDensity + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "FluidDensityDragDecrease",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        _fluidDensity = (float)Math.Round(_fluidDensity - amount, 2);
                    }
                }
            };

            _settingsManager.AddSettingGroup("FluidDensity", fluidDensitySettings);
        }


        /// <summary>
        /// Creates all of the screen stats to be shown on the screen.
        /// </summary>
        private void CreateScreenStats()
        {
            _screenStats = new ScreenStats(Content);

            _screenStats.AddStatText(new StatText()
            {
                Name = "Gravity",
                Text = "X: 0, Y: 0",
                Forecolor = Color.Black,
                SelectedColor = Color.Yellow,
                Selected = true,
                Position = new Vector2(0, 0)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "Bounciness",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(0, 25)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "Drag",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(0, 50)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "FluidDensity",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(0, 75)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "Velocity",
                Text = "X: 0, Y: 0",
                Forecolor = Color.Black,
                Position = new Vector2(0, _screenHeight - 25)
            });
        }


        /// <summary>
        /// Updates the physics using the given <paramref name="frameTime"/>.
        /// </summary>
        /// <param name="frameTime">The time in seconds since the last frame.</param>
        private void UpdatePhysics(float frameTime)
        {
            var allForces = new Vector2();//Total forces.  Gravity + air/fluid drag + etc....

            //Add the weight force, which only affects the y-direction (because that's the direction gravity is pulling from)
            //https://www.wikihow.com/Calculate-Force-of-Gravity
            allForces += _mass * _gravity;

            /*Add the air resistance force. This would affect both X and Y directions, but we're only looking at the y-axis in this example.
                Things to note:
                1. Multiplying 0.5 is the same as dividing by 2.  The original well known equation in the link below divides by 2 instead of \
                   multiplying by 0.5.
                2. Mutiplying the -1 constant is to represent the opposite direction that the wind is traveling compared to the direction 
                   the object is traveling
                3. Multiplying _velocityY * _velocityY is the same thing as _velocity^2 which is in the well known equation in the link below
            */
            http://www.softschools.com/formulas/physics/air_resistance_formula/85/
            allForces += Util.CalculateDragForceOnObject(_fluidDensity, _dragCoeffecient, _surfaceArea, _velocity);

            //Clamp the total forces
            allForces = Util.Clamp(allForces, -10f, 10f);

            /* Verlet integration for the y-direction
             * This is the amount the ball will be moving in this frame based on the ball's current velocity and acceleration. 
             * Part 1: https://www.youtube.com/watch?v=3HjO_RGIjCU
             * Part 2: https://www.youtube.com/watch?v=pBMivz4rIJY
             * Refer to C++ code sample and the velocity_verlet() function
             *      https://leios.gitbooks.io/algorithm-archive/content/chapters/physics_solvers/verlet/verlet.html
            */
            var predictedDelta = Util.IntegrateVelocityVerlet(_velocity, frameTime, _acceleration);

            // The following calculation converts the unit of measure from cm per pixel to meters per pixel
            _box.Position += predictedDelta * 100f;

            /*Update the acceleration in the Y direction to take in effect all of the added forces as well as the mass
             Find the new acceleration of the object in the Y direction by solving for A(Accerlation) by dividing all
             0f the net forces by the mass of the object.  This is one way to find out the acceleration.
             */
            var newAcceleration = allForces / _mass;

            var averageAcceleration = Util.Average(new[] { newAcceleration, _acceleration });

            _velocity += averageAcceleration * frameTime;

            _velocity = Util.Clamp(_velocity, -2f, 2f);
        }


        /// <summary>
        /// Checks collision with the edges of the screen.
        /// </summary>
        private void CheckCollision()
        {
            //Let's do very simple collision detection for the left of the screen
            if (_box.Position.X < 0 && _velocity.X < 0)
            {
                // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                _velocity.X *= _restitutionCoeffecient;

                // Move the ball back a little bit so it's not still "stuck" in the wall
                //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                _box.SetPosition(0, _box.Position.Y);
            }

            //Let's do very simple collision detection for the right of the screen
            if (_box.Position.X + (_radius * 2) > _screenWidth && _velocity.X > 0)
            {
                // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                _velocity.X *= _restitutionCoeffecient;

                // Move the ball back a little bit so it's not still "stuck" in the wall
                //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                _box.SetPosition(_screenWidth - (_radius * 2), _box.Position.Y);
            }

            //Let's do very simple collision detection for the top of the screen
            if (_box.Position.Y < 0 && _velocity.Y < 0)
            {
                // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                _velocity.Y *= _restitutionCoeffecient;

                // Move the ball back a little bit so it's not still "stuck" in the wall
                //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                _box.SetPosition(0, _box.Position.Y);
            }

            //Let's do very simple collision detection for the bottom of the screen
            if (_box.Position.Y + (_radius * 2) > _screenHeight && _velocity.Y > 0)
            {
                // This is a simplification of impulse-momentum collision response. e should be a negative number, which will change the velocity's direction
                _velocity.Y *= _restitutionCoeffecient;

                // Move the ball back a little bit so it's not still "stuck" in the wall
                //This is just for this demo.  This simulates a collision response to separate the ball from the wall.
                _box.SetPosition(_screenHeight - (_radius * 2), _box.Position.Y);
            }
        }
        #endregion
    }
}