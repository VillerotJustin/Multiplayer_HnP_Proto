using System.Collections;
using PurrNet.StateMachine;
using UnityEngine;

public class WaitForPlayerState : StateNode
{
    [SerializeField] private int minPlayers = 2;
    
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        
        StartCoroutine(WaitForPlayers());
    }

    private IEnumerator WaitForPlayers()
    {
        while (networkManager?.playerCount < minPlayers)
            yield return null;
        
        machine.Next();
    }
}
