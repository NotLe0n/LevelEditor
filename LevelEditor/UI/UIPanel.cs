using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UI
{
    class UIPanel : UIElement
    {
        public int Width;
        public int Height;
        public Color backgroundColor;
        public UIPanel(int width, int height, Color backgroundcolor)
        {
            Width = width;
            Height = height;
            backgroundColor = backgroundcolor;
        }
        public override void Update(GameTime gameTime)
        {
            bounds = new Rectangle((int)absolutePos.X, (int)absolutePos.Y, Width, Height);
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.solid, bounds, backgroundColor);
            base.Draw(spriteBatch);
        }
    }
}
