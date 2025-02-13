using UnityEngine;

public class RangeAttackController
{
    public void LaunchProjectile
        (
        Transform prefab,
        Vector3 startPosition, 
        Transform targetTransform, 
        IDamagable target, 
        float damageAmount,
        float projectileSpeed,
        float verticalOffset
        )
    {
        Quaternion direction = Quaternion.LookRotation(targetTransform.position - startPosition);

        var entity = ServiceLocator.GetService<EntitySpawner>().SpawnEntity(startPosition, Quaternion.identity);


        ProjectileComponent projectileComponent = new ProjectileComponent
        (
        targetTransform,
        entity.transform,
        projectileSpeed,
        verticalOffset
        );
        VisualComponent visual = new VisualComponent(entity, prefab);
        entity.Add(visual);
        entity.Add(projectileComponent);

        projectileComponent.OnCompleted += () => ServiceLocator.GetService<EntityDisposer>().Dispose(entity);
        projectileComponent.OnCompleted += () => ServiceLocator.GetService<DealDamageController>().DealDamage(target, damageAmount);
    }
}
