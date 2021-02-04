using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UI
{
    class UIPanel : UIElement
    {
        public Color BackgroundColor;
        public UIPanel(int width, int height, Color backgroundcolor)
        {
            Width.Pixels = width;
            Height.Pixels = height;
            BackgroundColor = backgroundcolor;
        }
        public UIPanel(StyleDimension width, StyleDimension height, Color backgroundcolor)
        {
            Width = width;
            Height = height;
            BackgroundColor = backgroundcolor;
        }
        protected override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();
            spriteBatch.Draw(Main.solid, Dimensions, BackgroundColor);
            base.Draw(spriteBatch);
        }
    }
}
