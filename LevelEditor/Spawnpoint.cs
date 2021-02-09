using Microsoft.Xna.Framework;

namespace LevelEditor
{
    public class Spawnpoint : GameObject
    {
        public Vector2 Position;
        public Spawnpoint(Vector2 pos)
        {
            Position = pos;
        }
        public override void Update()
        {
            Bounds = new Rectangle(Position.ToPoint(), new Point(50, 50));

            if (Bounds.Contains(Main.MousePos))
            {
                Main.MouseText = $"Position {Position}";
            }
            if (Main.tool == 0 && Bounds.Contains(Main.MousePos) && Main.LeftHeld && Main.mouseMoved)
            {
                Position = Main.MousePos - Bounds.Size.ToVector2() / 2;
            }
            base.Update();
        }
        public override void Draw()
        {
            Main.spriteBatch.Draw(Main.solid, Bounds, Color.Red * 0.5f);
            base.Draw();
        }
        public override string ToString()
        {
            return $"{(int)Position.X} {(int)Position.Y}";
        }
    }
}
