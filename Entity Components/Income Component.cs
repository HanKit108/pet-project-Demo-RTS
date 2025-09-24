using System;
using UnityEngine;

[Serializable]
public class IncomeComponent : BaseConditionComponent, 
    IComponent, ISwitchable, IEnable, IDisable, IDisposable
{
    [SerializeField, Min(0)]
    private int _incomeAmount;
    [SerializeField, Min(0)]
    private float _incomeDelay;

    private TickTimer _timer;
    private Action _enableAction, _disableAction;

    public IncomeComponent(int incomeAmount, float incomeDelay, string name)
    {
        _name = name;
        _incomeAmount = incomeAmount;
        _incomeDelay = incomeDelay;
        _timer = ServiceLocator.GetService<TimerSystem>().CreateTickTimer(_incomeDelay, EarnResourse);
    }

    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += Enable;
        disableAction += Disable;

        _enableAction = enableAction;
        _disableAction = disableAction;
    }

    public void Enable()
    {
        _timer?.Resume();
    }

    public void Disable()
    {
        _timer?.Pause();
    }

    public void Dispose()
    {
        _enableAction -= Enable;
        _disableAction -= Disable;
        _timer?.Abort();
    }

    private void EarnResourse()
    {
        ServiceLocator.GetService<PlayerManager>().ResourseBank.AddResourse(_incomeAmount);
    }
}

public class IncomeComponentCreator : BaseComponentCreator, IComponentCreator
{
    private const string INCOME_COMPONENT_NAME = "Income Component";

    [SerializeField, Min(0)]
    private int _incomeAmount;
    [SerializeField, Min(0)]
    private float _incomeDelay;

    public IncomeComponentCreator()
    {
        _name = INCOME_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        IncomeComponent income = new IncomeComponent(_incomeAmount, _incomeDelay, _name);
        TrySetLifeEvents(entity, income);
        entity.Add(income);
    }
}