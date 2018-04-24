using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GravityTesting
{
    /// <summary>
    /// Used to render stats to a screen.
    /// </summary>
    public class ScreenStats
    {
        private SpriteFont _titleFont;
        private SpriteFont _textFont;
        private List<StatText> _statItems = new List<StatText>();

        /// <summary>
        /// Create a new instance of <see cref="ScreenStats"/>.
        /// </summary>
        /// <param name="contentManager">The content manager to use to load the fonts.</param>
        public ScreenStats(ContentManager contentManager)
        {
            _titleFont = contentManager.Load<SpriteFont>(@"Font\Title");
            _textFont = contentManager.Load<SpriteFont>(@"Font\Text");
        }


        /// <summary>
        /// Update the stat with the given <paramref name="name"/> to the given <paramref name="text"/>.
        /// </summary>
        /// <param name="name">The name of the stat to update.</param>
        /// <param name="text">The content to update the stat to.</param>
        public void UpdateStat(string name, string text)
        {
            //Find the stat
            for (int i = 0; i < _statItems.Count; i++)
            {
                if (_statItems[i].Name.Replace(": ", "") == name)
                {
                    _statItems[i].Text = text;
                    break;
                }
            }
        }


        /// <summary>
        /// Render the screen stats to the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use to render.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Renter all of the stat items
            for (int i = 0; i < _statItems.Count; i++)
            {
                var textWidth = _titleFont.MeasureString(_statItems[i].Name + _statItems[i].Text).X;
                var textHeight = _titleFont.MeasureString(_statItems[i].Text).Y;

                //If the stat is selected, render the background color
                if(_statItems[i].Selected)
                    spriteBatch.FillRectangle(new Rectangle((int)_statItems[i].Position.X, (int)_statItems[i].Position.Y, (int)textWidth, (int)textHeight), _statItems[i].SelectedColor);

                spriteBatch.DrawString(_titleFont, _statItems[i].Name, _statItems[i].Position, _statItems[i].Forecolor);
                spriteBatch.DrawString(_textFont, _statItems[i].Text, new Vector2(_statItems[i].Position.X + _titleFont.MeasureString(_statItems[i].Name).X, _statItems[i].Position.Y), _statItems[i].Forecolor);
            }
        }


        /// <summary>
        /// Adds the given <paramref name="text"/> to the screen for rendering.
        /// </summary>
        /// <param name="text">The text to render.</param>
        public void AddStatText(StatText text)
        {
            text.Name += ": ";
            _statItems.Add(text);
        }


        /// <summary>
        /// Changes the <see cref="Color"/> of the stat that matches the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the stat to change.</param>
        /// <param name="color">The color to set the stat to.</param>
        public void ChangeStatColor(string name, Color color)
        {
            var statText = GetStat(name);

            statText.Forecolor = color;
        }


        /// <summary>
        /// Selects the stat that matches the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the stat to select.</param>
        public void SelectedStat(string name)
        {
            var foundStat = GetStat(name);

            foundStat.Selected = true;
        }


        /// <summary>
        /// Unselects all of the settings.
        /// </summary>
        public void UnselectAll()
        {
            for (int i = 0; i < _statItems.Count; i++)
            {
                _statItems[i].Selected = false;
            }
        }


        #region Private Methods
        /// <summary>
        /// Gets the index of a stat that matches the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the stat to find.</param>
        /// <returns></returns>
        private StatText GetStat(string name)
        {
            for (int i = 0; i < _statItems.Count; i++)
            {
                if (_statItems[i].Name.Replace(": ", "") == name)
                    return _statItems[i];
            }

            return new StatText();
        }
        #endregion
    }
}
