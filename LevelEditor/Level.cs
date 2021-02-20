using LevelEditor.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace LevelEditor
{
    public class Level
    {
        public Tile[,] tiles;
        public int width;
        public int height;
        public Spawnpoint spawnPoint;
        public List<EventTrigger> EventTriggers;
        public List<Enemy> Enemies;
        public Rectangle bounds;

        private string[] fileLine;
        private uint counter;

        public Level(string file)
        {
            EventTriggers = new List<EventTrigger>();
            Enemies = new List<Enemy>();
            counter = 0;
            Initialize(file);
        }

        public void Initialize(string file)
        {
            fileLine = File.ReadAllLines(file);

            // Parse Spawnpoint
            string[] spLine = fileLine[counter].Split(' ');
            spawnPoint = new Spawnpoint(new Vector2(int.Parse(spLine[0]), int.Parse(spLine[1])));

            InitializeEvents();
            InitializeEnemies();
            InitializeTileMap();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].Draw(spriteBatch);
                }
            }

            // Draw Event Triggers
            EventTriggers.ForEach(x => x.Draw(spriteBatch));

            // Draw Enemies
            Enemies.ForEach(x => x.Draw(spriteBatch));

            // Draw Spawnpoint
            spawnPoint.Draw(spriteBatch);
        }

        public void Update()
        {
            // Update tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].Update();
                }
            }

            // Update Event Triggers
            EventTriggers.ForEach(x => x.Update());

            Enemies.ForEach(x => x.Update());

            spawnPoint.Update();
        }

        #region init
        private void InitializeEvents()
        {
            // goes through the file and stops once it hits "enemies:"
            for (counter = 2; fileLine[counter] != "enemies:"; counter++)
            {
                string[] evtLine = fileLine[counter].Split(' ');

                List<string> evtParams = new List<string>();
                for (int j = 5; j < evtLine.Length; j++)
                {
                    evtParams.Add(evtLine[j]);
                }

                //Add the Event
                EventTriggers.Add(new EventTrigger(
                    int.Parse(evtLine[0]),    // ID
                    new Rectangle(int.Parse(evtLine[1]), int.Parse(evtLine[2]), int.Parse(evtLine[3]), int.Parse(evtLine[4])),  // Bounds (x, y, w, h)
                    evtParams.ToArray()) // Parameters
                    );
            }
        }

        private void InitializeEnemies()
        {
            // increments the counter once, goes through the file and stops once it hits "map:" 
            for (++counter; fileLine[counter] != "map:"; counter++)
            {
                string[] enemyLine = fileLine[counter].Split(' ');

                List<string> enmParams = new List<string>();
                for (int j = 3; j < enemyLine.Length; j++)
                {
                    enmParams.Add(enemyLine[j]);
                }

                Enemies.Add(new Enemy(int.Parse(enemyLine[0]), new Vector2(int.Parse(enemyLine[1]), int.Parse(enemyLine[2])), enmParams.ToArray()));
            }
        }
        private void InitializeTileMap()
        {
            string[] mapLines = new string[fileLine.Length];
            //initialize the tilemap
            for (int i = fileLine.Length - 1; i > -1; i--)
            {
                if (fileLine[i] == "map:")
                {
                    mapLines = new string[fileLine.Length - i];
                    Array.Copy(fileLine, i, mapLines, 0, fileLine.Length - i);
                }
            }
            //Load Textures from File (texMap handles this)
            Main.textureMap = new TextureMap(Path.GetDirectoryName(Main.selectedFile) + @"\" + mapLines[1]);

            // set Tilemap dimentions
            height = mapLines.Length - 2;
            width = mapLines[2].Length;
            bounds = new Rectangle(0, 0, width * (int)Tile.TileSize.X, height * (int)Tile.TileSize.Y);

            // initialize Tile array
            tiles = new Tile[width, height];

            //Read Map from File and construct tiles
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 worldPos = new Vector2(x * Tile.TileSize.X, y * Tile.TileSize.Y);
                    int tileID = (int)char.GetNumericValue(mapLines[mapLines.Length - height + y][x]);

                    tiles[x, y] = new Tile(worldPos, tileID);
                }
            }
            UpdateTextures();
        }
        public void UpdateTextures()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int tileID = tiles[x, y].TileID;

                    bool topAir = y == 0 || tiles[x, y - 1].TileID == 0;
                    bool leftAir = x == 0 || tiles[x - 1, y].TileID == 0;
                    bool rightAir = x == width - 1 || tiles[x + 1, y].TileID == 0;
                    bool bottomAir = y == height - 1 || tiles[x, y + 1].TileID == 0;

                    if (topAir && bottomAir)
                    {
                        if (leftAir)
                        {
                            if (rightAir)
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][15];
                            }
                            else
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][12];
                            }
                        }
                        else if (rightAir)
                        {
                            tiles[x, y].texture = Main.textureMap.textures[tileID][14];
                        }
                        else
                        {
                            tiles[x, y].texture = Main.textureMap.textures[tileID][13];
                        }
                    }
                    else
                    {
                        if (topAir)
                        {
                            if (leftAir)
                            {
                                if (rightAir)
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][3];
                                }
                                else
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][0];
                                }
                            }
                            else if (rightAir)
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][2];
                            }
                            else
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][1];
                            }
                        }
                        else if (bottomAir)
                        {
                            if (leftAir)
                            {
                                if (rightAir)
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][11];
                                }
                                else
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][8];
                                }
                            }
                            else if (rightAir)
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][10];
                            }
                            else
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][9];
                            }
                        }
                        else
                        {
                            if (leftAir)
                            {
                                if (rightAir)
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][7];
                                }
                                else
                                {
                                    tiles[x, y].texture = Main.textureMap.textures[tileID][4];
                                }
                            }
                            else if (rightAir)
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][6];
                            }
                            else
                            {
                                tiles[x, y].texture = Main.textureMap.textures[tileID][5];
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
