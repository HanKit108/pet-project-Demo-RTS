using System;
using System.Collections.Generic;

public class TimerSystem: IUpdatable
{
    private List<Timer> _timers = new();
    private List<TickTimer> _tickTimers = new();

    public TimerSystem()
    {
        ServiceLocator.GetService<UpdateManager>().Add(this);
    }

    public void OnUpdate(float deltaTime)
    {
        int i =  0;
        while (i < _timers.Count)
        {
            if (_timers[i].Tick(deltaTime))
            {
                _timers.Remove(_timers[i]);
            }
            i++;
        }

        int j = 0;
        while (j < _tickTimers.Count)
        {
            if (_tickTimers[j].Tick(deltaTime))
            {
                _tickTimers.Remove(_tickTimers[j]);
            }
            j++;
        }
    }

    public Timer CreateTimer(float cooldown, Action action)
    {
        Timer timer = new Timer(cooldown, action);
        _timers.Add(timer);
        return timer;
    }

    public TickTimer CreateTickTimer(float tickDelay, Action action)
    {
        TickTimer timer = new TickTimer(tickDelay, action);
        _tickTimers.Add(timer);
        return timer;
    }
}
