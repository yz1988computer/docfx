﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Newtonsoft.Json;

namespace Microsoft.DocAsCode.Plugins;

[JsonConverter(typeof(ManifestItemCollectionConverter))]
public class ManifestItemCollection : ObservableCollection<ManifestItem>
{
    public ManifestItemCollection() { }

    public ManifestItemCollection(IEnumerable<ManifestItem> collection)
        : base(collection) { }

    #region Overrides

    protected override void ClearItems()
    {
        var list = this.ToList();
        base.ClearItems();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list));
    }

    #endregion

    #region Binary Compatibility

    public new int Count => base.Count;

    public new void Add(ManifestItem item) => base.Add(item);

    public new void Clear() => base.Clear();

    public new bool Contains(ManifestItem item) => base.Contains(item);

    public new void CopyTo(ManifestItem[] array, int arrayIndex) => base.CopyTo(array, arrayIndex);

    public new IEnumerator<ManifestItem> GetEnumerator() => base.GetEnumerator();

    public new int IndexOf(ManifestItem item) => base.IndexOf(item);

    public new void Insert(int index, ManifestItem item) => base.Insert(index, item);

    public new bool Remove(ManifestItem item) => base.Remove(item);

    public new void RemoveAt(int index) => base.RemoveAt(index);

    #endregion

    public void AddRange(IEnumerable<ManifestItem> collection)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public int RemoveAll(Predicate<ManifestItem> match)
    {
        int count = 0;
        for (int i = Count - 1; i >= 0; i--)
        {
            if (match(this[i]))
            {
                RemoveAt(i);
                count++;
            }
        }
        return count;
    }
}
