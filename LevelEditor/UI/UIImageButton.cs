using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor.UI
{
    class UIImageButton : UIImage
    {
        private Color color;
        public UIImageButton(Texture2D tex, int width, int height) : base(tex, width, height)
        {
        }
        protected override void MouseEnter(MouseState args, UIElement elm)
        {
            color = Color.Lerp(Color.White, Color.White, 0.6f);
            base.MouseEnter(args, elm);
        }
        protected override void MouseLeave(MouseState args, UIElement elm)
        {
            color = Color.White;
            base.MouseLeave(args, elm);
        }
        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Dimensions, color);
            base.Draw(spriteBatch);
        }
    }
}
