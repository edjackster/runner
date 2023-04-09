using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void Start()
    {
        InitializeServices();

        //GameStateMachine game = new GameStateMachine();
        //game.SwitchState<BootstrapperState>();
    }

    private void InitializeServices()
    {
        AllServices.Instance.RegisterService(new InputService());
        AllServices.Instance.RegisterService(new CoinService());
    }
}
