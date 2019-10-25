using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Handler class for undo support
    /// </summary>
    public class UndoHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        public UndoHandler(Window window)
        {
            d_undoLimit = 10;
            d_undoPosition = -1;
            d_lastUndo = false;

            d_attachedWindow = window;
            d_undoList.Add(new UndoAction());
        }

        /// <summary>
        /// enum representing undo type (insert, delete)
        /// </summary>
        public enum UndoActionType
        {
            /// <summary>
            /// 
            /// </summary>
            UAT_INSERT = 1,

            /// <summary>
            /// 
            /// </summary>
            UAT_DELETE = 2
        }

        /// <summary>
        /// Struct used to store information about undo actions
        /// </summary>
        public struct UndoAction
        {
            public UndoActionType d_type; //!< Undo type
            public int d_startIdx; //!< Starting index for this line.
            public string d_text;
        }

        /// <summary>
        /// Perform undo
        /// </summary>
        /// <param name="cursor">
        /// new cursor position
        /// </param>
        /// <returns></returns>
        public bool Undo(ref int cursor)
        {
            if (d_attachedWindow == null)
                return false;

            if (CanUndo())
            {
                if (d_undoPosition < d_undoList.Count)
                {
                    var action = d_undoList[d_undoPosition--];

                    var newText = d_attachedWindow.GetText();
                    if (action.d_type == UndoActionType.UAT_INSERT)
                    {
                        newText = newText.Remove(action.d_startIdx, action.d_text.Length);
                        cursor = action.d_startIdx;
                    }
                    else
                    {
                        newText = newText.Insert(action.d_startIdx, action.d_text);
                        cursor = action.d_startIdx + action.d_text.Length;
                    }
                    d_attachedWindow.SetText(newText);
                    d_lastUndo = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Perform redo
        /// </summary>
        /// <param name="cursor">
        /// new cursor position
        /// </param>
        /// <returns></returns>
        public bool Redo(ref int cursor)
        {
            if (d_attachedWindow == null)
                return false;

            if (CanRedo())
            {
                var action = d_undoList[++d_undoPosition];

                var newText = d_attachedWindow.GetText();
                if (action.d_type == UndoActionType.UAT_INSERT)
                {
                    newText = newText.Insert(action.d_startIdx, action.d_text);
                    cursor = action.d_startIdx + action.d_text.Length;
                }
                else
                {
                    newText = newText.Remove(action.d_startIdx, action.d_text.Length);
                    cursor = action.d_startIdx;
                }
                d_attachedWindow.SetText(newText);
                d_lastUndo = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a single record to undo history
        /// </summary>
        /// <param name="action"></param>
        public void AddUndoHistory(UndoAction action)
        {
            // discard all actions that are after our current position in undo list
            while (d_undoList.Count > 0 && d_undoPosition < d_undoList.Count - 1)
                d_undoList.RemoveAt(d_undoList.Count - 1);

            if (d_undoList.Count > d_undoLimit)
                d_undoList.RemoveAt(0);

            d_undoList.Add(action);
            d_undoPosition = d_undoList.Count - 1;
        }

        /// <summary>
        /// Clear undo history
        /// </summary>
        public void ClearUndoHistory()
        {
            d_undoList.Clear();
            d_undoPosition = -1;
        }

        /// <summary>
        /// Set maximum number of undo steps to store.
        /// </summary>
        /// <param name="limit">
        /// size of undo list
        /// </param>
        public void SetUndoLimit(int limit)
        {
            if (limit < 0)
                throw new InvalidRequestException("Invalid undo limit. Limit cannot be less then zero.");

            d_undoLimit = limit;

            while (d_undoList.Count > d_undoLimit)
                d_undoList.RemoveAt(0);

            d_undoPosition = d_undoList.Count - 1;
        }

        /// <summary>
        /// Get count of undo actions in queue
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return d_undoList.Count;
        }

        /// <summary>
        /// Checks if undo action is possible
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return (d_undoList.Count > 0 && d_undoPosition >= 0);
        }

        /// <summary>
        /// Checks if redo action is possible
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return (d_undoList.Count > 0 && d_undoPosition < (d_undoList.Count - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UndoAction GetLastAction()
        {
            UndoAction lastAction;

            if (d_lastUndo)
            {
                if (d_undoPosition == d_undoList.Count - 1)
                    lastAction = d_undoList[d_undoPosition];
                else
                    lastAction = d_undoList[d_undoPosition + 1];
            }
            else
            {
                if (d_undoPosition <= 0)
                    lastAction = d_undoList[0];
                else
                    lastAction = d_undoList[d_undoPosition];
            }
            return lastAction;
        }

        #region Fields

        private int d_undoLimit;        //!< Maximum numer of undo entries
        private int d_undoPosition;     //!< Position in undo list
        private bool d_lastUndo;         //!< True if last function called was undo

        private readonly Window d_attachedWindow;   //!< Pointer to window that we will handle
        private readonly List<UndoAction> d_undoList = new List<UndoAction>();         //!< Holds the undo history

        #endregion
    }
}