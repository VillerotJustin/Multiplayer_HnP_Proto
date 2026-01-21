using System.Collections.Generic;
using System.Linq;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

public class GameEndState : StateNode
{
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);
        
        if (!asServer)
            return;
        
        if (!InstanceHandler.TryGetInstance(out ScoreManager scoreManager))
        {
            Debug.LogWarning("No ScoreManager found");
            return;
        }
        
        var winner = scoreManager.GetWinner();
        if (winner == default)
        {
            Debug.LogWarning("No winner found");
            return;
        }
        Debug.Log($"Game Game has now Ended \n Winner: {winner}");  
    }
}
