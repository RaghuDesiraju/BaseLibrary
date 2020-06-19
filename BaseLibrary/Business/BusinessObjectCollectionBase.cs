using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BaseLibrary.Business
{
    /// <summary>
    /// Delegate to provide notification in Business Base Collection Class of WPF
    /// </summary>
    /// <param name="sender"></param>
    [Serializable()]
    public delegate void ItemEndEditEventHandler(IEditableObject sender);

    /// <summary>
    /// Collection class for business object in WPF
    /// </summary>
    [Serializable()]
    public class BusinessObjectCollectionBase<T> : ObservableCollection<T> where T : IBusinessObjectBase
    {
        public delegate void ItemAddedDel(T item);
        public delegate void ItemRemovedDel(T item);
        public event ItemAddedDel ItemAdded;
        public event ItemRemovedDel ItemRemoved;
        public event ItemEndEditEventHandler ItemEndEdit;
        private List<T> deletedItemsList;



        public BusinessObjectCollectionBase()
            : base()
        {


        }
        public BusinessObjectCollectionBase(IList<T> list)
            : base(list)
        {


        }
        // This will use Comparer<T>.Default
        /// <summary>
        /// This will use Comparer<T>.Default
        /// </summary>
        public void Sort()
        {
            this.Sort(0, Count, null);
        }

        // Pass custom comparer to Sort method
        /// <summary>
        /// Pass custom comparer to Sort method
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, Count, comparer);
        }

        // Use this method to sort part of a collection
        /// <summary>
        /// Use this method to sort part of a collection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="comparer"></param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            (Items as List<T>).Sort(index, count, comparer);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public List<T> ToList
        {
            get
            {
                return (Items as List<T>);
            }
        }
        public List<T> DeletedItems
        {
            get
            {
                if (deletedItemsList == null)
                {
                    deletedItemsList = new List<T>();
                }
                return deletedItemsList;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this object's data has been changed.
        /// </summary>
        public bool IsObjectDirty
        {
            get
            {
                // any deletions make us dirty
                if (DeletedItems.Count > 0) return true;

                if (this.Items == null || this.Items.Count == 0)
                    return false;

                if (this.ToList.Find(p => p.IsObjectDirty) != null)
                    return true;
                else
                    return false;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this object is currently in
        /// a valid state.
        /// </summary>
        public virtual bool IsObjectValid
        {
            get
            {
                // run through all the child objects
                // and if any are invalid then the
                // collection is invalid
                if (this.Items == null || this.Items.Count == 0)
                    return false;

                if (this.ToList.Find(p => !p.IsObjectValid) != null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Insert item at an index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            if (item.IsObjectDeleted)
            {
                throw new ArgumentException("Cannot add item as it is marked deleted i.e. item.IsDeleted = true");
            }
            base.InsertItem(index, item);
            item.ItemEndEdit += new ItemEndEditEventHandler(ItemEndEditHandler);
            if (ItemAdded != null)
            {
                ItemAdded(item);
            }
        }

        void ItemEndEditHandler(IEditableObject sender)
        {
            // simply forward any EndEdit events
            if (ItemEndEdit != null)
            {
                ItemEndEdit(sender);
            }
        }

        /// <summary>
        /// remove the item at an index
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            T item = this[index];
            base.RemoveItem(index);
            DeletedItems.Add(item);

            if (ItemRemoved != null)
            {
                ItemRemoved(item);
            }

        }

        /// <summary>
        /// Clear the item from observable collection
        /// </summary>
        protected override void ClearItems()
        {
            foreach (T item in this.Items)
            {
                DeletedItems.Add(item);
            }
            base.ClearItems();
        }


    }
}
