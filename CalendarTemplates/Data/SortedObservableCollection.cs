using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace net.reidemeister.wp.CalendarTemplates.Data
{
    // http://www.xamlplayground.org/post/2010/04/27/Keeping-an-ObservableCollection-sorted-with-a-method-override.aspx
    public class SortedObservableCollection<T> : ObservableCollection<T> 
    {
        private IComparer<T> comparer;

        public SortedObservableCollection()
        {
        }

        public SortedObservableCollection(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        protected override void InsertItem(int index, T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                int result = this.comparer == null ? (this[i] as IComparable).CompareTo(item) : this.comparer.Compare(this[i], item);
                switch (Math.Sign(result))
                {
                    case 0:
                    case 1:
                        base.InsertItem(i, item);
                        return;
                    case -1:
                        break;
                }
            }
            base.InsertItem(this.Count, item);
        }
    }
}
