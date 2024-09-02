using Cyberpunk2077SaveModManager.DataEntity;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Cyberpunk2077SaveModManager.DataSource
{
    public class VirtualSaveFileDataSource : IList, IEnumerable<SaveFile>, INotifyCollectionChanged, IItemsRangeInfo
    {
        private List<SaveFile> _saveFiles = [];

        public object this[int index]
        {
            get
            {
                if (!this._saveFiles[index].IsLoaded)
                {
                    this.LoadItem(index);
                }

                return this._saveFiles[index];
            }

            set => throw new NotImplementedException();
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => true;

        public int Count => this._saveFiles.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => new object();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(sender, e);
        }

        public int Add(object value)
        {
            this._saveFiles.Add((SaveFile)value);
            var index = this._saveFiles.Count - 1;
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));

            return index;
        }

        public void Clear()
        {
            this._saveFiles.Clear();
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(object value) => this._saveFiles.Contains((SaveFile)value);

        public void CopyTo(Array array, int index) => throw new NotImplementedException();

        public IEnumerator GetEnumerator() => this._saveFiles.GetEnumerator();

        IEnumerator<SaveFile> IEnumerable<SaveFile>.GetEnumerator() => this._saveFiles.GetEnumerator();

        public int IndexOf(object value) => this._saveFiles.IndexOf((SaveFile)value);

        public void Insert(int index, object value) => this._saveFiles[index] = (SaveFile)value;

        public void Remove(object value)
        {
            var index = this.IndexOf(value);
            if (index >= 0)
            {
                var removedItem = this._saveFiles[index];
                this.RemoveAt(index);
                this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            }
        }

        public void RemoveAt(int index)
        {
            var removedItem = this._saveFiles[index];
            this._saveFiles.RemoveAt(index);
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            this.HandleRangesChanged(visibleRange);

            foreach (var item in trackedItems)
            {
                this.HandleRangesChanged(item);
            }
        }

        public void Dispose() {}

        private async void LoadItem(int index)
        {
            if (this._saveFiles[index].IsLoaded)
            {
                return;
            }

            var oldItem = this._saveFiles[index];
            var newItem = await oldItem.LoadAsync();
            this._saveFiles[index] = newItem;
            this.OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
        }

        private void HandleRangesChanged(ItemIndexRange range)
        {
            for (int index = range.FirstIndex; index < range.FirstIndex + range.Length; index++)
            {
                if (!_saveFiles[index].IsLoaded)
                {
                    // Fetch the item asynchronously
                    this.LoadItem(index);
                }
            }
        }
    }
}
