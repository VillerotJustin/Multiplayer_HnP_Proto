using System;
using PurrNet;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;

    public Action<PlayerHealth> OnDeath_server;
        
    public int Health => health.value;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursively(gameObject, actualLayer);

        if (isOwner)
        {
            health.onChanged += OnHealthChanged;
        }
    }

    protected override void OnDestroy()
    {
        if (isOwner)
        {
            health.onChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(int newValue)
    {
        var mainGameView = InstanceHandler.GetInstance<MainGameView>();
        if (mainGameView != null)
        {
            mainGameView.UpdateHealthDisplay(newValue);
        }
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

        if (health.value <= 0)
        {
            // Handle player death (e.g., respawn, game over, etc.)
            Debug.Log($"Player {owner?.id} has died.");
            OnDeath_server?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
