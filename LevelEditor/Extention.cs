using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;

namespace LevelEditor
{
    public static class Helper
    {
        /// <summary>
        /// Loads texture from file
        /// </summary>
        /// <param name="path">absolute file path</param>
        /// <returns>the loaded Texture</returns>
        public static Texture2D LoadTexture(string path)
        {
            using FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(Main.selectedFile)) + @"\Content\" + path + ".png", FileMode.Open);
            return Texture2D.FromStream(Main.graphics.GraphicsDevice, fileStream);
        }

        /// <summary>
        /// Loads a part of a Texture from path defined by a sourceRectangle
        /// </summary>
        /// <param name="path">absolute file path</param>
        /// <param name="srcRect">source Rectangle which defines what Part of the Texture is loaded</param>
        /// <returns>A Texture2D containing the specified part</returns>
        public static Texture2D LoadTexturePart(string path, Rectangle srcRect)
        {
            using FileStream fileStream = new FileStream(Path.GetDirectoryName(Path.GetDirectoryName(Main.selectedFile)) + @"\Content\" + path + ".png", FileMode.Open);

            Texture2D wholeTex = Texture2D.FromStream(Main.graphics.GraphicsDevice, fileStream);
            Texture2D returnTex = new Texture2D(Main.instance.GraphicsDevice, srcRect.Width, srcRect.Height);
            Color[] data = new Color[srcRect.Width * srcRect.Height];
            wholeTex.GetData(0, srcRect, data, 0, data.Length);
            returnTex.SetData(data);
            return returnTex;
        }
    }

    public static class Extention
    {
        public static bool IsValid(this char c)
        {
            char[] validChars = {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Ä', 'Ö', 'Ü',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ä', 'ö', 'ü',
                '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '.', ',', ';', ':', '!', '$', '%', '&', '/', '(', ')', '=', '?', '{', '}', '[', ']',
                 '@', '€', '°', '^', '<', '>', '|', '\'', '#', '+', '*', '~', '\"', '\\', ' ', '|' };
            for (int i = 0; i < validChars.Length; i++)
            {
                if (validChars.Contains(c))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
