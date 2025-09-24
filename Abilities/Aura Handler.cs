using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AurasHandler
{
    [SerializeField]
    Dictionary<long, AuraHandler> _handlers = new();

    public void AddEntity(Entity entity, Action<Entity> castAction, int id, float delay)
    {
        if (_handlers.TryGetValue(id, out var other))
        {
            other.AddEntity(entity);
        }
        else
        {
            var handler = new AuraHandler(castAction, delay);
            handler.AddEntity(entity);
            _handlers[id] = handler;
        }
    }

    public void RemoveEntity(Entity entity, int id)
    {
        if (_handlers.TryGetValue(id, out var other))
        {
            other.RemoveEntity(entity);
        }
    }
}

[Serializable]
public class AuraHandler
{
    private List<Entity> _entities = new();
    private TickTimer _timer;
    private Action<Entity> _castAction;
    private float _delay;

    public AuraHandler(Action<Entity> castAction, float delay)
    {
        _castAction = castAction;
        _delay = delay;
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
        if (_timer == null)
        {
            _timer = ServiceLocator.GetService<TimerSystem>().CreateTickTimer(_delay, Cast);
        }
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
        if (_entities.Count == 0)
        {
            _timer.Abort();
            _timer = null;
        }
    }

    private void Cast()
    {
        for(int i = 0; i < _entities.Count; i++)
        {
            _castAction(_entities[i]);
        }
    }
}