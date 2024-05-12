using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    private bool readyToGo = false;
    private Vector3 targetPosition;
    [SerializeField] float moveSpeed;
    [SerializeField] Transform explosionVFX;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        readyToGo = true;
    }

    private void Update()
    {

        if (readyToGo)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

            if(distanceBeforeMoving < distanceAfterMoving)
            {
                Instantiate(explosionVFX, transform.position, Quaternion.identity);
                WeaponManager.Instance.DealDamage();
                WeaponManager.Instance.UpdateUiEnemyStatusValues();
                Destroy(gameObject);
            }
        }
    }
}
