using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager
{
    private Transform _targetTransform;
    private List<EntitySquadSpawnComponent> _spawners = new();

    public SpawnerManager(Transform target, float tickDelay)
    {
        _targetTransform = target;
        ServiceLocator.GetService<TimerSystem>().CreateTickTimer(tickDelay, SpawnAndAttack);
    }

    private void SpawnAndAttack()
    {
        if(_spawners.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, _spawners.Count);
            var entities = _spawners[index].SpawnEntitySquad();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].TryGetComponent<UnitControlComponent>(out var unitControl))
                {
                    unitControl.OrderToAutoattackMove(_targetTransform.position);
                }
            }
        }
    }

    public void AddSpawner(EntitySquadSpawnComponent spawner)
    {
        _spawners.Add(spawner);
    }

    public void RemoveSpawner(EntitySquadSpawnComponent spawner)
    {
        _spawners.Remove(spawner);
    }
}