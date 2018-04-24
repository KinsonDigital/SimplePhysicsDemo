using Microsoft.Xna.Framework.Input;
using System;

namespace GravityTesting
{
    /// <summary>
    /// Represents a single setting that can be changed.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// The name of the setting to change.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The keyboard key used to invoke the <see cref="ChangeAction"/>.
        /// </summary>
        public Keys InvokeActionKey { get; set; }

        /// <summary>
        /// The action to be executed to apply the change.
        /// </summary>
        public Action<float> ChangeAction { get; set; }

        /// <summary>
        /// The amount to change the setting value.
        /// </summary>
        public float ChangeAmount { get; set; }
    }
}
