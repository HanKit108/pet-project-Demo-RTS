using System;

public class Timer
{
    protected Action _action;
    protected float _delay;
    protected bool _isCompleted, _isPaused;
    private Action action;

    public Timer(float delay, Action action)
    {
        _delay = Math.Max(delay, 0);
        _action = action;
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    public void Abort()
    {
        _isCompleted = true;
    }

    public virtual bool Tick(float deltaTime)
    {
        if (!_isCompleted && !_isPaused)
        {
            _delay -= Math.Max(deltaTime, 0);
            if (_delay <= 0)
            {
                _isCompleted = true;
                _action();
            }
        }
        return _isCompleted;
    }
}