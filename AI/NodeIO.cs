using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using System.Windows.Forms;     // for open/save dialog

namespace PanzerAdmiral.AI
{
    /// <summary>
    /// Note: Careful with floats and split function! -> go to XML
    /// Todo: ErrorHandling
    /// </summary>
    public static class NodeIO
    {
        // loading and saving dialogs
        static OpenFileDialog dialogOpen = new OpenFileDialog();
        static SaveFileDialog dialogSave = new SaveFileDialog();

        /// <summary>Method that will invoke the save dialog, Saves all the Nodes</summary>
        public static void ShowSaveDialog()
        {
            // Go ahead and save the file
            if (dialogSave.ShowDialog() == DialogResult.OK)
                SaveNodes(dialogSave.FileName);
        } // ShowSaveDialog()

        /// <summary>Method that will invoke the Load dialog, Loads all the Nodes in Actors </summary>
        public static void ShowLoadDialog()
        {
            if (dialogOpen.ShowDialog() == DialogResult.OK)
            {
                // Loop all of our actors and delete them if it's a node
                for (int i = Actor.Actors.Count - 1; i >= 0; i--)
                {
                    if (Actor.Actors[i] is Node)
                        Actor.Actors[i].Delete();
                } // for (i)

                LoadNodes(dialogOpen.FileName);
            } // if
        } // ShowLoadDialog()

        #region SaveNodes
        /// <summary>
        /// Saves the Node to a .txt file
        /// nLines to describe nodes
        /// cLines to describe connection
        /// </summary>
        /// <param name="filename">the filename the created node file will have</param>
        public static void SaveNodes(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                // Create the dictionary to map id numbers to specific nodes
                Dictionary<Node, int> nodeIdMap = new Dictionary<Node, int>();

                // save the nodes
                IEnumerable<Node> nodes = Actor.Actors.OfType<Node>();
                foreach (var node in nodes)
                {
                    int nodeId = nodeIdMap.Count + 1; // Our first actual id number is 1
                    nodeIdMap[node] = nodeId;         // assign the node Id
                    writer.WriteLine("n,{0},{1},{2}", nodeId, (int)node.Position.X, (int)node.Position.Y); // now write the node, needs cast! comma sucks as splitter when using float values
                }

                // save the connections
                foreach (var node in nodes)
                {
                    foreach (var neighbor in node.Connected)
                    {
                        writer.WriteLine("c,{0},{1}", nodeIdMap[node], nodeIdMap[neighbor]);  // now write the connections
                    } // foreach neighbor
                } // foreach node
            } // using writer
        } // SaveNodes(filename)
        #endregion

        #region LoadNodes
        /// <summary>
        /// Loads Nodes from filename /bin/x86/debug/ or /bin/x86/Release/
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static void LoadNodes(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                // create the nodeRefMap
                Dictionary<int, Node> nodeRefMap = new Dictionary<int, Node>();

                // continue reading until the end of the stream, TODO Errorhandling
                while (!reader.EndOfStream)
                {
                    string[] fields = reader.ReadLine().Split(',');


                    if (fields[0] == "n") // watch out for node lines
                    {
                        // Load id and Position
                        int nodeId = Convert.ToInt32(fields[1]);
                        float x = Convert.ToSingle(fields[2]);      // float = single
                        float y = Convert.ToSingle(fields[3]);

                        // we now have all our informations for the node line, create the node
                        Node node = new Node();
                        node.Position = new Vector2(x, y);
                        nodeRefMap[nodeId] = node;
                    }
                    else if (fields[0] == "c") // watch out for connection lines
                    {
                        // Load the node and neighbor id
                        int nodeId = Convert.ToInt32(fields[1]);
                        int neighborId = Convert.ToInt32(fields[2]);

                        // assign the node and its neighbor to the refMap 
                        Node node = nodeRefMap[nodeId];
                        Node neighbor = nodeRefMap[neighborId];

                        // connect the node to the Neighbor
                        node.ConnectTo(neighbor);

                    } // else if

                } // while
            } // using reader
        } // LoadNodes(filename)
        #endregion
    } // class NodeIO
} // namespace PanzerAdmiral.AI
