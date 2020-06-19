using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BaseLibrary.Business
{
    /// <summary>
    /// Base class for all the business object in Windows/Web/WPF. Not yet intended for MVC pattern
    /// </summary>
    [Serializable()]
    public class BusinessObjectBase : IBusinessObjectBase
    {

        #region Constructors

        public BusinessObjectBase()
        {
        }

        #endregion

        #region IsNew, IsDeleted, IsDirty, IsSavable

        // keep track of whether we are new, deleted or dirty
        private bool _isObjectNew = true;
        private bool _isObjectDeleted;
        private bool _isObjectDirty = true;

        /// <summary>
        /// can override this property to say if the object is valid or not
        /// </summary>
        public virtual bool IsObjectValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns <see langword="true" /> if object is new and is not saved to database, 
        /// <see langword="false" /> if the object is already saved.
        /// </summary>        
        public bool IsObjectNew
        {
            get { return _isObjectNew; }
        }

        /// <summary>
        /// Returns if object is deleted
        /// </summary>
        public bool IsObjectDeleted
        {
            get { return _isObjectDeleted; }
        }

        /// <summary>
        /// Returns <see langword="true" /> if this object's properties or data has been changed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the set of each property call MarkDirty() to indicate data has changed
        /// </para>
        /// </remarks>
        /// <returns>A value indicating if this object's data has been changed.</returns>
        public virtual bool IsObjectDirty
        {
            get { return _isObjectDirty; }
        }

        /// <summary>
        /// Marks an object as New by default. If the object is
        /// </summary>
        public virtual void MarkNew()
        {
            _isObjectNew = true;
            _isObjectDeleted = false;
            MarkDirty();
        }

        /// <summary>
        /// Mark the object as old
        /// </summary>
        public virtual void MarkOld()
        {
            _isObjectNew = false;
            MarkClean();
        }

        /// <summary>
        /// Mark the object as deleted as remove it at later time when removed from database
        /// </summary>
        public void MarkDeleted()
        {
            _isObjectDeleted = true;
            MarkDirty();
        }

        /// <summary>
        /// Any variable property changed mark it as dirty
        /// </summary>
        public void MarkDirty()
        {
            _isObjectDirty = true;
        }


        /// <summary>
        /// cleans the dirty flag
        /// </summary>
        public void MarkClean()
        {
            _isObjectDirty = false;
        }

        /// <summary>
        /// If the object is dirty and valid. Can be used only if want to save
        /// </summary>
        public virtual bool IsObjectSavable
        {
            get { return (IsObjectDirty && IsObjectValid); }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This is needed to support WPF
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {

            if (this.PropertyChanged != null)

                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion

        #region IBusinessBase Members


        public event ItemEndEditEventHandler ItemEndEdit;

        #endregion

        #region IEditableObject Members

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
            if (ItemEndEdit != null)
            {
                ItemEndEdit(this);
            }
        }

        #endregion
    }
}
