using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string 
        UNIT_LAYER_NAME = "Unit", 
        UNSELECTABLE_LAYER_NAME = "Unselectable", 
        GROUND_LAYER_NAME = "Ground";


    public int UnitLayer { get; private set; }
    public int UnselectableLayer { get; private set; }
    public int GroundLayer { get; private set; }

    [Header("Game Settings")]
    [SerializeField]
    private Transform _selectionCirclePrefab;
    [SerializeField]
    private Transform _pointsBarPrefab;
    [SerializeField]
    private Transform _canvasPrefab;
    [SerializeField]
    private Transform _colliderPrefab;
    [SerializeField]
    private Button _buttonPrefab;
    [SerializeField]
    private Entity _entityPrefab;
    [SerializeField]
    private Transform _entitiesContainer;
    [SerializeField]
    private Transform _poolTransform;
    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private float _disposeDelay = 5f;
    [SerializeField]
    private RectTransform _selectionAreaTransform;
    [SerializeField]
    private float _distanceBetweenUnits;

    [Header("Camera Settings")]
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    private float _moveSpeed, _minYPosition, _minXRotation, _maxXRotation, _smooth;

    [Header("UI Settings")]
    [SerializeField]
    private TextMeshProUGUI _resourseDisplayText;
    [SerializeField]
    private Transform _buttonsPanel, _queuePanel;

    [Header("Player Settings")]
    [SerializeField]
    private int _startResourse;
    [SerializeField]
    private TeamSO _team;

    [Header("Enemy Settings")]
    [SerializeField]
    private float _tickDelay;
    [SerializeField]
    private Transform _targetTransform;
    
    void Awake()
    {
        UnitLayer = LayerMask.NameToLayer(UNIT_LAYER_NAME);
        UnselectableLayer = LayerMask.NameToLayer(UNSELECTABLE_LAYER_NAME);
        GroundLayer = (int) Mathf.Pow(2, (float) LayerMask.NameToLayer(GROUND_LAYER_NAME));
        

        ServiceLocator.AddService(typeof(GameManager), this);
        ServiceLocator.AddService(typeof(DisableManager), new DisableManager());
        ServiceLocator.AddService(typeof(EntitySpawner), new EntitySpawner());
        ServiceLocator.AddService(typeof(EntityConstructor), new EntityConstructor());
        ServiceLocator.AddService(typeof(EntityTeamInstaller), new EntityTeamInstaller());
        ServiceLocator.AddService(typeof(EntityDisposer), new EntityDisposer(_disposeDelay));
        ServiceLocator.AddService(typeof(DealDamageController), new DealDamageController());
        ServiceLocator.AddService(typeof(RangeAttackController), new RangeAttackController());
        ServiceLocator.AddService(typeof(UpdateManager), new UpdateManager());
        ServiceLocator.AddService(typeof(PlayerControlsManager), new PlayerControlsManager());
        var playerControls = ServiceLocator.GetService<PlayerControlsManager>().PlayerControls;
        ServiceLocator.AddService(typeof(CameraMoving), new CameraMoving(_cameraTransform, _moveSpeed, _minYPosition, _minXRotation, _maxXRotation, _smooth, playerControls));
        ServiceLocator.AddService(typeof(TimerSystem), new TimerSystem());
        ServiceLocator.AddService(typeof(SpawnerManager), new SpawnerManager(_targetTransform, _tickDelay));
        ServiceLocator.AddService(typeof(PlayerManager), new PlayerManager(_team, _startResourse));
        ServiceLocator.AddService(typeof(ResourseDisplay), new ResourseDisplay(_resourseDisplayText));
        ServiceLocator.AddService(typeof(PlayerPanel), new PlayerPanel(_buttonsPanel, _queuePanel));
        ServiceLocator.AddService(typeof(PoolManager), new PoolManager());
        ServiceLocator.AddService(typeof(PoolsContainer), new PoolsContainer(
            _selectionCirclePrefab,
            _pointsBarPrefab,
            _canvasPrefab,
            _colliderPrefab,
            _buttonPrefab,
            _entityPrefab,
            _poolTransform,
            _entitiesContainer));
        ServiceLocator.AddService(typeof(SquadControl), new SquadControl(
            playerControls,
            _distanceBetweenUnits));
        ServiceLocator.AddService(typeof(SelectionQuadController), new SelectionQuadController(
            playerControls,
            _selectionAreaTransform));

        for (int i = 0; i < _entitiesContainer.childCount; i++)
        {
            if (_entitiesContainer.GetChild(i).TryGetComponent<EntityWorldConstructor>(out var other))
            {
                other.Construct();
            }
        }

        _quitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void OnDisable()
    {
        ServiceLocator.GetService<DisableManager>().DisableAll();
    }

    private void Update()
    {
        ServiceLocator.GetService<UpdateManager>().Update();
    }

    private void LateUpdate()
    {
        ServiceLocator.GetService<UpdateManager>().LateUpdate();
    }
}
