using System;

[Serializable]
public class Effect
{
    private int _id;
    private Timer _timer;
    private Action _dispelAction;
    public Action OnCompleted;

    public int Id => _id;

    public Effect(int id, Action dispelAction, float duration)
    {
        _id = id;
        _dispelAction = dispelAction;
        _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(duration, () => OnComplete());
    }

    public void Refresh(float duration)
    {
        _timer.Abort();
        _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(duration, () => OnComplete());
    }

    public void Dispel()
    {
        _dispelAction();
        _timer.Abort();
    }

    private void OnComplete()
    {
        OnCompleted?.Invoke();
    }
}
