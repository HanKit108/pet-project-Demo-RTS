using UnityEngine;

public class SelectionQuadController: IUpdatable, IDisable
{
    private PlayerControls _playerControls;
    private RectTransform _selectionAreaTransform;
    private Vector3 _startMousePosition;
    private Vector2 _p1, _p2;

    public SelectionQuadController(PlayerControls playerControls, RectTransform selectionAreaTransform)
    {
        _playerControls = playerControls;
        _selectionAreaTransform = selectionAreaTransform;

        _playerControls.Units.Select.started += context => StartSelect();
        _playerControls.Units.Select.canceled += context => CancelSelect();

        ServiceLocator.GetService<UpdateManager>().Add(this);
        ServiceLocator.GetService<DisableManager>().Add(this);
    }
    public void Disable()
    {
        _playerControls.Units.Select.started -= context => StartSelect();
        _playerControls.Units.Select.canceled -= context => CancelSelect();
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void OnUpdate(float deltaTime)
    {
        if (_playerControls.Units.Select.IsPressed())
        {
            PerformSelect();
        }
    }

    private void StartSelect()
    {
        _startMousePosition = Input.mousePosition;
        _selectionAreaTransform.gameObject.SetActive(true);
    }

    private void PerformSelect()
    {
        _p1 = _startMousePosition;
        _p2 = Input.mousePosition;

        float w = _p2.x - _p1.x;
        float h = _p2.y - _p1.y;
        _selectionAreaTransform.anchoredPosition = _p1 + new Vector2(w / 2, h / 2);
        _selectionAreaTransform.sizeDelta = new Vector2
        (
            Mathf.Abs(w),
            Mathf.Abs(Mathf.Abs(h))
        );
    }

    private void CancelSelect()
    {
        _selectionAreaTransform.gameObject.SetActive(false);
    }
}
