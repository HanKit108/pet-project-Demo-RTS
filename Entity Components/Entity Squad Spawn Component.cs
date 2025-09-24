using System.Collections.Generic;
using UnityEngine;

public class EntitySquadSpawnComponent : BaseConditionComponent, IComponent
{
    private Entity _owner;
    private EntityGroupSO _entitySquad;

    public EntitySquadSpawnComponent(Entity owner, EntityGroupSO entitySquad, string name)
    {
        _name = name;
        _owner = owner;
        _entitySquad = entitySquad;
    }

    public List<Entity> SpawnEntitySquad()
    {
        if (_compositeConditions.Invoke())
        {
            var entities = ServiceLocator.GetService<EntitySpawner>().SpawnEntityGroup(
            _entitySquad,
            _owner.transform.position, Quaternion.identity);
            ServiceLocator.GetService<EntityTeamInstaller>().TryReplicateTeam(_owner, entities);
            return entities;
        }
        return null;
    }
}

public class SpawnerComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string SPAWNER_COMPONENT_NAME = "Spawner Component";
    
    [SerializeField]
    private EntityGroupSO _entitySquad;

    public SpawnerComponentCreator()
    {
        _name = SPAWNER_COMPONENT_NAME;
    }
    public void CreateComponent(Entity entity)
    {
        EntitySquadSpawnComponent spawner = new EntitySquadSpawnComponent(entity, _entitySquad, _name);

        TryAddDeathCondition(entity, spawner);

        var manager = ServiceLocator.GetService<SpawnerManager>();
        manager.AddSpawner(spawner);
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            health.OnDied += () => manager.RemoveSpawner(spawner);
        }

        entity.Add(spawner);
    }
}