using System;

namespace GravityTesting
{
    /// <summary>
    /// Represents the data of a change setting event.
    /// </summary>
    public class ChangeSettingEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChangeSettingEventArgs"/>.
        /// </summary>
        /// <param name="previousSettingGroupName">The name of the previous setting group being changed from.</param>
        /// <param name="currentSettingGroupName">The name of the current setting group name being changed to.</param>
        public ChangeSettingEventArgs(string previousSettingGroupName, string currentSettingGroupName)
        {
            PreviousSettingGroupName = previousSettingGroupName;
            CurrentSettingGroupName = currentSettingGroupName;
        }


        /// <summary>
        /// The previous setting group name.
        /// </summary>
        public string PreviousSettingGroupName { get; private set; }

        /// <summary>
        /// The next setting group name.
        /// </summary>
        public string CurrentSettingGroupName { get; private set; }
    }
}
