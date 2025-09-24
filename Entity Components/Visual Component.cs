using UnityEngine;

public class VisualComponent : BaseComponent, IComponent, IDisposable
{
    private const string ANIMATION_COMPONENT_NAME = "Animation Component";

    private Transform _transform;
    private Pool _basePool;
    private Entity _owner;

    public Transform Transform => _transform;

    public VisualComponent(Entity entity, Transform prefab, string name)
    {
        _name = name;
        _owner = entity;

        SetVisual(prefab);
    }
    
    public void Dispose()
    {
        ReturnModel();
    }

    public void SetVisual(Transform prefab)
    {
        ReturnModel();
        SetModel(prefab);
        if (_transform.TryGetComponent<Animator>(out var animator))
        {
            if (!_owner.TryGetComponent<AnimationComponent>(out var animation))
            {
                animation = new AnimationComponent(animator, ANIMATION_COMPONENT_NAME);
            }

            animation.SetAnimations(_owner);
        }
    }

    public void ReturnModel()
    {
        _basePool?.Release(_transform);
    }

    private void SetModel(Transform prefab)
    {
        _basePool = ServiceLocator.GetService<PoolsContainer>().GetPool(prefab);
        _transform = (Transform)_basePool.Take();
        _transform.position = _owner.transform.position;
        _transform.transform.rotation = _owner.transform.rotation;
        _transform.parent = _owner.transform;
    }
}

public class VisualComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string VISUAL_COMPONENT_NAME = "Visual Component";

    [SerializeField]
    private Transform _visualPrefab;

    public VisualComponentCreator()
    {
        _name = VISUAL_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        VisualComponent visual = new VisualComponent(entity, _visualPrefab, _name);
        entity.Add(visual);
    }
}