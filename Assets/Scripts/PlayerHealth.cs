using System;
using PurrNet;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;

    public int Health => health.value;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursively(gameObject, actualLayer);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    [ServerRpc(requireOwnership:false)]
    public void ChangeHealth(int amount)
    {
        health.value += amount;
    }
}
