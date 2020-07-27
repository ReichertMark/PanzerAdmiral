using System.Collections.Generic;

namespace PanzerAdmiral.AI
{
    /// <summary> Actor Selection </summary>
    public partial class Actor
    {
        /// <summary> List of selected Actors </summary>
        public static List<Actor> Selection = new List<Actor>();

        /// <summary> Gets the last element in the Selection List  </summary>
        public static Actor LastSelected
        {
            get 
            {
                if (Selection.Count == 0)
                    return null;
                
                return Selection[Selection.Count - 1];
            }
        } // LastSelected

        /// <summary> Is this actor Selected ?</summary>
        public bool IsSelected
        {
            get { return Selection.Contains(this); }
        } // IsSelected

        /// <summary> Method for selecting this actor </summary>
        public void Select()
        {
            if (!this.IsSelected)
                Selection.Add(this);
        } // Select()

        /// <summary> Method for deselecting this actor </summary>
        public void Deselect()
        {
            Selection.Remove(this);
        } //  Deselect()

        public void ToggleSelect()
        {
            if (this.IsSelected)
                Selection.Remove(this);
            else
                Selection.Add(this);
        } // ToggleSelect()

    } // class Actor
} // namespace PanzerAdmiral.AI
