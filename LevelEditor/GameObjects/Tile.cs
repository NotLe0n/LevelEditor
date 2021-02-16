using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelEditor.GameObjects
{
    public class Tile : GameObject
    {
        public int TileID;
        public Texture2D texture;
        public Vector2 Position;
        public static readonly Vector2 TileSize = new Vector2(50, 50); //for less magic numbers
        public Tile(Vector2 pos, int tileID, Texture2D tex = null)
        {
            TileID = tileID;
            texture = tex;
            Position = pos;
        }
        public override void Update()
        {
            Bounds = new Rectangle(Position.ToPoint(), TileSize.ToPoint());
            if (!Main.MouseOverUI && Main.tool == 1)
            {
                if (Bounds.Contains(Main.MousePos) && Main.RightHeld)
                {
                    TileID = 0;
                }
                if (Bounds.Contains(Main.MousePos) && Main.LeftHeld)
                {
                    TileID = Main.selectedMaterial;
                    texture = Main.textureMap.textures[Main.selectedMaterial];
                }
            }
            // Hover text
            if (TileID != 0 && Bounds.Contains(Main.MousePos))
            {
                Main.MouseText = $"ID: {TileID}\n Dimensions: {Bounds}";
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw tooltip
            if (Bounds.Contains(Main.MousePos) && TileID != Main.selectedMaterial && Main.tool == 1)
            {
                spriteBatch.Draw(Main.textureMap.textures[Main.selectedMaterial], Bounds, Color.White * 0.7f);
            }
            // Draw Tile
            if (TileID != 0)
            {
                if (Main.tool == 1 && Bounds.Contains(Main.MousePos) && TileID != Main.selectedMaterial)
                    return;

                spriteBatch.Draw(texture, Bounds, Color.White);
            }
        }
    }
}
