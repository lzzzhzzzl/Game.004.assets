using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class Buttle : MonoBehaviour, IButtle
{
    public float speed;
    public float buttleDamage;
    public Vector3 buttleDirection;
    public Transform attackerTtansfrom;

    private ObjectPool<GameObject> buttleObject;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

    public void Init(Vector3 targetPosition, ObjectPool<GameObject> objectPool, Transform startTransfrom)
    {
        rb.velocity = Vector2.zero;
        attackerTtansfrom = startTransfrom;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
        transform.rotation = rotation;

        buttleDirection = direction;
        rb.velocity = direction * speed;
        buttleObject = objectPool;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerParameter>())
        {
            other.GetComponent<PlayerParameter>().HurtState(buttleDamage, buttleDirection);
            PoolManager.Instance.ReleaseButtle(buttleObject, this.gameObject);
        }
    }
}
