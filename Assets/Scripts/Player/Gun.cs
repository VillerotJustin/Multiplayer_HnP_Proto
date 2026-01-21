using System;
using System.Collections;
using PurrNet;
using UnityEngine;

public class Gun : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] private float range = 20f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private bool autoFire = true;
    
    [Header("Recoil")]
    [SerializeField] private float recoilStrength = 1f;
    [SerializeField] private float recoilDuration = 0.2f;
    [SerializeField] private AnimationCurve recoilCurve;
    [SerializeField] private float rotationAmount = 25f;
    [SerializeField] private AnimationCurve rotationCurve;
    
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform HandTarget;
    [SerializeField] private Transform HandIKTarget;
    
    private float _lastFireTime;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Coroutine _recoilCoroutine;

    private void Start()
    {
        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;
        Debug.Log(_originalPosition);
        Debug.Log(_originalRotation);
    }

    private void Update()
    {
        SetIKTarkets();
        
        if (!isOwner)
            return;
        
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

    private void SetIKTarkets()
    {
        HandIKTarget.SetPositionAndRotation(HandTarget.position, HandTarget.rotation);
    }

    [ObserversRpc(runLocally: true)]
    private void PlayShotEffect()
    {
        if (muzzleFlash)
            muzzleFlash.Play();
        
        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);
        
        _recoilCoroutine = StartCoroutine(PlayRecoil());
    }
    
    private IEnumerator PlayRecoil()
    {
        float elasped = 0f;

        while (elasped < recoilDuration)
        {
            elasped += Time.deltaTime;
            float curveTime = elasped / recoilDuration;
            
            // Position recoil
            float recoilValue = recoilCurve.Evaluate(curveTime);
            Vector3 recoilOffset = Vector3.back * (recoilValue * recoilStrength);
            transform.localPosition = _originalPosition + recoilOffset;
            
            // Rotation recoil
            float rotationValue = rotationCurve.Evaluate(curveTime);
            Vector3 rotationOffset = new Vector3(rotationValue * rotationAmount, 0f, 0f);
            transform.localRotation = _originalRotation * Quaternion.Euler(rotationOffset);

            yield return null;
        }
        
        transform.localPosition = _originalPosition;
        transform.localRotation = _originalRotation;
    }

}
