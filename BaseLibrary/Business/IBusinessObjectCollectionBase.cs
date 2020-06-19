using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BaseLibrary.Business
{
    public interface IBusinessObjectCollectionBase<T> : ICollection<T> where T : IBusinessObjectBase, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}
