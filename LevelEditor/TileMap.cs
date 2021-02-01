using Microsoft.Xna.Framework;
using System.IO;

namespace LevelEditor
{
    public class TileMap
    {
        public Tile[,] tiles;
        public int width;
        public int height;
        public TileMap(string filePath)
        {
            //Get the path of specified file
            string[] lines = File.ReadAllLines(filePath);
            //Load Textures from File (texMap handles this)
            Main.textureMap = new TextureMap(Path.GetDirectoryName(filePath) + @"\" + lines[0]);

            height = lines.Length - 2;
            width = lines[2].Length;
            tiles = new Tile[width, height];
            //Read Map from File and construct tiles
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(lines[lines.Length - height + y][x]), Main.solid);
                    if ((int)char.GetNumericValue(lines[lines.Length - height + y][x]) != 0) //with texture (in textures)
                    {
                        tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(lines[lines.Length - height + y][x]), Main.textureMap.textures[(int)char.GetNumericValue(lines[lines.Length - height + y][x])]);
                    }
                    else //without texture (0)
                    {
                        tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(lines[lines.Length - height + y][x]));
                    }
                }
            }
        }
    }
}
