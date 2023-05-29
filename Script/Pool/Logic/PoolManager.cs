using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine.Audio;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [Header("粒子特效列表")]
    public PoolObjectData_SO buttlePoolObjectData_SO;
    public PoolObjectData_SO particlePoolObjectData_SO;
    [Header("音效组件")]
    public GameObject soundPrefab;
    private List<ObjectPool<GameObject>> poolButtleList = new List<ObjectPool<GameObject>>();
    private List<ObjectPool<GameObject>> poolParticleList = new List<ObjectPool<GameObject>>();
    private Queue<GameObject> soundQueue = new Queue<GameObject>();
    private void OnEnable()
    {
        EventHandler.InitSoundEffect += OnInitSoundEffect;
        EventHandler.ButtleGenerateEvent += OnButtleGenerateEvent;
        EventHandler.ParticleGenerateEvent += OnParticleGenerateEvent;
    }
    private void OnDisable()
    {
        EventHandler.InitSoundEffect -= OnInitSoundEffect;
        EventHandler.ButtleGenerateEvent -= OnButtleGenerateEvent;
        EventHandler.ParticleGenerateEvent -= OnParticleGenerateEvent;
    }
    private void Start()
    {
        CreatButtlePool();
        CreatParticlePool();
    }


    public void CreatButtlePool()
    {
        List<GameObject> buttlePrefabs = buttlePoolObjectData_SO.gameObjectsPrefabs;
        foreach (var item in buttlePrefabs)
        {
            var itemParent = new GameObject(item.name).transform;
            itemParent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, itemParent),
                i => { i.SetActive(true); },
                i => { i.SetActive(false); },
                i => { Destroy(i); }
            );

            var parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            poolButtleList.Add(newPool);
        }
    }
    private void OnButtleGenerateEvent(BulletType bulletType, Vector3 startPos, Vector3 targetPos, Transform startTransfrom)
    {
        ObjectPool<GameObject> objPool = bulletType switch
        {
            BulletType.undead_1 => poolButtleList[0],
            BulletType.undead_2 => poolButtleList[1],
            BulletType.undead_3 => poolButtleList[2],
            BulletType.undead_4 => poolButtleList[3],
            BulletType.undead_5 => poolButtleList[4],
            BulletType.undead_6 => poolButtleList[5],
            BulletType.archer_1 => poolButtleList[6],
            _ => poolButtleList[0]
        };

        GameObject buttle = objPool.Get();
        buttle.transform.position = startPos;


        buttle.GetComponent<IButtle>().Init(targetPos, objPool, startTransfrom);
        ReleaseButtle(objPool, buttle);
    }
    public void ReleaseButtle(ObjectPool<GameObject> pool, GameObject buttle)
    {
        StartCoroutine(ReleaseButtleRoutine(pool, buttle));
    }
    private IEnumerator ReleaseButtleRoutine(ObjectPool<GameObject> pool, GameObject buttle)
    {
        yield return new WaitForSeconds(5f);
        if (buttle.activeInHierarchy == true)
            pool.Release(buttle);
    }


    public void CreatParticlePool()
    {
        List<GameObject> particlePrefabs = particlePoolObjectData_SO.gameObjectsPrefabs;
        foreach (var item in particlePrefabs)
        {
            var itemParent = new GameObject(item.name).transform;
            itemParent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, itemParent),
                i => { i.SetActive(true); },
                i => { i.SetActive(false); },
                i => { Destroy(i); }
            );

            var parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            poolParticleList.Add(newPool);
        }
    }
    private void OnParticleGenerateEvent(Vector3 position, ParticaleEffectType particaleEffectType, float damage)
    {
        ObjectPool<GameObject> objPool = particaleEffectType switch
        {
            ParticaleEffectType.HealthDamage => poolParticleList[0],
            ParticaleEffectType.HealthRecover => poolParticleList[1],
            ParticaleEffectType.Stone => poolParticleList[2],
            ParticaleEffectType.Leaves => poolParticleList[3],
            ParticaleEffectType.Grass => poolParticleList[4],
            ParticaleEffectType.Gonden => poolParticleList[5],
            ParticaleEffectType.None => poolParticleList[0],
            _ => poolButtleList[0]
        };

        GameObject particale = objPool.Get();
        particale.transform.position = position;

        if ((particaleEffectType == ParticaleEffectType.HealthDamage || particaleEffectType == ParticaleEffectType.HealthRecover) && particale.GetComponent<HealthParticle>())
            particale.GetComponent<HealthParticle>().ChangeHealthText(damage);
        ReleaseParticale(objPool, particale);
    }
    public void ReleaseParticale(ObjectPool<GameObject> pool, GameObject particale)
    {
        StartCoroutine(ReleaseParticaleRoutine(pool, particale));
    }
    private IEnumerator ReleaseParticaleRoutine(ObjectPool<GameObject> pool, GameObject buttle)
    {
        yield return new WaitForSeconds(1f);
        if (buttle.activeInHierarchy == true)
            pool.Release(buttle);
    }


    private void CreatSoundPool()
    {
        var parent = new GameObject(soundPrefab.name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(soundPrefab, parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }
    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)
            CreatSoundPool();
        return soundQueue.Dequeue();
    }
    private void OnInitSoundEffect(SoundDetail soundDetail)
    {
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetail);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetail.soundClip.length));
    }
    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
}
