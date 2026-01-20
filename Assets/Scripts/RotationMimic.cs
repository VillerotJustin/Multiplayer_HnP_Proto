using System;
using PurrNet;
using UnityEngine;

public class RotationMimic : NetworkBehaviour
{
    [SerializeField] private Transform targetTransform;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetTransform) return;

        transform.rotation = targetTransform.rotation;
    }
}
