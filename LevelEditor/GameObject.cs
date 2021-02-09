using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LevelEditor
{
    public class GameObject
    {
        public delegate void MouseEvent(MouseState evt, GameObject elm);
        public event MouseEvent OnClick;
        public event MouseEvent OnClickAway;

        public static List<GameObject> objects = new List<GameObject>();
        public Rectangle Bounds;
        public GameObject()
        {
            objects.Add(this);
        }
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
        public virtual void Draw()
        {

        }
    }
}
