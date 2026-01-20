using System.Collections.Generic;
using System.Linq;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

public class GameEndState : StateNode<Dictionary<PlayerID, int>>
{
    public override void Enter(Dictionary<PlayerID, int> RoundWins, bool asServer)
    {
        base.Enter(asServer);
        
        if (!asServer)
            return;

        var winner = RoundWins.First();

        foreach (var player in RoundWins)
        {
            if (player.Value > winner.Value)
                winner = player;
        }
        
        
        Debug.Log($"Game Game has now Ended \n Winner: {winner}");  
    }
}
