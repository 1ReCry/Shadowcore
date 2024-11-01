using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrefab : MonoBehaviour
{
    public BoxCollider coll;
    public float damage = 25;
    public float destroyTimer;
    public float disableColliderTimer;
    public bool playerAttack;

    public bool isBullet;
    public bool destroyWhenCollide;
    public float bulletSpeed;

    void Start()
    {
        coll = GetComponent<BoxCollider>();
        coll.enabled = true;
        if(disableColliderTimer == 0) disableColliderTimer = destroyTimer;
    }

    void Update()
    {
        destroyTimer -= Time.deltaTime;
        disableColliderTimer -= Time.deltaTime;
        if(destroyTimer<=0)
        {
            Destroy(gameObject);
        }
        if(disableColliderTimer<=0 && coll != null)
        {
            coll.enabled = false;
        }
        if(isBullet)
        {
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.name.Contains("Player") && !playerAttack){
            if(destroyWhenCollide) gameObject.SetActive(false);
        }
        if(other.name.Contains("Enemy") && playerAttack){
            if(destroyWhenCollide) gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision other){
        if(other.collider.name.Contains("Player") && !playerAttack){
            if(destroyWhenCollide) gameObject.SetActive(false);
        }
        if(other.collider.name.Contains("Enemy") && playerAttack){
            if(destroyWhenCollide) gameObject.SetActive(false);
        }
    }
}
