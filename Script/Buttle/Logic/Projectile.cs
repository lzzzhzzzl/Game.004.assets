using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour, IButtle
{
    public float height;
    public float speed;
    public float radius;
    public float damage;
    public Sprite startSprite;
    public Sprite landingSprite;
    public Transform attackerTransfrom;

    private float distance;
    private float currentDistance;
    private Vector3 startPosition;
    private Vector3 centerPosition;
    private Vector3 targetPosition;
    private Vector3 direction;
    private Transform spriteTransfrom;
    private Transform shadowTransfrom;
    private ObjectPool<GameObject> buttleObject;
    private bool isHit;
    private void Awake()
    {
        spriteTransfrom = transform.GetChild(0);
        shadowTransfrom = transform.GetChild(1);
    }

    public void Init(Vector3 target, ObjectPool<GameObject> objectPool, Transform startTransfrom)
    {
        isHit = false;
        spriteTransfrom.GetComponent<SpriteRenderer>().enabled = true;
        shadowTransfrom.GetComponent<SpriteRenderer>().enabled = true;
        spriteTransfrom.GetComponent<SpriteRenderer>().sprite = startSprite;
        shadowTransfrom.GetComponent<SpriteRenderer>().sprite = startSprite;
        attackerTransfrom = startTransfrom;
        float k = (target.x - transform.position.x) != 0 ? (target.y - transform.position.y) / (target.x - transform.position.x) : float.MaxValue;
        distance = Vector3.Distance(target, transform.position);
        float r = distance / 2f;
        float halfx = Mathf.Sqrt(r * r / (1 + 4 * k * k));
        Vector3 halfPosition = Vector3.Distance(new Vector3(halfx, k * halfx, 0), target) > Vector3.Distance(new Vector3(-halfx, -k * halfx, 0), target) ?
                            new Vector3(-halfx, -k * halfx + height, 0) + transform.position : new Vector3(halfx, k * halfx + height, 0) + transform.position;

        currentDistance = 0;
        buttleObject = objectPool;
        startPosition = transform.position;
        centerPosition = halfPosition;
        targetPosition = target;
        direction = target - transform.position;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
        shadowTransfrom.rotation = rotation;
        shadowTransfrom.position = transform.position;
    }
    private Vector3 GetBezierPoint(float t)
    {
        return (1 - t) * (1 - t) * startPosition + 2 * t * (1 - t) * centerPosition + t * t * targetPosition;
    }
    private Vector3 GetBezierTangentLine(float t)
    {
        return 2 * (t - 1) * startPosition + (2 - 4 * t) * centerPosition + 2 * t * targetPosition;
    }
    private void Update()
    {
        Bounce();
    }
    public void Bounce()
    {
        if (distance - currentDistance > .5f)
        {
            currentDistance += speed;
            spriteTransfrom.position = GetBezierPoint(currentDistance / distance);
            spriteTransfrom.rotation = Quaternion.FromToRotation(Vector3.right, GetBezierTangentLine(currentDistance / distance));
            shadowTransfrom.position += direction * (speed / distance);
        }
        else
        {
            spriteTransfrom.position = targetPosition;
            shadowTransfrom.position = targetPosition;
            if (!isHit)
            {
                Collider2D[] colliders = new Collider2D[20];
                int count = Physics2D.OverlapCircleNonAlloc(targetPosition, radius, colliders);
                for (int i = 0; i < count; i++)
                {
                    if (colliders[i].GetComponent<EnemyBaseController>())
                    {
                        colliders[i].GetComponent<EnemyBaseController>().HurtState(damage, attackerTransfrom);
                        PoolManager.Instance.ReleaseButtle(buttleObject, this.gameObject);
                        spriteTransfrom.GetComponent<SpriteRenderer>().enabled = false;
                        shadowTransfrom.GetComponent<SpriteRenderer>().enabled = false;
                        isHit = true;
                    }
                }
            }
            if (landingSprite != null)
            {
                spriteTransfrom.GetComponent<SpriteRenderer>().sprite = landingSprite;
                shadowTransfrom.GetComponent<SpriteRenderer>().sprite = landingSprite;
            }
        }
    }
}
