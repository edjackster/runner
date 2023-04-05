using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateSwitcher
{
    public void SwitchState<TState>() where TState : IState;
}
