using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor
{
    public class Tile
    {
        public Rectangle rect;
        public int TileID;
        public Texture2D texture;
        public Vector2 Position;
        public static readonly Vector2 TileSize = new Vector2(50, 50); //for less magic numbers
        public Tile(Vector2 pos, int tileID, Texture2D tex = null)
        {
            rect = new Rectangle((int)pos.X, (int)pos.Y, (int)TileSize.X, (int)TileSize.Y);
            TileID = tileID;
            texture = tex;
            Position = pos;
        }
        public void Update()
        {
            rect = new Rectangle(Position.ToPoint(), rect.Size);
            if (!Main.MouseOverUI && Main.tool == 1)
            {
                if (rect.Contains(Main.MousePos) && Main.RightHeld)
                {
                    TileID = 0;
                }
                if (rect.Contains(Main.MousePos) && Main.LeftHeld)
                {
                    TileID = Main.selectedMaterial;
                    texture = Main.textureMap.textures[Main.selectedMaterial];
                }
            }
            // Hover text
            if (TileID != 0 && rect.Contains(Main.MousePos))
            {
                Main.MouseText = $"ID: {TileID}\n Dimensions: {rect}";
            }
        }
        public void Draw()
        {
            // Draw tooltip
            if (rect.Contains(Main.MousePos) && TileID != Main.selectedMaterial && Main.tool == 1)
            {
                Main.spriteBatch.Draw(Main.textureMap.textures[Main.selectedMaterial], rect, Color.White * 0.7f);
            }
            // Draw Tile
            if (TileID != 0)
            {
                if (Main.tool == 1 && rect.Contains(Main.MousePos) && TileID != Main.selectedMaterial)
                    return;

                Main.spriteBatch.Draw(texture, rect, Color.White);
            }
        }
    }
}
