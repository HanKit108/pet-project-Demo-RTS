using System;
using System.Collections.Generic;

public class BasePool<T>
{
    private readonly Queue<T> _items;
    private readonly Func<T> _preloadFunc;
    private readonly Action<T> _takeAction;
    private readonly Action<T> _releaseAction;

    public BasePool(
        Func<T> preloadFunc, 
        Action<T> takeAction,
        Action<T> releaseAction, 
        int initialCount)
    {
        _items = new Queue<T>(initialCount);
        _preloadFunc = preloadFunc;
        _takeAction = takeAction;
        _releaseAction = releaseAction;

        for (int i = 0; i < initialCount; i++)
        {
            Release(preloadFunc());
        }
    }

    public T Take()
    {
        T item = _items.Count > 0 ? _items.Dequeue() : _preloadFunc();
        _takeAction(item);

        return item;
    }

    public void Release(T item)
    {
        _releaseAction(item);
        _items.Enqueue(item);
    }
}