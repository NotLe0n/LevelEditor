using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LevelEditor
{
    public class Spawnpoint : GameObject
    {
        public Rectangle Bounds => new Rectangle(Position.ToPoint(), new Point(50, 50));
        public Vector2 Position { get; set; }
        public Spawnpoint(Vector2 pos)
        {
            Position = pos;
        }
        public override void Update()
        {
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
