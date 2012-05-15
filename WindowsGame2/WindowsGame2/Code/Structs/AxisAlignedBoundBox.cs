using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MiningGame.Code.Structs
{
    public class AxisAlignedBoundBox
    {
        private int width = 0;
        private int height = 0;
        private Vector2 position;

        public AxisAlignedBoundBox(Rectangle r)
        {
            position = new Vector2(r.Center.X, r.Center.Y);
            width = r.Width;
            height = r.Height;
        }

        public Vector2 axisCollide(AxisAlignedBoundBox bound2)
        {
            AxisAlignedBoundBox bound1 = this;

            Vector2[] bound1HalfWidths = bound1.getHalfWidths(bound2);
            Vector2[] bound2HalfWidths = bound2.getHalfWidths(bound1);

            float yMove = 0;
            float xMove = 0;
            float multY = (bound1.position.Y < bound2.position.Y) ? -1 : 1;
            float multX = (bound1.position.X < bound2.position.X) ? -1 : 1;

            if (bound1.position.X < bound2.position.X)
            {
                xMove = bound1HalfWidths[0].X - bound2HalfWidths[0].X;
            }
            else if (bound1.position.X > bound2.position.X)
            {
                xMove = bound2HalfWidths[0].X - bound1HalfWidths[0].X;
            }
            if (xMove < 0) return Vector2.Zero;

            if (bound1.position.Y < bound2.position.Y)
            {
                yMove = bound1HalfWidths[1].Y - bound2HalfWidths[1].Y;
            }
            else if (bound1.position.Y > bound2.position.Y)
            {
                yMove = bound2HalfWidths[1].Y - bound1HalfWidths[1].Y;
            }

            //Not colliding.
            if (yMove < 0) return Vector2.Zero;

            if(yMove < xMove && yMove > 0) xMove = 0;
            else if(xMove < yMove && xMove > 0) yMove = 0;

            return new Vector2(xMove * multX, yMove * multY);
        }

        public Vector2[] getHalfWidths(AxisAlignedBoundBox bound2)
        {
            List<Vector2> ret = new List<Vector2>();
            if (this.position.X < bound2.position.X)
            {
                ret.Add(new Vector2(position.X + (width / 2), 0));
            }
            else
            {
                ret.Add(new Vector2(position.X - (width / 2), 0));
            }
            if (this.position.Y < bound2.position.Y)
            {
                ret.Add(new Vector2(0, position.Y + (height / 2)));
            }
            else
            {
                ret.Add(new Vector2(0, position.Y - (height / 2)));
            }

            return ret.ToArray();
        }
    }
}
