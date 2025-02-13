using UnityEngine;

public class EntityConstructor
{
    public void ConstructEntity(Entity entity, EntityConfigSO entityConfig)
    {
        entity.gameObject.name = entityConfig.Name;

        foreach (IComponentCreator stats in entityConfig.GetStats())
        {
            stats.CreateComponent(entity);
        }

        //UpdateManager.instance.Add(entity);
        //entity.transform.localScale = new Vector3(entityConfig.Size, entityConfig.Size, entityConfig.Size);
    }
}