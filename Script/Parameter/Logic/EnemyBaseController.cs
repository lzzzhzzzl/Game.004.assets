using System.Collections;
using System.Collections.Generic;
using Strategy.Astar;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour, ICharacter
{
    public float health;
    public Parameter enemyParameter;
    public Stack<MovementStep> moveToFireSteps;
    public MovementStep currentToFirePosition;
    public bool isDie;
    private IState currentState;
    private Dictionary<StateType, IState> enemyStateDict = new Dictionary<StateType, IState>();
    public Animator animator;
    public Transform targetTransfrom;
    public Rigidbody2D rb;
    public Astar astar;

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
        astar = gameObject.GetComponent<Astar>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator.runtimeAnimatorController = enemyParameter.animatorOverride;
        health = enemyParameter.health;

        enemyStateDict = ParameterManager.Instance.GetIStateDict(enemyParameter.stateDataList, this);
        astar.OnSetMapObstacleToCharacter();
        moveToFireSteps = new Stack<MovementStep>();

        if (enemyParameter.hasAppearAnimal)
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

        currentState = enemyStateDict[stateType];
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
                Transition(StateType.Hurt);
            else if (health <= 0)
                Transition(StateType.Die);
        }
    }

    private void OnDrawGizmos()
    {
        if (targetTransfrom)
        {
            Vector3 pointPosition = CommandMethod.GetRelativeDirection(targetTransfrom.position, gameObject.transform.position);
            Gizmos.DrawWireSphere(gameObject.transform.position + pointPosition + enemyParameter.MeleeAttackPosition, enemyParameter.MeleeAttackRadius);
        }
        Gizmos.DrawWireSphere(gameObject.transform.position, enemyParameter.checkRange);
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
        return enemyParameter.ID;
    }
    public float GetParameterHealth()
    {
        return health;
    }
}
