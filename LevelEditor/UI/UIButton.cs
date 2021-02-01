using Microsoft.Xna.Framework;

namespace LevelEditor.UI
{
    class UIButton : UIPanel
    {
        public UIText Text;
        public UIButton(UIText text, int width, int height, Color backgroundColor) : base(width, height, backgroundColor)
        {
            Text = text;
            Append(text);
        }
        public override void Update(GameTime gameTime)
        {
            Text.absolutePos = new Vector2(absolutePos.X + Width / 6, absolutePos.Y + Height / 4);
            bounds = new Rectangle((int)absolutePos.X, (int)absolutePos.Y, Width, Height);
            base.Update(gameTime);
        }
    }
}
