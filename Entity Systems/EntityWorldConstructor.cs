using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityWorldConstructor : MonoBehaviour
{
    [SerializeField]
    private EntityConfigSO _entityConfig;
    [SerializeField]
    private TeamSO _team;

    public void Construct()
    {
        var entity = GetComponent<Entity>();
        ServiceLocator.GetService<EntityConstructor>().ConstructEntity(entity, _entityConfig);
        ServiceLocator.GetService<EntityTeamInstaller>().TrySetTeam(entity, _team);
    }
}
