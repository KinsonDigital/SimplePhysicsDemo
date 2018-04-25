using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SimplePhysicsDemo
{
    /// <summary>
    /// Manages settings to be changed.
    /// </summary>
    public class SettingsManager
    {
        public event EventHandler<ChangeSettingEventArgs> OnNextSetting;
        public event EventHandler<ChangeSettingEventArgs> OnPreviousSetting;

        private Dictionary<string, Setting[]> _settingGroups = new Dictionary<string, Setting[]>();
        private List<Setting> _settings = new List<Setting>();
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private int _currentSettingKeyIndex;
        private Keys _nextSettingKey;
        private Keys _previousSettingKey;


        #region Constructor
        /// <summary>
        /// Creates a new instance of <see cref="SettingsManager"/>.
        /// </summary>
        /// <param name="nextSettingKeys">The keys to be pressed to move to the next setting.</param>
        /// <param name="previousSettingKeys">The keys to be pressed to move to the previous setting.</param>
        public SettingsManager(Keys nextSettingKey, Keys previousSettingKey)
        {
            _nextSettingKey = nextSettingKey;
            _previousSettingKey = previousSettingKey;
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the current setting.
        /// </summary>
        public string CurrentSetting => _settings[_currentSettingKeyIndex].Name;
        #endregion


        #region Public Methods
        /// <summary>
        /// Creates a setting group to the <see cref="SettingsManager"/> with the given <paramref name="groupName"/> and using the given <paramref name="settings"/>.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="settings">The settings in the group.</param>
        public void AddSettingGroup(string groupName, Setting[] settings)
        {
            _settingGroups.Add(groupName, settings);
        }


        /// <summary>
        /// Adds the given <paramref name="setting"/> to the <see cref="SettingsManager"/>.
        /// </summary>
        /// <param name="setting">The setting to add.</param>
        public void AddSetting(string groupName, Setting setting)
        {
            var foundSettings = new List<Setting>();

            //Find the setting group
            if (_settingGroups.Keys.Contains(groupName))
            {
                foundSettings = _settingGroups[groupName].ToList();
                foundSettings.Add(setting);

                _settingGroups[groupName] = foundSettings.ToArray();
            }
        }


        /// <summary>
        /// Updates any internal logic.
        /// </summary>
        /// <param name="gameTime">The game time elapsed since last frame.</param>
        public void Update(GameTime gameTime)
        {
            _currentKeyboardState = Keyboard.GetState();

            if(_currentKeyboardState.IsKeyDown(_nextSettingKey) && _previousKeyboardState.IsKeyUp(_nextSettingKey))
                NextSetting();

            if (_currentKeyboardState.IsKeyDown(_previousSettingKey) && _previousKeyboardState.IsKeyUp(_previousSettingKey))
                PreviousSetting();

            ////Check if the next setting keys are all pressed
            //for (int i = 0; i < _nextSettingKeys.Length; i++)
            //{
            //    //If the current change setting key is not pressed, all the keys are not pressed,
            //    //then exit the loop.
            //    if (_currentKeyboardState.IsKeyUp(_nextSettingKeys[i]) && _previousKeyboardState.IsKeyDown(_nextSettingKeys[i]))
            //    {

            //        break;
            //    }
            //}

            ////Check if the next setting keys are all pressed
            //for (int i = 0; i < _previousSettingKeys.Length; i++)
            //{
            //    //If the current change setting key is not pressed, all the keys are not pressed,
            //    //then exit the loop.
            //    if (_currentKeyboardState.IsKeyUp(_previousSettingKeys[i]))
            //    {
            //        allPrevKeysPressed = false;
            //        break;
            //    }
            //}

            ProcessCurrentSettingGroup();

            _previousKeyboardState = _currentKeyboardState;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Processes the keys for the current setting group.
        /// </summary>
        private void ProcessCurrentSettingGroup()
        {
            foreach (var group in _settingGroups)
            {
                if (group.Key == _settingGroups.Keys.ToArray()[_currentSettingKeyIndex])
                {
                    foreach (var setting in group.Value)
                    {
                        //If the current settings change action key has been pressed
                        if (_currentKeyboardState.IsKeyDown(setting.InvokeActionKey) && _previousKeyboardState.IsKeyUp(setting.InvokeActionKey))
                            setting?.ChangeAction.Invoke(setting.ChangeAmount);
                    }

                    break;
                }
            }
        }


        /// <summary>
        /// Moves to the next setting.
        /// </summary>
        private void NextSetting()
        {
            var oldSettingKeyIndex = _currentSettingKeyIndex;

            //Set the curent setting index
            _currentSettingKeyIndex = _currentSettingKeyIndex < _settingGroups.Keys.Count - 1 ? _currentSettingKeyIndex + 1 : 0;

            var keys = _settingGroups.Keys.ToArray();

            OnNextSetting?.Invoke(this, new ChangeSettingEventArgs(keys[oldSettingKeyIndex], keys[_currentSettingKeyIndex]));
        }


        /// <summary>
        /// Moves to the previous setting
        /// </summary>
        private void PreviousSetting()
        {
            var oldSettingKeyIndex = _currentSettingKeyIndex;

            //Set the curent setting index
            _currentSettingKeyIndex = _currentSettingKeyIndex <= 0 ? _settingGroups.Keys.Count - 1 : _currentSettingKeyIndex - 1;

            var keys = _settingGroups.Keys.ToArray();

            OnPreviousSetting?.Invoke(this, new ChangeSettingEventArgs(keys[oldSettingKeyIndex], keys[_currentSettingKeyIndex]));
        }
        #endregion
    }
}