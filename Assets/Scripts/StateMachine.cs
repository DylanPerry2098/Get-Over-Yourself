using System;
using UnityEngine;

public class StateMachine: MonoBehaviour
{
    [HideInInspector] public State currentState;

    public void Initialize(State startingState)
    {
        currentState = startingState;
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }

    private void Update()
    {
        currentState.StateUpdate();
    }

    private void FixedUpdate()
    {
        currentState.StateFixedUpdate();
    }
}
