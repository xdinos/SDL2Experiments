using System;
using System.Collections.Generic;

namespace SharpCEGui.Base.Views
{
    /// <summary>
    /// Base class for the items used by GenericItemModel. The item has a list
    /// of children attached to it, thus being able to represent a tree hierarchy.
    /// 
    /// It's important to understand that this is a simple class that only holds
    /// structured data. Direct changes to this item won't be propagated to the
    /// ItemModel it's part of, unless one links them manually. In order to modify
    /// this item and update the ItemModel (and consequently the view) you need to
    /// call ItemModel implementation's functions (e.g.:
    /// GenericItemModel::removeItem(const GenericItem*)).
    /// </summary>
    public class GenericItem : IEquatable<GenericItem>
    {
        public GenericItem() 
            : this(String.Empty, null)
        { }

        public /*explicit*/ GenericItem(string text)
            : this(text, null)
        { }

        public GenericItem(string text, string icon)
        {
            d_text = text;
            d_icon = icon;
            d_parent = null;
        }

        // TODO: 
        //virtual ~GenericItem()
        //{
        //    while (!d_children.empty())
        //    {
        //        GenericItem* item = d_children.back();
        //        d_children.pop_back();
        //        delete item;
        //    }
        //}

        public string GetText()
        {
            return d_text;
        }

        public void SetText(String val)
        {
            d_text = val;
        }

        public string GetIcon()
        {
            return d_icon;
        }

        public void SetIcon(string icon)
        {
            d_icon = icon;
        }

        public GenericItem GetParent()
        {
            return d_parent;
        }

        public void SetParent(GenericItem item)
        {
            d_parent = item;
        }

        public List<GenericItem> GetChildren()
        {
            return d_children;
        }
        
        /// <summary>
        /// Adds a child item to the item.
        /// </summary>
        /// <remarks>
        /// This method <b>does not</b> notify anyone of the child that was just
        /// added. If you want the notifications, you can use one of the following
        /// methods:
        /// - GenericItemModel::addItemAtPosition(GenericItem*, size_t)
        /// - GenericItemModel::addItemAtPosition(GenericItem*, const ModelIndex&, size_t)
        /// - GenericItemModel::insertItem(GenericItem*, const GenericItem*)
        /// </remarks>
        /// <param name="child"></param>
        public virtual void AddItem(GenericItem child)
        {
            d_children.Add(child);
            child.SetParent(this);
        }

        // TODO: ...
        //virtual bool operator== (const GenericItem& other) const;
        //virtual bool operator!= (const GenericItem& other) const;
        //virtual bool operator< (const GenericItem& other) const;

        //public static bool operator ==(GenericItem lhs, GenericItem rhs)
        //{
        //    return lhs.Equals(rhs);
        //}

        //public static bool operator !=(GenericItem lhs, GenericItem rhs)
        //{
        //    return !(lhs == rhs);
        //}

        public static int operator <(GenericItem lhs, GenericItem rhs)
        {
            return string.CompareOrdinal(lhs.d_text, rhs.d_text);
        }

        public static int operator >(GenericItem lhs, GenericItem rhs)
        {
            return string.CompareOrdinal(rhs.d_text, lhs.d_text);
        }

        public bool Equals(GenericItem other)
        {
            if (d_text != other.d_text) return false;
            if (d_icon != other.d_icon) return false;
            if (d_children.Count != other.d_children.Count) return false;

            for (var i = 0; i < other.d_children.Count; ++i)
            {
                if (d_children[i] != other.d_children[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GenericItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (d_text != null ? d_text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (d_icon != null ? d_icon.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (d_children != null ? d_children.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (d_parent != null ? d_parent.GetHashCode() : 0);
                return hashCode;
            }
        }

        #region Fields

        protected string d_text;
        protected string d_icon;
        protected GenericItem d_parent;
        protected readonly List<GenericItem> d_children = new List<GenericItem>();

        #endregion
    }
}