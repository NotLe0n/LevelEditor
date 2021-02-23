using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.GameObjects
{
    public class Enemy : GameObject
    {
        public int ID;
        public Vector2 Position;
        public string[] Parameters;
        public Enemy(int id, Vector2 pos, params string[] parameters)
        {
            ID = id;
            Position = pos;
            Parameters = parameters;
            Bounds = new Rectangle(pos.ToPoint(), new Point(50, 50));
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
            spriteBatch.Draw(Main.solid, Bounds, Color.Yellow * 0.5f);
        }
        public override void Remove()
        {
            Main.level.Enemies.Remove(this);
            base.Remove();
        }
        public override string ToString()
        {
            return $"{ID} {Bounds.X} {Bounds.Y} {string.Join(' ', Parameters)}";
        }
    }
}
