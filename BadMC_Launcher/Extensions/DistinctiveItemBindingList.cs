using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Extensions;

public class DistinctiveItemBindingList<T> : BindingList<T> {
    public DistinctiveItemBindingList(IList<T> initialData) : base(initialData) {

    }
    public DistinctiveItemBindingList() {

    }
    protected override void InsertItem(int index, T item) {
        if (!Contains(item)) {
            base.InsertItem(index, item);
        }
    }
    protected override void SetItem(int index, T item) {
        if (!Contains(item)) {
            base.SetItem(index, item);
        }
    }
}
