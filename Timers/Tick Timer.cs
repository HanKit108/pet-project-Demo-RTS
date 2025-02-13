using System;

public class TickTimer : Timer
{
    private float _tickDelay;

    public TickTimer(float delay, Action action): base(delay, action)
    {
        _tickDelay = Math.Max(delay, 0);
        _delay = _tickDelay;
        _action = action;
    }

    public override bool Tick(float deltaTime)
    {
        if (!_isCompleted && !_isPaused)
        {
            _delay -= Math.Max(deltaTime, 0);
            if (_delay <= 0)
            {
                _delay = _tickDelay;
                _action();
            }
        }
        return _isCompleted;
    }
}