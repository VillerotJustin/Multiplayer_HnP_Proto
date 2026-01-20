using System.Collections;
using PurrNet.StateMachine;
using UnityEngine;
using UnityEngine.Serialization;

public class RoundEndState : StateNode
{
    [SerializeField] private int ammountOfRounds = 3;
    [SerializeField] private StateNode spawningState;

    private int _roundCount;
    private WaitForSeconds _delay = new(3f); // Time to wait

    public override void Enter()
    {
        base.Enter();
        
        _roundCount++;
        
        StartCoroutine(DelayNextState());
    }

    private IEnumerator DelayNextState()
    {
        yield return _delay;
        machine.SetState(spawningState);
    }
}
