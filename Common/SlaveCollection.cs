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
    public class SlaveCollection<TMaster, TSlave> : ObservableCollection<TSlave>
    {
        private ObservableCollection<TMaster> masterCollection;
        private Func<TMaster, TSlave> addSlave;
        private Func<TSlave, TMaster> whoIsTheMaster;

        public SlaveCollection(
            ObservableCollection<TMaster> masterCollection, 
            Func<TMaster, TSlave> addSlave, 
            Func<TSlave, TMaster> whoIsTheMaster
            ) => ObeyTheMaster(masterCollection, addSlave, whoIsTheMaster);

        public void ObeyTheMaster(
            ObservableCollection<TMaster> masterCollection,
            Func<TMaster, TSlave> addSlave,
            Func<TSlave, TMaster> whoIsTheMaster)
        {
            if (masterCollection == null)
            {
                throw new ArgumentNullException("A slave cannot exist without a Master");
            }

            if (this.masterCollection != null)
            {
                this.masterCollection.CollectionChanged -= DoIt;
                Clear();
            }

            this.masterCollection = masterCollection;
            this.addSlave = addSlave;
            this.whoIsTheMaster = whoIsTheMaster;

            foreach (var master in masterCollection)
            {
                Add(addSlave(master));
            }

            masterCollection.CollectionChanged += DoIt;
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
                    foreach (var master in e.NewItems.OfType<TMaster>())
                    {
                        Add(addSlave(master));
                    }
                    break;
            }
        }
    }
}
