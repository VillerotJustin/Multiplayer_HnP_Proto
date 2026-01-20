using PurrNet.StateMachine;
using UnityEngine;

public class GameEndState : StateNode
{
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        
        if (!asServer)
            return;
        
        Debug.Log("Game Game has now Ended");  
    }
}
