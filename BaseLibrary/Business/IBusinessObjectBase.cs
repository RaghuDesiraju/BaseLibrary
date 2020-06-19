using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BaseLibrary.Business
{
    /// Interface for the Business Base class used in WPF 
    /// </summary>
    public interface IBusinessObjectBase : INotifyPropertyChanged, INotifyCollectionChanged, IEditableObject
    {
        bool IsObjectValid { get; }
        bool IsObjectNew { get; }
        bool IsObjectDirty { get; }
        bool IsObjectDeleted { get; }
        bool IsObjectSavable { get; }
        event ItemEndEditEventHandler ItemEndEdit;
    }
}
