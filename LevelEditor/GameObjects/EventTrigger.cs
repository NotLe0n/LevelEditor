using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.GameObjects
{
    public class EventTrigger : GameObject
    {
        public int ID;
        public string[] Parameters;
        public EventTrigger(int id, Rectangle rect, params string[] parameters)
        {
            ID = id;
            Bounds = rect;
            Parameters = parameters;
        }
        public override void Update()
        {
            if (Bounds.Contains(Main.MousePos))
            {
                Main.MouseText = $"ID: {ID}\nBounds: {Bounds}\nParameters: {string.Join(", ", Parameters)}";
            }
            if (Main.tool == 0 && Bounds.Contains(Main.MousePos) && Main.LeftHeld && Main.mouseMoved)
            {
                Bounds.Location = Main.MousePos.ToPoint() - (Bounds.Size.ToVector2() / 2).ToPoint();
            }
            base.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Main.solid, Bounds, Color.Blue * 0.5f);
        }
        public override string ToString()
        {
            return $"{ID} {Bounds.X} {Bounds.Y} {Bounds.Width} {Bounds.Height} {string.Join(' ', Parameters)}";
        }
        public override void Remove()
        {
            Main.level.EventTriggers.Remove(this);
            base.Remove();
        }
    }
}
