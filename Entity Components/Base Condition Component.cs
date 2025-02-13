using System;

public class BaseConditionComponent: BaseComponent
{
    protected CompositeCondition _compositeConditions = new();

    public void AddCondition(Func<bool> condition)
    {
        _compositeConditions.AddCondition(condition);
    }

    public void RemoveCondition(Func<bool> condition)
    {
        _compositeConditions.RemoveCondition(condition);
    }
}
