using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SimplePhysicsDemo
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
        private KeyboardState _currentKeyboardState;
        private KeyboardState _prevKeyboardState;

        private World _world;
        private PhysicsEngine _physics = new PhysicsEngine();

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Main"/>.
        /// </summary>
        public Main()
        {
            //Make this game loop a variable time step
            IsFixedTimeStep = false;
            IsMouseVisible = true;

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
            var orangeVerts = new Vector2[4];
            orangeVerts[0] = new Vector2(-50, -50);
            orangeVerts[1] = new Vector2(50, -50);
            orangeVerts[2] = new Vector2(50, 50);
            orangeVerts[3] = new Vector2(-50, 50);

            var orangeBox = new RectObject(orangeVerts, new Vector2(250, 300))
            {
                Name = "OrangeBox",
                Velocity = Vector2.Zero,
                Radius = 50f,
                Mass = 0.1f,
                Drag = 1f
            };

            orangeBox.SurfaceArea = (float)Math.PI * orangeBox.Radius * orangeBox.Radius / 50000f;

            _world = new World()
            {
                Gravity = Vector2.Zero,
                AirFluidDensity = 10f
            };

            _world.AddGameObject(orangeBox);
            //_world.AddGameObject(purpleBox);

            _physics.SetWorld(_world);

            _graphics.PreferredBackBufferHeight = _graphics.PreferredBackBufferHeight + 200;
            _graphics.ApplyChanges();

            _world.Width = _graphics.PreferredBackBufferWidth;
            _world.Height = _graphics.PreferredBackBufferHeight;

            var screenCenterX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            var screenCenterY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            var halfWidth = _graphics.GraphicsDevice.Viewport.Width / 2;
            var halfHeight = _graphics.GraphicsDevice.Viewport.Height / 2;

            Window.Position = new Point(screenCenterX - halfWidth, screenCenterY - halfHeight);

            //Add the various settings to the settings manager
            AddSettings();

            //Create screen stats
            CreateLinearScreenStats();
            CreateAngularScreenStats();

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
            _currentKeyboardState = Keyboard.GetState();

            //Change the angle of the object
            if (_currentKeyboardState.IsKeyDown(Keys.D) && _prevKeyboardState.IsKeyUp(Keys.D))
            {
                _world.GetGameObject("OrangeBox").AngularForce = .3f;
            }

            if (_currentKeyboardState.IsKeyUp(Keys.D))
            {
                _world.GetGameObject("OrangeBox").AngularForce = 0f;
            }

            _physics.Update(gameTime);

            UpdateLinearStats();
            UpdateAngularStats();

            _settingsManager.Update(gameTime);

            _prevKeyboardState = _currentKeyboardState;

            base.Update(gameTime);
        }


        /// <summary>
        /// Updates the stat values on the screen.
        /// </summary>
        private void UpdateLinearStats()
        {
            var box = _world.GetGameObject("OrangeBox");

            //If the velocity is infinity, just set text to N/A
            var velX = float.IsInfinity(box.Velocity.X) ? "Inf" : Math.Round(box.Velocity.X, 2).ToString();
            var velY = float.IsInfinity(box.Velocity.Y) ? "Inf" : Math.Round(box.Velocity.Y, 2).ToString();

            _screenStats.UpdateStat("Gravity", $"X: {Math.Round(_world.Gravity.X, 2)} , Y:{Math.Round(_world.Gravity.Y, 2)}");
            _screenStats.UpdateStat("Velocity", $"X: {velX} , Y:{velY}");
            _screenStats.UpdateStat("Bounciness", box.Restitution.ToString());
            _screenStats.UpdateStat("Drag", box.Drag.ToString());
            _screenStats.UpdateStat("FluidDensity", _world.AirFluidDensity.ToString());
            _screenStats.UpdateStat("Angle", box.Angle.ToString());
            _screenStats.UpdateStat("Position", $"X: {box.Position.X.ToString()} Y: {box.Position.Y}");
        }


        private void UpdateAngularStats()
        {
            var box = _world.GetGameObject("OrangeBox");

            _screenStats.UpdateStat("AngularVelocity", Math.Round(box.AngularVelocity, 2).ToString());
            _screenStats.UpdateStat("AngularForce", box.AngularForce.ToString());
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            foreach(var obj in _world.GameObjects)
            {
                var color = obj.Name == "OrangeBox" ? Color.DarkOrange : Color.MediumPurple;

                foreach (var side in obj.Sides)
                {
                    _spriteBatch.DrawLine(side.Start, side.Stop, Color.Black);
                }
            }

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
            var box = _world.GetGameObject("Box");

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
                        _world.SetGravity(_world.Gravity.X + amount, _world.Gravity.Y);
                    }
                },
                new Setting()
                {
                    Name = "GravityLeft",
                    InvokeActionKey = Keys.Left,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _world.SetGravity(_world.Gravity.X - amount, _world.Gravity.Y);
                    }
                },
                new Setting()
                {
                    Name = "GravityDown",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _world.SetGravity(_world.Gravity.X, _world.Gravity.Y + amount);
                    }
                },
                new Setting()
                {
                    Name = "GravityUp",
                    InvokeActionKey = Keys.Up,
                    ChangeAmount = 1,
                    ChangeAction = (float amount) =>
                    {
                        _world.SetGravity(_world.Gravity.X, _world.Gravity.Y - amount);
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
                        box.Restitution = (float)Math.Round(box.Restitution + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "DecreaseBounciness",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 0.01f,
                    ChangeAction = (float amount) =>
                    {
                        box.Restitution = (float)Math.Round(box.Restitution - amount, 2);
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
                        box.Drag = (float)Math.Round(box.Drag + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "DragDecrease",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        box.Drag = (float)Math.Round(box.Drag - amount, 2);
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
                        _world.AirFluidDensity = (float)Math.Round(_world.AirFluidDensity + amount, 2);
                    }
                },
                new Setting()
                {
                    Name = "FluidDensityDragDecrease",
                    InvokeActionKey = Keys.Down,
                    ChangeAmount = 1f,
                    ChangeAction = (float amount) =>
                    {
                        _world.AirFluidDensity = (float)Math.Round(_world.AirFluidDensity - amount, 2);
                    }
                }
            };

            _settingsManager.AddSettingGroup("FluidDensity", fluidDensitySettings);
        }


        /// <summary>
        /// Creates all of the screen stats to be shown on the screen.
        /// </summary>
        private void CreateLinearScreenStats()
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
                Name = "Angle",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(0, _world.Height - 75)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "Position",
                Text = "X: 0, Y: 0",
                Forecolor = Color.Black,
                Position = new Vector2(0, _world.Height - 50)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "Velocity",
                Text = "X: 0, Y: 0",
                Forecolor = Color.Black,
                Position = new Vector2(0, _world.Height - 25)
            });
        }

        private void CreateAngularScreenStats()
        {
            _screenStats.AddStatText(new StatText()
            {
                Name = "AngularVelocity",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(_world.Width - 200, _world.Height - 25)
            });

            _screenStats.AddStatText(new StatText()
            {
                Name = "AngularForce",
                Text = "N/A",
                Forecolor = Color.Black,
                Position = new Vector2(_world.Width - 200, _world.Height - 50)
            });
        }
        #endregion
    }
}