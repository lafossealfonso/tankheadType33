using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Mathematics;
using UnityEngine;

public class RailTank : MonoBehaviour
{
    public static RailTank Instance { get; private set; }

    private float targetRotation;
    private float currentRotation;
    [SerializeField] private float gunRotateSpeed;

    [SerializeField] private Transform playerTransform;

    [SerializeField]private float movementSpeed = 2f;
    [SerializeField] private Transform topTankTransform;
    [SerializeField] private Transform shootPointA;
    [SerializeField] private Transform shootPointB;
    [SerializeField] private WeaponScriptableObject railWeapon;


    private Transform lastPlayerTransform;

    private HealthSystem currentHealthSystem;

    private bool isMoving;
    [SerializeField] private Transform particleGroup; 

    [SerializeField] private LayerMask hittableLayerMask;
    private bool shootPointBool;
    private Transform currentShootPoint;
    

    public bool playerInProximity = false;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one WeaponManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        targetRotation = currentRotation;
        GameObject gameObject = GameObject.FindWithTag("Player");
        playerTransform = gameObject.transform;
        InvokeRepeating("UpdatePlayerTransform", 0f, 1.5f);
        InvokeRepeating("ShootRailTank", 0f, 0.5f);
        
        
    }

    private void Update()
    {
        RotateGunToTarget();
        

        transform.eulerAngles = new Vector3(transform.rotation.x, currentRotation, transform.rotation.z);

        if(Mathf.Abs(targetRotation - currentRotation) > 2f)
        {
            isMoving = true;
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, movementSpeed * Time.deltaTime);
        }

        else
        {
            isMoving = false;
        }

        particleGroup.gameObject.SetActive(isMoving);

        
    }

    public void MoveToNewPosition()
    {
        int firstPass = Random.Range(0, 360);
        if(Mathf.Abs(firstPass - currentRotation) <= 40)
        {
            MoveToNewPosition();
        }

        else
        {
            targetRotation = firstPass;
        }
    }

    public void RotateGunToTarget()
    {
        Vector3 directionToTarget = playerTransform.position - topTankTransform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        topTankTransform.rotation = Quaternion.RotateTowards(topTankTransform.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
    }

    private void UpdatePlayerTransform()
    {
        lastPlayerTransform = playerTransform;
    }

    public void ShootRailTank()
    {
        if(shootPointBool ==  false)
        {
            currentShootPoint = shootPointA;
        }
        else if(shootPointBool == true)
        {
            currentShootPoint = shootPointB;
        }

        Transform bulletPrefab = Instantiate(railWeapon.bulletPrefab, currentShootPoint);
        BulletPrefabScript bulletPrefabScript = bulletPrefab.GetComponent<BulletPrefabScript>();
        bulletPrefabScript.isEnemy = true;

        Vector3 shootDir = (lastPlayerTransform.position - currentShootPoint.position).normalized;

        if(Physics.Raycast(currentShootPoint.position, shootDir, out RaycastHit hit, railWeapon.range, hittableLayerMask))
        {
            Vector3 shootTargetPosition = hit.point;
            GameObject hitObject = hit.collider.gameObject;

            if(hitObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
            {
                currentHealthSystem = healthSystem;
                bulletPrefabScript.Setup(lastPlayerTransform.position);
            }

            else
            {
                Vector3 shootTargetVector = lastPlayerTransform.position;
                bulletPrefabScript.Setup(shootTargetVector);
            }
        }

        shootPointBool = !shootPointBool;

    }

    public void DealEnemyDamage()
    {
        if (currentHealthSystem != null)
        {
            currentHealthSystem.TakeDamage(WeaponManager.Instance.CalculateDamage(railWeapon.damageMin, railWeapon.damageMax));
        }

    }

}
