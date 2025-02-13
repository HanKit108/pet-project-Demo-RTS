using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner
{
    private const float OFFSET_VALUE = 0.1f;

    public Entity SpawnEntity(EntityConfigSO entityConfig, Vector3 position, Quaternion rotation)
    {
        var entity = SpawnEntity(position, rotation);
        ServiceLocator.GetService<EntityConstructor>().ConstructEntity(entity, entityConfig);
        
        return entity;
    }

    public List<Entity> SpawnEntityGroup(EntityGroupSO entityGroup, Vector3 position, Quaternion rotation)
    {
        List<Entity> entities = new();

        Vector3 offset = new Vector3(Random.Range(-OFFSET_VALUE, OFFSET_VALUE), 0f, Random.Range(-OFFSET_VALUE, OFFSET_VALUE));
        foreach (var entityConfig in entityGroup.GetEntityConfigList())
        {
            entities.Add(SpawnEntity(entityConfig, position + offset, rotation));
        }
        return entities;
    }


    public Entity SpawnEntity(Vector3 position, Quaternion rotation)
    {
        Vector3 offset = new Vector3(Random.Range(-OFFSET_VALUE, OFFSET_VALUE), 0f, -OFFSET_VALUE);
        Entity entity = (Entity)ServiceLocator.GetService<PoolsContainer>().GetEntityPool().Take();
        entity.transform.position = position + offset;
        entity.transform.rotation = rotation;
        entity.transform.parent = ServiceLocator.GetService<PoolsContainer>().WorldTransform;
        return entity;
    }
}
