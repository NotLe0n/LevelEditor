using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.UI
{
    class UIImage : UIElement
    {
        public Texture2D Texture;
        public UIImage(Texture2D tex, int width, int height)
        {
            Texture = tex;
            Width.Pixels = width;
            Height.Pixels = height;
        }
        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Dimensions, Color.White);
            base.Draw(spriteBatch);
        }
    }
}
