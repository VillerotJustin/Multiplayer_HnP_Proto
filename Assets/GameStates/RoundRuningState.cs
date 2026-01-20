using System.Collections.Generic;
using PurrNet;
using PurrNet.StateMachine;
using UnityEngine;

public class RoundRuningState : StateNode<List<PlayerHealth>>
{
    private List<PlayerID> _players = new();
    
    public override void Enter(List<PlayerHealth> players, bool asServer)
    {
        base.Enter(players, asServer);
        
        if(!asServer)
            return;
        
        _players.Clear();
        foreach (var player in players)
        {
            if (player.owner.HasValue)
                _players.Add(player.owner.Value);
            player.OnDeath_server += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath(PlayerID deadPlayer)
    {
        _players.Remove(deadPlayer);

        if (_players.Count <= 1)
        {
            Debug.Log($"Someone won the round!");
            if (_players.Count == 1)
                machine.Next(_players[0]);
            else
                machine.Next();
            
            
        }
    }
}
