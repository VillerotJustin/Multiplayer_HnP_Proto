using System.Collections.Generic;
using PurrNet.StateMachine;
using UnityEngine;

public class PlayerSpawningState : StateNode
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();


    public override void Enter()
    {
        base.Enter(asServer);

        if (!asServer) return;

        int currentSpawnIndex = 0;
        foreach (var player in NetworkManager.Instance.players)
        {
            var spawnPoint = spawnPoints[currentSpawnIndex];
            var newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            newPlayer.GiveOwnership(player);
            
            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
        }

        machine.Next();
    }

    public override void Exit()
    {
        base.Exit(asServer);

    }
}
