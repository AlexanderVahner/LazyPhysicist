using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LazyPhysicist.Common
{
    /// <summary>
    /// An ObservableCollection extension for binding TMaster Collection to TSlave Collection. Any changes in a TMaster Collection change a TSlave Collection. 
    /// Helpful for ViewModel Collections.
    /// </summary>
    public sealed class SlaveCollection<TMaster, TSlave> : ObservableCollection<TSlave>
    {
        private ObservableCollection<TMaster> masterCollection;
        private Func<TMaster, TSlave> addSlave;
        private Func<TSlave, TMaster> whoIsTheMaster;

        /// <summary>
        /// An ObservableCollection extension for binding TMaster Collection to TSlave Collection. Any changes in a TMaster Collection change a TSlave Collection. 
        /// Helpful for ViewModel Collections.
        /// Constructor without binding
        /// </summary>
        public SlaveCollection() { }

        /// <summary>
        /// An ObservableCollection extension for binding TMaster Collection to TSlave Collection. Any changes in a TMaster Collection change a TSlave Collection. 
        /// Helpful for ViewModel Collections.
        /// Constructor with binding
        /// </summary>
        /// <param name="masterCollection">Source Collection</param>
        /// <param name="addSlave">How to make TSlave for TMaster lambda</param>
        /// <param name="whoIsTheMaster">How to find out a TSlave's TMaster lambda</param>
        public SlaveCollection(
            ObservableCollection<TMaster> masterCollection, 
            Func<TMaster, TSlave> addSlave, 
            Func<TSlave, TMaster> whoIsTheMaster
            ) => ObeyTheMaster(masterCollection, addSlave, whoIsTheMaster);
        /// <summary>
        /// Bind Collections
        /// </summary>
        /// <param name="masterCollection">Source Collection</param>
        /// <param name="addSlave">How to make TSlave for TMaster lambda</param>
        /// <param name="whoIsTheMaster">How to find out a TSlave's TMaster lambda</param>
        /// <exception cref="ArgumentNullException">masterCollection is null</exception>
        public void ObeyTheMaster(
            ObservableCollection<TMaster> masterCollection,
            Func<TMaster, TSlave> addSlave,
            Func<TSlave, TMaster> whoIsTheMaster)
        {
            BreakFree();

            this.masterCollection = masterCollection ?? throw new ArgumentNullException("The slave needs a Master");
            this.addSlave = addSlave;
            this.whoIsTheMaster = whoIsTheMaster;

            foreach (var master in masterCollection)
            {
                Add(addSlave(master));
            }

            masterCollection.CollectionChanged += DoIt;
        }
        /// <summary>
        /// Bind Collections. Use only if TMaster and TSlave are equal, and only if you are too lazy to write simple "s => s, m => m" lambdas yourself
        /// </summary>
        /// <param name="masterCollection">Source Collection</param>
        /// <exception cref="InvalidOperationException">TMaster and TSlave are not equal</exception>
        public void ObeyTheMaster(ObservableCollection<TMaster> masterCollection)
        {
            if (Equals(typeof(TMaster), typeof(TSlave)))
            {
                throw new InvalidOperationException("The slave doesn't fit the Master");
            }
            ObeyTheMaster(masterCollection, m => (TSlave)(object)m, s => (TMaster)(object)s);
        }
        /// <summary>
        /// Unbind Collections
        /// </summary>
        public void BreakFree()
        {
            if (masterCollection != null)
            {
                masterCollection.CollectionChanged -= DoIt;
            }
            Clear();
        }

        private void DoIt(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var master in e.NewItems.OfType<TMaster>())
                    {
                        Add(addSlave(master));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var master in e.OldItems.OfType<TMaster>())
                    {
                        Remove(this.FirstOrDefault(slave => Equals(master, whoIsTheMaster(slave))));
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
            }
        }
    }
}
