using EasyButtons;
using UnityEngine;

public class TestVisualManager : MonoBehaviour
{
    [SerializeField]
    private Transform _prefab;

    [SerializeField]
    private Entity _target;

    [Button]
    public void SetVisual()
    {
        if (_target != null && _target.TryGetComponent<VisualComponent>(out var visual))
        {
            visual.SetVisual(_prefab);
        }
    }
}
