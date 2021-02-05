using System;
using System.Collections.Generic;
using System.Text;

namespace LevelEditor
{
    public class GameObject
    {
        public static List<GameObject> objects = new List<GameObject>();
        public GameObject()
        {
            objects.Add(this);
        }
        public virtual void Update()
        {

        }
        public virtual void Draw()
        {

        }
    }
}
