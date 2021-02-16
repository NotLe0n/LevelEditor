using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelEditor
{
    public class GameObject
    {
        public delegate void MouseEvent(MouseState evt, GameObject elm);
        public event MouseEvent OnClick;
        public event MouseEvent OnClickAway;

        public Rectangle Bounds;

        public virtual void Update()
        {
            if (Main.tool == 0 && Main.LeftClick && !Main.MouseOverUI)
            {
                if (Bounds.Contains(Main.MousePos))
                {
                    OnClick?.Invoke(Main.mouse, this);
                }
                else
                {
                    OnClickAway?.Invoke(Main.mouse, this);
                }
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void Remove()
        {
        }
    }
}
