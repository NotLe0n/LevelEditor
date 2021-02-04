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
            if (!Main.mouseOverUI)
            {
                if (rect.Contains(Main.mouse.Position.ToVector2() / Main.zoom) && Main.RightHeld)
                {
                    TileID = 0;
                }
                if (rect.Contains(Main.mouse.Position.ToVector2() / Main.zoom) && Main.LeftHeld)
                {
                    TileID = Main.selectedMaterial;
                    texture = Main.textureMap.textures[Main.selectedMaterial];
                }
            }
        }
        public void Draw()
        {
            if (rect.Contains(Main.mouse.Position.ToVector2() / Main.zoom) && TileID != Main.selectedMaterial)
            {
                Main.spriteBatch.Draw(Main.textureMap.textures[Main.selectedMaterial], rect, Color.White * 0.7f); // Color.White * 0.5f halbiert die transparenz
            }
            else if (TileID != 0)
            {
                Main.spriteBatch.Draw(texture, rect, Color.White);
            }
        }
    }
}
