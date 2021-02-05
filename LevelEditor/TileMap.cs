using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

            // Read Spawn Point
            string[] spLine = lines[0].Split(' '); //The line where the spawnPoint is written
            Main.spawnPoint = new Spawnpoint(new Vector2(int.Parse(spLine[0]), int.Parse(spLine[1])));

            #region Events
            for (int i = 2; i < lines.Length && lines[i] != "enemies:"; i++)
            {
                string[] evtLines = lines[i].Split(' ');

                // Read Event Dimensions
                var evtRect = new Rectangle(int.Parse(evtLines[1]), int.Parse(evtLines[2]), int.Parse(evtLines[3]), int.Parse(evtLines[4]));

                // Read Event Params
                List<string> evtParams = new List<string>();
                for(int j = 5; j < evtLines.Length; j++)
                {
                    evtParams.Add(evtLines[j]);
                }

                // Create event
                var evt = new Event(int.Parse(evtLines[1]), evtRect, evtParams.ToArray());
                Main.events.Add(evt);
            }
            #endregion

            #region tiles
            string[] mapLines = new string[lines.Length];
            for (int i = lines.Length - 1; i > -1; i--)
            {
                if (lines[i] == "map:")
                {
                    mapLines = new string[lines.Length - i];
                    Array.Copy(lines, i, mapLines, 0, lines.Length - i);
                }
            }
            
            //Load Textures from File (texMap handles this)
            Main.textureMap = new TextureMap(Path.GetDirectoryName(filePath) + @"\" + mapLines[1]);

            // set Tilemap dimentions
            height = mapLines.Length - 2;
            width = mapLines[2].Length;

            // initialize Tile array
            tiles = new Tile[width, height];

            //Read Map from File and construct tiles
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x]), Main.solid);
                    if ((int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x]) != 0) //with texture (in textures)
                    {
                        tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x]), Main.textureMap.textures[(int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x])]);
                    }
                    else //without texture (0)
                    {
                        tiles[x, y] = new Tile(new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y), (int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x]));
                    }
                }
            }
            #endregion
        }
    }
}
