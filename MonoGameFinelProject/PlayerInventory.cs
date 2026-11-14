using System;
using System.Collections.Generic;
namespace MonoGameFinelProject;

public class PlayerInventory
{
    private readonly Dictionary<string, int> _items = new();

    public IReadOnlyDictionary<string, int> Items => _items;

    public void AddItem(string itemId, int amount = 1)
    {
        if (amount <= 0)
            return;

        if (_items.ContainsKey(itemId))
            _items[itemId] += amount;
        else
            _items[itemId] = amount;
    }

    public bool TryRemoveItem(string itemId, int amount = 1)
    {
        if (!_items.ContainsKey(itemId))
            return false;

        if (_items[itemId] < amount)
            return false;

        _items[itemId] -= amount;

        if (_items[itemId] == 0)
            _items.Remove(itemId);

        return true;
    }

    public int GetQuantity(string itemId)
    {
        return _items.TryGetValue(itemId, out int quantity) ? quantity : 0;
    }

    public bool Contains(string itemId)
    {
        return _items.ContainsKey(itemId);
    }

}