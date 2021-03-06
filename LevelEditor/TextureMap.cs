﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LevelEditor
{
    //The whole class is essentially a wrapper
    //for a <int, Texture2D> Dictionary and handles
    //the texture loading from a .texmap file
    public class TextureMap
    {
        //The Dictianory holding the Textures
        public Dictionary<int, Texture2D[]> textures;
        public string filePath;
        public Texture2D[] maps;
        public TextureMap(string file)
        {
            filePath = file;
            textures = new Dictionary<int, Texture2D[]>();
            //Load File
            string[] lines = File.ReadAllLines(file);

            var temp = new List<Texture2D>();
            //Load Textures from File
            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(' ');

                temp.Add(Helper.LoadTexture(line[1]));

                List<Texture2D> sliced = new List<Texture2D>();
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        Rectangle srcRect = new Rectangle(x * 16, y * 16, 16, 16);
                        sliced.Add(Helper.LoadTexturePart(line[1], srcRect));
                    }
                }
                textures.Add(int.Parse(line[0]), sliced.ToArray());
            }
            maps = temp.ToArray();
        }
    }
}

