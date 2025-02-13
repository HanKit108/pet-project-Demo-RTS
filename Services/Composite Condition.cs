using System;
using System.Collections.Generic;

public class CompositeCondition
{
    private readonly List<Func<bool>> _conditions = new();

    public void AddCondition(Func<bool> condition)
    {
        _conditions.Add(condition);
    }

    public void RemoveCondition(Func<bool> condition)
    {
        _conditions.Remove(condition);
    }

    public bool Invoke()
    {
        foreach (var condition in _conditions)
        {
            var result = condition.Invoke();

            if (!result)
            {
                return false;
            }
        }
        return true;
    }
}
