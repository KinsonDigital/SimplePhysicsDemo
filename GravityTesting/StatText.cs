using Microsoft.Xna.Framework;

namespace GravityTesting
{
    /// <summary>
    /// Represents a single piece of text rendered to a screen to show a piece of information.
    /// </summary>
    public class StatText
    {
        /// <summary>
        /// Gets or sets the text color of the stat.
        /// </summary>
        public Color Forecolor { get; set; }

        /// <summary>
        /// Gets or sets the selected color of the stat.
        /// </summary>
        public Color SelectedColor { get; set; } = new Color(255, 255, 0, 255);

        /// <summary>
        /// Gets or sets the name of the stat.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the information of the stat to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the position of the stat.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the stat text will render as selected.
        /// </summary>
        public bool Selected { get; set; }
    }
}