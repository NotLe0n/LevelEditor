using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UI
{
    class UIText : UIElement
    {
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public UIText(string text, Color textColor)
        {
            Text = text;
            TextColor = textColor;
        }
        public UIText(int text, Color textColor)
        {
            Text = text.ToString();
            TextColor = textColor;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            bounds = new Rectangle(absolutePos.ToPoint(), Main.font.MeasureString(Text).ToPoint());
            spriteBatch.DrawString(Main.font, Text, absolutePos, TextColor);
            base.Draw(spriteBatch);
        }
    }
}
