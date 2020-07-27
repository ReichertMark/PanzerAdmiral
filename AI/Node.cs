using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PanzerAdmiral.ScreenSystem;
using Microsoft.Xna.Framework.Input;

namespace PanzerAdmiral.AI
{
    class Node : Actor
    {
        // Path Network
        public List<Node> Connected = new List<Node>();

        // A* State information
        public Node Parent;
        public bool Closed;
        /// <summary>Mark specific nodes as being part of the final path that got generated after the algorithm has run, purely for visuals</summary>
        public bool InPath;
        public float GScore; // various scores associated with this node
        public float HScore;
        public float FScore;

        public static bool DebugPaths = false;  // whether to draw Nodes and the lines between them

        public Node()
            : base("AI/circle16", Style.NodeColor)
        {
        } // Node()

        #region Reset
        public void Reset()
        {
            this.Parent = null;
            this.Closed = false;
            this.InPath = false;
            this.GScore = 0;
            this.HScore = 0;
            this.FScore = 0;
        } // Reset()
        #endregion

        #region ConnectTo
        /// <summary>
        /// Connects this Node to another node
        /// </summary>
        /// <param name="node">the node to connect with</param>
        /// <returns></returns>
        public void ConnectTo(Node node)
        {
            if (!this.Connected.Contains(node))
                this.Connected.Add(node);
        } // ConnectTo(node)
        #endregion

        #region DisconnectFrom
        /// <summary>
        /// DisconnectFrom
        /// </summary>
        /// <param name="node">the node to disconnect from</param>
        public void DisconnectFrom(Node node)
        {
            this.Connected.Remove(node);
        } // DisconnectFrom(node)
        #endregion

        #region BidiConnect
        /// <summary>
        /// Bidirectional Connect
        /// </summary>
        /// <param name="node">the node to connect with</param>
        public void BidiConnect(Node node)
        {
            this.ConnectTo(node);
            node.ConnectTo(this);
        } // BidiConnect(node)
        #endregion

        #region BidiDisconnect
        /// <summary>
        /// Bidirectional Disconnect
        /// </summary>
        /// <param name="node">the node to disconnect connect</param>
        /// <returns></returns>
        public void BidiDisconnect(Node node)
        {
            this.DisconnectFrom(node);
            node.DisconnectFrom(this);
        } // BidiDisconnect(node)
        #endregion

        #region Delete
        /// <summary> Delete, only loop through Nodes </summary>
        public override void Delete()
        {
            foreach (var node in Actor.Actors.OfType<Node>())
                node.DisconnectFrom(this);

            base.Delete();
        } // Delete()
        #endregion

        #region GetClosestNode
        /// <summary>
        /// Returns the closest node from the given Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Returns the closest node from the given Position</returns>
        public static Node GetClosestNode(Vector2 position)
        {
            Node result = null;
            float shortestDistance = float.PositiveInfinity;

            foreach (var node in Actor.Actors.OfType<Node>())
            {
                float distance = Vector2.Distance(node.Position, position);
                if (distance < shortestDistance)
                {
                    result = node;
                    shortestDistance = distance;
                } // if
            } // foreach
            return result;
        } // GetClosestNode(Vector2 position)
        #endregion

        #region GetRandomNode
        /// <summary>
        /// Gets a Random Node by randomizing the index and accessing the list of nodes
        /// </summary>
        /// <param name="random">Random seed</param>
        /// <returns>Node</returns>
        public static Node GetRandomNode(Random random)
        {
            IEnumerable<Node> nodes = Actor.Actors.OfType<Node>();
            return nodes.ElementAt(random.Next(nodes.Count()));
        } // GetRandomNode()
        #endregion

        #region Draw
        /// <summary> Draw connected Line </summary>
        public override void Draw()
        {
            if (ScreenManager.Input.IsNewKeyPress(Keys.X))
                DebugPaths = true;
                //DebugPaths = !DebugPaths;
            if (ScreenManager.Input.IsNewKeyPress(Keys.V))
                DebugPaths = false;

            if (!DebugPaths)
                return;

            // Draw lines
            foreach (var node in this.Connected)
            {
                Color lineColor = Style.LineColor;
                float lineLayer = Style.LineLayer;

                if (this.InPath && node.InPath && node == this.Parent) // figure out the nodes that are in the path
                {
                    lineColor = Style.PathColor;
                    lineLayer = Style.MarkerLayer;
                }

                Line.DrawLine(Style.FillTexture, lineColor, this, node, lineLayer);
            }

            // Draw parent indicators in red
            Node parent = this.Parent;
            if (parent != null && !this.InPath)
            {
                Vector2 direction = parent.Position - this.Position;
                float rotation = (float)Math.Atan2(direction.Y, direction.X);
                ScreenManager.SpriteBatch.Draw(Style.TailTexture, this.Position, null, Color.Red, rotation, 
                                               Style.TailOrigin, 1f, SpriteEffects.None, Style.ParentLayer);
            }

            // Draw Markers
            if (this.Closed || this.InPath)
            {
                Color markerColor = Style.ClosedColor;
                if (this.InPath)
                    markerColor = Style.PathColor;

                ScreenManager.SpriteBatch.Draw(Style.MarkerTexture, this.Position, null, markerColor, 0f, 
                                               Style.MarkerOrigin, 1f, SpriteEffects.None, Style.MarkerLayer);
            }



            // Draw Score Values
            if (this.Parent != null)
            {
                string text;
                Vector2 textDims;
                Vector2 position;



                text = String.Format("{0:0}", this.GScore);
                textDims = Style.FontSmall.MeasureString(text);
                position = this.Position + new Vector2(-this.Origin.X - textDims.X, this.Origin.Y);
                ScreenManager.SpriteBatch.DrawString(Style.FontSmall, text, position,  Style.DarkText, 0f, 
                                                     Vector2.Zero, 1f, SpriteEffects.None, Style.TextLayer);

                text = String.Format("{0:0}", this.HScore);
                textDims = Style.FontSmall.MeasureString(text);
                position = this.Position + new Vector2(this.Origin.X, this.Origin.Y);
                ScreenManager.SpriteBatch.DrawString(Style.FontSmall, text, position, Style.DarkText, 0f,
                                                     Vector2.Zero, 1f, SpriteEffects.None, Style.TextLayer);

                text = String.Format("{0:0}", this.FScore);
                textDims = Style.FontSmall.MeasureString(text);
                position = this.Position + new Vector2(-this.Origin.X - textDims.X, -this.Origin.Y - textDims.Y);
                ScreenManager.SpriteBatch.DrawString(Style.FontSmall, text, position, Style.BrightText, 0f,
                                                     Vector2.Zero, 1f, SpriteEffects.None, Style.TextLayer);


            }

            base.Draw();
        }
        #endregion
    } // class Node
} // namespace PanzerAdmiral.AI
