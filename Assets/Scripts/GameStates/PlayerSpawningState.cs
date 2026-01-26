using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine;

public class PlayerSpawningState : StateNode
{
    [SerializeField] private PlayerHealth playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();
    
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
            return;

        DespawnPlayers();

        var spawnedPlayers = SpawnPlayers();

        machine.Next(spawnedPlayers);
    }
    
    private List<PlayerHealth> SpawnPlayers()
    {
        var spawnedPlayers = new List<PlayerHealth>();
        
        int currentSpawnIndex = 0;
        foreach (var player in networkManager.players)
        {
            var spawnPoint = spawnPoints[currentSpawnIndex];
            var newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            newPlayer.GiveOwnership(player);
            spawnedPlayers.Add(newPlayer);
            
            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
        }

        return spawnedPlayers;
    }
    
    private void DespawnPlayers()
    {
        var allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var player in allPlayers)
        {
            Destroy(player.gameObject);
        }
    }

}
