using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LevelEditor.UI
{
    public class UIElement
    {
        public delegate void MouseEvent(UIElement elm);
        public static readonly List<UIElement> Elements = new List<UIElement>();
        public List<UIElement> Children = new List<UIElement>();
        public UIElement Parent { get; private set; }
        public Vector2 absolutePos;
        public Rectangle bounds;
        public bool isMouseOver => bounds.Contains(Main.mouse.Position);
        public UIElement()
        {
            Elements.Add(this);
        }

        public event MouseEvent Click;
        public event MouseEvent RightClick;
        public event MouseEvent Draging;
        public event MouseEvent Hovering;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            DrawChildren(spriteBatch);
        }

        public virtual void DrawChildren(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Draw(spriteBatch);
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            if (isMouseOver)
            {
                OnHover();
                if (Main.LeftClick)
                {
                    OnClick();
                }
                if (Main.RightClick)
                {
                    OnRightClick();
                }
                if (Main.LeftHeld && Main.mouseMoved)
                {
                    OnDrag();
                }
            }
        }
        public virtual void OnClick()
        {
            Click?.Invoke(this);
        }
        public virtual void OnRightClick()
        {
            RightClick?.Invoke(this);
        }
        public virtual void OnHover()
        {
            Hovering?.Invoke(this);
        }
        public virtual void OnDrag()
        {
            Draging?.Invoke(this);
        }
        public void Append(UIElement element)
        {
            element.Parent = this;
            Children.Add(element);
        }
        public void Remove()
        {
            Elements.Remove(this);
            Parent = null;
        }
    }
}
