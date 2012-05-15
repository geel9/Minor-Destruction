using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningGame.Code.Structs
{
    public class AnimationControlPoint
    {
        public string name { get; set; }
        //We don't use a Vector2 because that would take up a lot more space in the JSON.
        public float x { get; set; }
        public float y { get; set; }
        public AnimationControlPoint(string name, float x, float y)
        {
            this.name = name;
            this.x = x;
            this.y = y;
        }
        public AnimationControlPoint()
        {

        }
    }
}
