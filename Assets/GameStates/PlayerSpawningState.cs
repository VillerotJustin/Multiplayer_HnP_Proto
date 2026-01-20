using System.Collections.Generic;
using PurrNet.StateMachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawningState : StateNode
{
    [SerializeField] private PlayerHealth playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();
    
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        Debug.Log($"[PlayerSpawningState] Enter called - asServer: {asServer}");
        if (!asServer)
        {
            Debug.Log("[PlayerSpawningState] Skipping - not server");
            return;
        }

        var spawnedPlayers = new List<PlayerHealth>();
        
        Debug.Log($"[PlayerSpawningState] Spawning {networkManager.players.Count} players");
        int currentSpawnIndex = 0;
        foreach (var player in networkManager.players)
        {
            var spawnPoint = spawnPoints[currentSpawnIndex];
            var newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            newPlayer.GiveOwnership(player);
            spawnedPlayers.Add(newPlayer);
            
            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
        }
        
        Debug.Log("[PlayerSpawningState] Players spawned, calling machine.Next()");

        machine.Next(spawnedPlayers);
    }

    public override void Exit(bool asServer)
    {
        base.Exit(asServer);
        
        //Debug.Log($"[PlayerSpawningState] Exit called - asServer: {asServer}");
    }
}
