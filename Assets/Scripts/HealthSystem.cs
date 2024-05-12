using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public string objectName;
    public int health;
    public int maxHealth = 100;

    private void Start()
    {
        health = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
       if(health <= 0)
        {
            Destroy(gameObject);
        } 
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
