using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RatingTDS.Code
{
    public class EquationResult
    {

        public List<Vector2> points = new List<Vector2>();
        public Color drawColor = new Color();
        Vector2 xWindow = Vector2.Zero, yWindow = Vector2.Zero, xDist = Vector2.Zero, yDist = Vector2.Zero;
        public string equation = "";
        double incrementSize = 1;
        Vector2 center = Vector2.Zero;
        public EquationResult(Vector2 xWindow, Vector2 yWindow, Vector2 xDist, Vector2 yDist, string equation, double incrementSize, Vector2 center)
        {
            this.equation = equation;
            this.xDist = xDist;
            this.yDist = yDist;
            this.xWindow = xWindow;
            this.yWindow = yWindow;
            this.equation = equation;
            this.incrementSize = incrementSize;
            this.center = center;
            recomputeEquation();
            drawColor = new Color(Main.r.Next(0, 150), Main.r.Next(0, 150), Main.r.Next(0, 150));
        }

        public void recomputeEquation()
        {
            points.Clear();
            Dictionary<char, double> variables = new Dictionary<char,double>();
            variables.Add('r', Main.r.Next(0, 20));
            for (double x = xWindow.X; x < xWindow.Y * 1; x += incrementSize)
            {
                variables.Remove('x');
                variables.Remove('c');
                variables.Remove('s');
                variables.Remove('t');
                variables.Remove('a');
                variables.Add('x', x);
                variables.Add('c', Math.Cos(x));
                variables.Add('s', Math.Sin(x));
                variables.Add('t', Main.time);
                variables.Add('a', Math.Sin(Main.time));
                List<Tokenizer.Token> tokens = Tokenizer.DoTokens.tokenize(equation.ToCharArray(), variables);
                double y = Tokenizer.DoTokens.parseTokens(tokens, variables);
                double mult = -xDist.X;
                double multY = yDist.X;
                Vector2 plus = center;
                if (x < 0)
                {
                    mult = xDist.Y;
                }
                if (y < 0)
                {
                    multY = -yDist.Y;
                }
                double xx = x * mult;
                y *= multY;
                y += plus.Y;
                xx += plus.X;

                Vector2 p = new Vector2((float)xx, (float)y);
                points.Add(p);
            }
        }
    }
}
