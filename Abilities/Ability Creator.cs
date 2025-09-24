using System.Collections.Generic;
using UnityEngine;

public class AbilityCreator : MonoBehaviour
{
    [SerializeField]
    private AbilityConfigSO _abilitySO;
    [SerializeField]
    private float _radius, _cooldown;
    [SerializeField]
    private LayerMask _layerMask;
    public bool add;

    private void Start()
    {
        ServiceLocator.GetService<TimerSystem>().CreateTickTimer(_cooldown, Cast);
    }

    private void Cast()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radius, _layerMask);
        List<Entity> entities = new();

        foreach (var hitCollider in hitColliders)
        {
            var entity = hitCollider.GetComponentInParent<Entity>();
            if (
                entity != null &&
                entity.TryGetComponent<AbilitiesComponent>(out var abilities)
                )
            {
                if (add)
                {
                    abilities.Add(_abilitySO);
                }
                else
                {
                    abilities.RemoveAll();
                }
            }
        }
    }
}
