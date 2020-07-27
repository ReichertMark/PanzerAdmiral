using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PanzerAdmiral.AI
{
    static class NodeBuilder
    {

        /// <summary>
        /// BuildGrid
        /// </summary>
        /// <param name="position">set Grid Position</param>
        /// <param name="numX">Number of nodes in X</param>
        /// <param name="numY">Number of nodes in Y</param>
        /// <param name="spacing"></param>
        public static void BuildGrid(Vector2 position, int numX, int numY, int spacing)
        {
            List<Node> nodes = new List<Node>();
            int count = numX * numY;
            for (int i = 0; i < count; i++)
            {
                int x = i % numX;   // reset to zero every row
                int y = i / numX;

                Node node = new Node();
                nodes.Add(node);
                node.Position = new Vector2(x * spacing, y * spacing) + position;

                if (x > 0)
                    node.BidiConnect(nodes[i - 1]); // left
                if (y > 0)
                    node.BidiConnect(nodes[i - numX]); // up
                if (x > 0 && y > 0)
                    node.BidiConnect(nodes[i - numX - 1]); // diagonal left - up
                if (x < numX - 1 && y > 0)
                    node.BidiConnect(nodes[i - numX + 1]); // diagonal right - up
            } // for (i)
        } // BuildGrid(position)

        /// <summary>
        /// BuildCircle
        /// </summary>
        /// <param name="position">set circle position</param>
        /// <param name="radius">radius of the circle</param>
        /// <param name="count">how many nodes we would like to use in the creation of the circle</param>
        /// <returns></returns>
        public static void BuildCircle(Vector2 position, float radius, int count)
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < count; i++)
            {
                Node node = new Node();
                nodes.Add(node);

                float rotation = ((float)i / count) * MathHelper.TwoPi;
                node.Position = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * radius + position;

                if (i > 0)
                    node.BidiConnect(nodes[i - 1]);

            } // for(i)

            nodes[0].BidiConnect(nodes[nodes.Count - 1]);

        } // BuildCircle(position, radius, count)

    } // NodeBuilder
} // namespace PanzerAdmiral.AI
