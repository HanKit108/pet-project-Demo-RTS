using System;
using UnityEngine;

[Serializable]
public class ResourseBank
{
    private int _resourseAmount;
    public int ResourseAmount => _resourseAmount;
    public Action<int> OnAmountChanged;

    public void AddResourse(int amount)
    {
        _resourseAmount += Mathf.Max(0, amount);
        OnAmountChanged?.Invoke(_resourseAmount);
    }

    public bool TrySpendResourse(int amount)
    {
        if (_resourseAmount >= amount)
        {
            _resourseAmount -= amount;
            OnAmountChanged?.Invoke(_resourseAmount);
            return true;
        }
        return false;
    }
}
