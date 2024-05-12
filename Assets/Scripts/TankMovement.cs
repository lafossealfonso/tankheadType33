using System;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    

    [SerializeField] private float lookRotationSpeed = 1f;
    [SerializeField] private float tankRotationSpeed = 5f;
    [SerializeField] private float tankMoveSpeed = 100f;
    [SerializeField] private float gunRotateSpeed = 5f;

    [SerializeField]private Transform lookAtParent; 
    [SerializeField]private Transform tankTopParent;

    private Transform lookAtChild;
    private Vector3 initialOffset;

    private void Awake()
    {
        WeaponManager.Instance.OnShootEvent += Shoot;
    }
    private void Start()
    {
        initialOffset = tankTopParent.position - transform.position;
        lookAtChild = lookAtParent.GetChild(0).transform;
    }

    private void Update()
    {
        MoveTank();
        AimGun();
        RotateTank();
        RotateLookAt();
        
        
    }

    private void RotateLookAt()
    {
        Vector3 tankTopPos = transform.position + initialOffset;

        lookAtParent.position = tankTopPos;
        tankTopParent.position = tankTopPos;


        float mouseX = Input.GetAxis("Mouse X");
        lookAtParent.Rotate(Vector3.up, mouseX * lookRotationSpeed);
    }

    private void AimGun()
    {
        Vector3 directionToTarget = lookAtChild.position - tankTopParent.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        tankTopParent.rotation = Quaternion.RotateTowards(tankTopParent.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
        // Check if the rotation is necessary
        if (Quaternion.Angle(tankTopParent.rotation, targetRotation) > 1f) // Adjust the threshold as needed
        {
            WeaponManager.Instance.currentlyRotating = true;
        }
        else
        {
            WeaponManager.Instance.currentlyRotating = false;
        }
    }

    private void MoveTank()
    {
        float accelerationInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * accelerationInput * tankMoveSpeed * Time.deltaTime);
    }

    private void RotateTank()
    {
        float rotationInput = Input.GetAxis("Horizontal");
        float rotationY = rotationInput * tankRotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationY);
    }

    private void Shoot(object sender, EventArgs e)
    {
        float shootRange = WeaponManager.Instance.currentWeapon.range;
        Vector3 newPosition = lookAtParent.position + (lookAtParent.forward * shootRange);
        lookAtChild.position = newPosition;
        Debug.Log(shootRange);
    }
}
