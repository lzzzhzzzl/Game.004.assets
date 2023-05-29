using System.Collections;
using System.Collections.Generic;
using Strategy.Astar;
using UnityEngine;

public class AnimalBaseController : MonoBehaviour, ICharacter
{
    public float health;
    public Parameter animalParameter;
    public bool isDie;
    private IState currentState;
    private Dictionary<StateType, IState> animalStateDict = new Dictionary<StateType, IState>();
    public Animator animator;
    public Transform targetTransfrom;
    public Rigidbody2D rb;

    private void OnEnable()
    {
        EventHandler.ClearAllEnemyTargetTransfromEvent += OnClearAllEnemyTargetTransfromEvent;
    }
    private void OnDisable()
    {
        EventHandler.ClearAllEnemyTargetTransfromEvent -= OnClearAllEnemyTargetTransfromEvent;
    }
    private void Start()
    {
        isDie = false;
        Init();
        if (health <= 0)
            Transition(StateType.Die);
    }

    public void Init()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        animator.runtimeAnimatorController = animalParameter.animatorOverride;
        health = animalParameter.health;

        animalStateDict = ParameterManager.Instance.GetIStateDict(animalParameter.stateDataList, this);

        if (animalParameter.hasAppearAnimal)
            Transition(StateType.Appear);
        else
            Transition(StateType.Idle);
    }
    private void Update()
    {
        if (currentState != null)
            currentState.OnUpdate();
    }


    public void Transition(StateType stateType)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = animalStateDict[stateType];
        currentState.OnEnter();
    }
    public void FlipTo(Transform targetTransfrom)
    {
        if (targetTransfrom != null)
        {
            if (targetTransfrom.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (targetTransfrom.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
    public void HurtState(float attackDamage, Transform targetTransfrom)
    {
        if (!isDie)
        {
            this.targetTransfrom = targetTransfrom;
            health -= attackDamage;
            EventHandler.CallParticleGenerateEvent(transform.position + Vector3.up, ParticaleEffectType.HealthDamage, attackDamage);
            if (health > 0)
                Transition(StateType.Die);
            else if (health <= 0)
                Transition(StateType.Die);
        }
    }

    private void OnDrawGizmos()
    {
        if (targetTransfrom)
        {
            Vector3 pointPosition = CommandMethod.GetRelativeDirection(targetTransfrom.position, gameObject.transform.position);
            Gizmos.DrawWireSphere(gameObject.transform.position + pointPosition + animalParameter.MeleeAttackPosition, animalParameter.MeleeAttackRadius);
        }
        Gizmos.DrawWireSphere(gameObject.transform.position, animalParameter.checkRange);
    }
    public void EnenyDieEvent()
    {
        currentState = null;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(EnenyCountdownDeath());
    }
    private IEnumerator EnenyCountdownDeath()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < animalParameter.itemCanGenerate.Length; i++)
        {
            int amountToProduce;

            if (animalParameter.itemMaxCount[i] == animalParameter.itemMinCount[i])
            {
                amountToProduce = animalParameter.itemMaxCount[i];
            }
            else
            {
                amountToProduce = Random.Range(animalParameter.itemMinCount[i], animalParameter.itemMaxCount[i] + 1);
            }

            for (int j = 0; j < amountToProduce; j++)
            {
                var spawPos = new Vector3(transform.position.x + Random.Range(-1f, 1f),
                transform.position.y + Random.Range(-1f, 1f), 0);
                EventHandler.CallGenerateItemEvent(animalParameter.itemCanGenerate[i], spawPos);
            }
        }
        Destroy(gameObject);
    }
    private void OnClearAllEnemyTargetTransfromEvent()
    {
        targetTransfrom = null;
    }

    public SerializableVector3 GetParameterPosition()
    {
        return new SerializableVector3(transform.position);
    }
    public string GetParameterID()
    {
        return animalParameter.ID;
    }
    public float GetParameterHealth()
    {
        return health;
    }
}
