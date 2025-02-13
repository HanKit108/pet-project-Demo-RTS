using UnityEngine;

public class TestPlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private EntityGroupSO _entitySquad;
    [SerializeField]
    private TeamSO _team;

    private void Awake()
    {
        SquadControl.OnSpawned += OnSpawned;
    }

    private void OnSpawned(Vector3 position)
    {
        ServiceLocator.GetService<EntityTeamInstaller>().TrySetTeam(ServiceLocator.GetService<EntitySpawner>().SpawnEntityGroup(_entitySquad, position, Quaternion.identity), _team);
    }
}