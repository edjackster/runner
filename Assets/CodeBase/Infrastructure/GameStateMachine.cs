using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : IStateSwitcher
{
    private readonly List<IState> _states;
    private IState _currentState;

    public GameStateMachine()
    {
        _states = new List<IState>();

    }

    public void SwitchState<TState>()
        where TState : IState
    {
        _currentState?.Exit();
        _currentState = _states.FirstOrDefault(state => state is TState);
        _currentState.Enter();
    }
}
