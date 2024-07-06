using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 2f;
    public float knockbackForce = 5f;

    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Rigidbody>() != null)
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            other.gameObject.GetComponent<Rigidbody>().AddForce(-knockbackDirection * knockbackForce, ForceMode.Impulse);
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 bulletDirection)
    {
        rb.velocity = bulletDirection * speed;
    }
}
