using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LevelEditor
{
    public class Event
    {
        public int ID;
        public Rectangle Bounds;
        public string[] Parameters;
        public Event(int id, Rectangle rect, params string[] parameters)
        {
            ID = id;
            Bounds = rect;
            Parameters = parameters;
        }
    }
}
