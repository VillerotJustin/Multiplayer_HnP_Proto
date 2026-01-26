using System.Collections.Generic;
using System.Linq;
using PurrNet;
using PurrNet.StateMachine;
using UI;
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

        if (!InstanceHandler.TryGetInstance(out EndGameView _endGameView))
        {
            Debug.LogWarning("No EndGameView found");
            return;
        }
        
        if (!InstanceHandler.TryGetInstance(out GameViewManager _gameViewManager))
        {
            Debug.LogWarning("No GameViewManager found");
            return;
        }
        
        _gameViewManager.ShowView<EndGameView>();
        
        _endGameView.SetWinner(winner);
    }
}
