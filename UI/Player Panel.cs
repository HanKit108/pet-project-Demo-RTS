using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel
{
    private Transform _buttonsPanel, _queuePanel;

    private List<Button> _items = new List<Button>();
    private Queue<Button> _queue = new Queue<Button>();

    public PlayerPanel(Transform buttonsPanel, Transform queuePanel)
    {
        _buttonsPanel = buttonsPanel;
        _queuePanel = queuePanel;
    }

    public void RemoveFromQueue()
    {
        var pool = ServiceLocator.GetService<PoolsContainer>().GetButtonPool();
        pool.Release(_queue.Dequeue());
    }

    public void AddToQueue(Sprite icon)
    {
        var item = (Button)ServiceLocator.GetService<PoolsContainer>().GetButtonPool().Take();
        item.transform.SetParent(_queuePanel);
        item.transform.GetChild(0).GetComponent<Image>().sprite = icon;
        _queue.Enqueue(item);
    }

    public void AddButton(Action action, Sprite icon)
    {
        var pool = ServiceLocator.GetService<PoolsContainer>().GetButtonPool();
        var item = (Button)pool.Take();
        item.transform.SetParent(_buttonsPanel);
        item.onClick.AddListener(() => action());
        item.transform.GetChild(0).GetComponent<Image>().sprite = icon;
        _items.Add(item);
    }

    public void ClearPanel()
    {
        foreach (var item in _items)
        {
            item.onClick.RemoveAllListeners();
            ServiceLocator.GetService<PoolsContainer>().GetButtonPool().Release(item);
        }
        int count = _queue.Count;
        for (int i = 0; i < count; i++)
        {
            ServiceLocator.GetService<PoolsContainer>().GetButtonPool().Release(_queue.Dequeue());
        }
        _items.Clear();
    }
}
