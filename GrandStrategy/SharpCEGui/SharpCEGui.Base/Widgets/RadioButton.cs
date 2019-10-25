#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Base class to provide the logic for Radio Button widgets.
    /// </summary>
    public class RadioButton : ToggleButton
    {
        /// <summary>
        /// Window factory name
        /// </summary>
        public new const string WidgetTypeName = "CEGUI/RadioButton";

        /// <summary>
        /// set the groupID for this radio button
        /// </summary>
        /// <param name="group">
        /// ulong value specifying the radio button group that this widget 
        /// belongs to.
        /// </param>
        public void SetGroupId(int group)
        {
            _groupId = group;

            if (Selected)
                DeselectSiblingRadioButtonsInGroup();
        }

        /// <summary>
        /// return the groupID assigned to this radio button
        /// </summary>
        /// <returns>
        /// ulong value that identifies the Radio Button group this widget
        /// belongs to.
        /// </returns>
        public int GetGroupId()
        {
            return _groupId;
        }

        /// <summary>
        /// Return a pointer to the RadioButton object within the same group as this
        /// RadioButton, that is currently selected.
        /// </summary>
        /// <returns>
        /// Pointer to the RadioButton object that is the RadioButton within the
        /// same group as this RadioButton, and is attached to the same parent
        /// window as this RadioButton, that is currently selected. Returns 0 if no
        /// button within the group is selected, or if 'this' is not attached to a
        /// parent window.
        /// </returns>
        public RadioButton GetSelectedButtonInGroup()
        {
            // Only search we we are a child window
            if (d_parent != null)
            {
                var childCount = d_parent.GetChildCount();

                // scan all children
                for (var child = 0; child < childCount; ++child)
                {
                    // is this child same type as we are?
                    if (GetParent().GetChildAtIdx(child).GetType() == GetType())
                    {
                        var rb = (RadioButton) GetParent().GetChildAtIdx(child);

                        // is child same group and selected?
                        if (rb.IsSelected() && (rb.GetGroupId() == _groupId))
                        {
                            // return the matching RadioButton pointer (may even be 'this').
                            return rb;
                        }

                    }

                }

            }

            // no selected button attached to this window is in same group
            return null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public RadioButton(string type, string name)
            : base(type, name)
        {
            _groupId = 0;
            AddRadioButtonProperties();
        }

        protected void DeselectSiblingRadioButtonsInGroup()
        {
            if (d_parent == null)
                return;

            var childCount = d_parent.GetChildCount();
            for (var child = 0; child < childCount; ++child)
            {
                var rb = GetParent().GetChildAtIdx(child) as RadioButton;
                if (rb != null)
                {
                    if (rb.IsSelected() && (rb != this) &&
                        (rb.GetGroupId() == _groupId))
                    {
                        rb.SetSelected(false);
                    }
                }
            }
        }

        protected void AddRadioButtonProperties()
        {
            AddProperty(new TplWindowProperty<RadioButton, int>(
                            "GroupID",
                            "Property to access the radio button group ID. Value is an unsigned integer number.",
                            (w, v) => w.SetGroupId(v), w => w.GetGroupId(), WidgetTypeName));
        }

        protected override bool GetPostClickSelectState()
        {
            return true;
        }

        protected override void OnSelectStateChange(WindowEventArgs e)
        {
            if (Selected)
                DeselectSiblingRadioButtonsInGroup();

            base.OnSelectStateChange(e);
        }

        #region Fields

        private int _groupId;

        #endregion
    }
}