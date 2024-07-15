using System;
using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject serverProjectilePrefab;

    [SerializeField] private CoinWallet wallet;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private InputReader inputReader;

    [Header("Settings")] [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireRate = 0.75f;
    [SerializeField] private float muzzleFlashDuration = 0.0075f;
    [SerializeField] private int costFire;

    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;

    void Update()
    {
        if (muzzleFlashTimer > 0)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner)
            return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
            
        if (!shouldFire)
            return;
        
        if (timer > 0)
            return;

        if (wallet.TotalCoins.Value < costFire)
        {
            return;
        }
        
        PrimaryFireServerRPC(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        timer = 1 / fireRate;
        wallet.SpendCoins(costFire);
        
        
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPoint, Vector3 direction)
    {
        if (wallet.TotalCoins.Value < costFire)
        {
            return;
        }
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPoint, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage dd))
        {
            dd.SetOwner(OwnerClientId);
            
        }
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
        SpawnDummyProjectileClientRPC(spawnPoint, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 spawnPoint, Vector3 direction)
    {
        if (IsOwner)
            return;
        SpawnDummyProjectile(spawnPoint, direction);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public void OnEnable()
    {
        if (!IsOwner)
            return;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool isFiring)
    {
        this.shouldFire = isFiring;
    }

    private void SpawnDummyProjectile(Vector3 spawnPoint, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPoint, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage dd))
        {
            dd.SetOwner(OwnerClientId);
        }
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = projectileInstance.transform.up * projectileSpeed;
        }
    }
}