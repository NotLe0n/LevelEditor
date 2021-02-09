﻿using System;
using System.Linq;

namespace ExtentionMethods
{
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
