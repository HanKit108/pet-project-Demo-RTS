using System;
using UnityEngine;
using UnityEngine.UI;

public class PointsBarComponent : BaseComponent, 
    IComponent, IDisposable, ILateUpdatable
{
    private const float VERTICAL_OFFSET = 2f;

    [SerializeField, HideInInspector]
    private Image _imageBar;
    [SerializeField, HideInInspector]
    private Transform _transform, _canvas;
    [SerializeField, HideInInspector]
    private float _maxAmount;

    private Action<float> _onAmountChangedAction;
    private Action<float> _onMaxAmountChangedAction;
    private Action<Color> _onColorChangedAction;
    private Pool _barPool, _canvasPool;

    public PointsBarComponent(Transform owner)
    {
        _name = "Points Bar Component";

        _canvasPool = ServiceLocator.GetService<PoolsContainer>().GetCanvasPool();
        _canvas = (Transform)_canvasPool.Take();
        _canvas.position = owner.position + Vector3.up * VERTICAL_OFFSET;
        _canvas.parent = owner;

        _barPool = ServiceLocator.GetService<PoolsContainer>().GetBarPool();
        _transform = (Transform)_barPool.Take();
        _transform.position = _canvas.position;
        _transform.parent = _canvas;

        Image[] images = _transform.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.type == Image.Type.Filled)
            {
                _imageBar = image;
            }
        }
        Hide();
        ServiceLocator.GetService<UpdateManager>().Add(this);
    }

    public void SetAmountChangeEvents(
        ref Action<float> onAmountChangedAction,
        ref Action<float> onMaxAmountChangedAction,
        float maxAmount)
    {
        _maxAmount = maxAmount;

        onAmountChangedAction += OnAmountChanged;
        onMaxAmountChangedAction += OnMaxAmountChanged;

        _onAmountChangedAction = onAmountChangedAction;
        _onMaxAmountChangedAction = onMaxAmountChangedAction;
    }

    public void SetColorChangeEvent(ref Action<Color> onColorChangedAction)
    {
        onColorChangedAction += OnColorChanged;
        _onColorChangedAction = onColorChangedAction;
    }

    public void Dispose()
    {
        _onAmountChangedAction -= OnAmountChanged;
        _onMaxAmountChangedAction -= OnMaxAmountChanged;
        _onColorChangedAction -= OnColorChanged;
        _barPool?.Release(_transform);
        _canvasPool?.Release(_canvas);
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void OnLateUpdate(float deltaTime)
    {
        if (_transform != null)
        {
            _transform.rotation = ServiceLocator.GetService<CameraMoving>().CameraTransform.rotation;
        }
    }

    public void OnAmountChanged(float amount)
    {
        _imageBar.fillAmount = amount / _maxAmount;

        if (amount > 0 && amount < _maxAmount)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnMaxAmountChanged(float amount)
    {
        _maxAmount = amount;
    }

    private void Hide()
    {
        _transform.gameObject.SetActive(false);
    }

    private void Show()
    {
        _transform.gameObject.SetActive(true);
    }

    private void OnColorChanged(Color color)
    {
        _imageBar.color = color;
    }
}

public class PointsBarComponentCreator : BaseComponentCreator, IComponentCreator
{
    public PointsBarComponentCreator()
    {
        _name = "Points Bar Component";
    }

    public void CreateComponent(Entity entity)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            PointsBarComponent points = new PointsBarComponent(
                       entity.transform);
            points.SetAmountChangeEvents(
                ref health.OnHealthAmountChanged,
                ref health.OnMaxHealthAmountChanged,
                health.MaxHealthAmount);

            if (entity.TryGetComponent<TeamComponent>(out var team))
            {
                points.SetColorChangeEvent(ref team.OnTeamChanged);
            }
            entity.Add(points);
        }
    }
}