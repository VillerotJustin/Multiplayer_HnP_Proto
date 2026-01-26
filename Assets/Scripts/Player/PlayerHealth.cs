using System;
using PurrNet;
using UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;

    public Action<PlayerID> OnDeath_server;
        
    public int Health => health.value;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        var actualLayer = isOwner ? selfLayer : otherLayer;
        SetLayerRecursively(gameObject, actualLayer);

        if (isOwner)
        {
            InstanceHandler.GetInstance<MainGameView>().UpdateHealthDisplay(Health);
            health.onChanged += OnHealthChanged;
        }
    }

    protected override void OnDestroy()
    {
        if (isOwner)
        {
            health.onChanged -= OnHealthChanged;
        }
        
        // Clear all subscribers from the death event
        OnDeath_server = null;
    }

    /**
     * @brief Updates the health display on the UI when the player's health changes
     * 
     * This callback is triggered when the health SyncVar changes. It retrieves the
     * MainGameView instance and updates the health display with the new value.
     * Only executed on the owner's client.
     * 
     * @param newValue The new health value to display on the UI
     * 
     * @return void
     */
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
    public void ChangeHealth(int amount, RPCInfo info = default)
    {
        health.value += amount;

        if (health.value <= 0)
        {
            if (InstanceHandler.TryGetInstance(out ScoreManager scoreManager))
            {
                scoreManager.AddKill(info.sender);
                if (owner.HasValue)
                    scoreManager.AddDeath(owner.Value);
            }
            Debug.Log($"Player {owner?.id} has died.");
            OnDeath_server?.Invoke(owner.Value);
            Destroy(gameObject);
        }
    }
}
