using System.Collections.Generic;
using UnityEngine;

public class UpdateManager
{
    [SerializeReference]
    private List<IUpdatable> _updatables = new();
    [SerializeReference]
    private List<ILateUpdatable> _lateUpdatables = new();

    public void Add(ITickable tickable)
    {
        if(tickable is IUpdatable updatable)
        {
            _updatables.Add(updatable);
        }
        if (tickable is ILateUpdatable lateUpdatable)
        {
            _lateUpdatables.Add(lateUpdatable);
        }
    }

    public void Remove(ITickable tickable)
    {
        if (tickable is IUpdatable updatable)
        {
            _updatables.Remove(updatable);
        }
        if (tickable is ILateUpdatable lateUpdatable)
        {
            _lateUpdatables.Remove(lateUpdatable);
        }
    }


    public void Update()
    {
        for(int i = 0; i < _updatables.Count; i++)
        {
            _updatables[i].OnUpdate(Time.deltaTime);
        }
    }

    public void LateUpdate()
    {
        for (int i = 0; i < _lateUpdatables.Count; i++)
        {
            _lateUpdatables[i].OnLateUpdate(Time.deltaTime);
        }
    }
}
