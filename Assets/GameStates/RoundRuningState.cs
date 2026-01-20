using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine;

public class RoundRuningState : StateNode<List<PlayerHealth>>
{
    private int _playersAlive;
    
    public override void Enter(List<PlayerHealth> players, bool asServer)
    {
        base.Enter(players, asServer);
        
        if(!asServer)
            return;
        
        _playersAlive = players.Count;
        foreach (var player in players)
        {
            player.OnDeath_server += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath(PlayerHealth deadPlayer)
    {
        deadPlayer.OnDeath_server -= OnPlayerDeath;
        _playersAlive--;

        if (_playersAlive <= 1)
        {
            Debug.Log($"Someone won the round!");
        }
    }
}
