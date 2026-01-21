using System;
using PurrNet;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float range = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private bool autoFire = true;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    private float _lastFireTime;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void Update()
    {
        if ((autoFire && !Input.GetKey(KeyCode.Mouse0)) || (!autoFire && !Input.GetKeyDown(KeyCode.Mouse0)) )
            return;
        
        if (_lastFireTime + fireRate > Time.unscaledTime)
            return;

        PlayShotEffect();
        _lastFireTime = Time.unscaledTime;
        
        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo, range, hitLayer))
            return;
        
        // Debug.Log($"Hit {hitInfo.transform.name}");

        if (!hitInfo.transform.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            return;
        
        playerHealth.ChangeHealth(-damage);
    }
    
    [ObserversRpc(runLocally: true)]
    private void PlayShotEffect()
    {
        if (muzzleFlash)
            muzzleFlash.Play();
    }
}
