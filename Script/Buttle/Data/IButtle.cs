using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public interface IButtle
{
    public void Init(Vector3 position, ObjectPool<GameObject> objectPool, Transform startTransfrom);
}
