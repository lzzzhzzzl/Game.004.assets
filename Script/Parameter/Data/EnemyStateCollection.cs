using System.Collections.Generic;
using System.Collections;
using Strategy.Astar;
using UnityEngine;
[System.Serializable]
public class EnemyAppearState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    public EnemyAppearState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Appear");
    }
    public void OnUpdate()
    {
        info = controller.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            controller.Transition(StateType.Idle);
        }
    }
    public void OnExit()
    {

    }
}
[System.Serializable]
public class EnemyIdelState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private float timer;

    public EnemyIdelState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Idle");
        CheckTargetTransfrom();
        CheckToMoveState();
    }
    public void OnUpdate()
    {
        controller.FlipTo(controller.targetTransfrom);

        if (timer >= enemyParameter.checkTime)
        {
            CheckTargetTransfrom();
            CheckToMoveState();
            timer = 0;
        }

        timer += Time.deltaTime;
    }
    public void OnExit()
    {
        timer = 0;
    }
    private void CheckTargetTransfrom()
    {
        if (controller.targetTransfrom != null)
            return;

        Collider2D[] colliders = new Collider2D[20];
        int count = Physics2D.OverlapCircleNonAlloc(controller.transform.position, enemyParameter.checkRange, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].GetComponent<PlayerParameter>())
            {
                controller.targetTransfrom = colliders[i].transform;
            }
        }
    }
    private void CheckToMoveState()
    {
        if (controller.targetTransfrom != null && Vector3.Distance(controller.targetTransfrom.position, controller.transform.position) <= enemyParameter.checkRange)
            controller.Transition(StateType.Move);
        else
            controller.targetTransfrom = null;
    }
}
[System.Serializable]
public class EnemyMoveState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private Stack<MovementStep> movementSteps = new Stack<MovementStep>();
    private MovementStep currentMovementStep;
    private Vector3 targetPosition;
    private float timer;
    private float skillTimer;
    public EnemyMoveState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        timer = enemyParameter.checkTime + 1;
        controller.animator.Play("Move");
        CheckToAttack();
        FindMovementStep();
    }
    public void OnUpdate()
    {
        if (!controller.targetTransfrom || Vector3.Distance(controller.targetTransfrom.position, controller.transform.position) > enemyParameter.checkRange)
        {
            controller.targetTransfrom = null;
            if (controller.moveToFireSteps.Count != 0)
            {
                if (Vector3.Distance(controller.transform.position, targetPosition) < .1f)
                {
                    controller.currentToFirePosition = controller.moveToFireSteps.Pop();
                    targetPosition = ((Vector3Int)controller.currentToFirePosition.gridCoordinate);
                }

                CheckTransfrom();
            }
            else
                controller.Transition(StateType.Idle);
        }
        else
        {
            controller.FlipTo(controller.targetTransfrom);
            CheckToAttack();

            if (timer > enemyParameter.checkTime && Vector3.Distance(controller.transform.position, controller.targetTransfrom.position) > enemyParameter.MeleeAttackRadius)
            {
                FindMovementStep();
                if (movementSteps.Count != 0)
                {
                    currentMovementStep = movementSteps.Pop();
                    targetPosition = ((Vector3Int)currentMovementStep.gridCoordinate);
                }
                timer = 0f;
            }

            if (movementSteps.Count != 0)
            {
                if (Vector3.Distance(controller.transform.position, targetPosition) < .1f)
                {
                    currentMovementStep = movementSteps.Pop();
                    targetPosition = ((Vector3Int)currentMovementStep.gridCoordinate);
                }

                if (controller.targetTransfrom)
                {
                    controller.transform.position = Vector2.MoveTowards(controller.transform.position,
                    targetPosition, enemyParameter.moveSpeed * Time.deltaTime);
                }
            }
            timer += Time.deltaTime;
        }
    }

    public void OnExit()
    {
        timer = 0f;
        skillTimer = 0f;
    }
    private void CheckToAttack()
    {
        if (!controller.targetTransfrom)
            return;

        Vector3 pointPosition = CommandMethod.GetRelativeDirection(controller.targetTransfrom.position, controller.transform.position);
        if (Vector3.Distance(controller.targetTransfrom.position, controller.transform.position + enemyParameter.MeleeAttackPosition + pointPosition) <= enemyParameter.MeleeAttackRadius)
            controller.Transition(StateType.Attack);
        else if (Vector3.Distance(controller.targetTransfrom.position, controller.transform.position + enemyParameter.MeleeAttackPosition + pointPosition) <= enemyParameter.RemoteAttackRadius
            && skillTimer >= enemyParameter.RemoteAttackTime)
        {
            controller.Transition(StateType.Skill);
            skillTimer = 0;
        }

        skillTimer += Time.deltaTime;
    }
    private void CheckTransfrom()
    {
        Collider2D collider = Physics2D.OverlapCircle(controller.gameObject.transform.position + enemyParameter.MeleeAttackPosition, enemyParameter.MeleeAttackRadius * 2);

        if (collider.GetComponent<PlayerParameter>())
        {
            controller.targetTransfrom = collider.transform;
        }
    }
    private void FindMovementStep()
    {
        if (!controller.targetTransfrom)
            return;

        movementSteps = new Stack<MovementStep>();
        Vector2Int startPos = new Vector2Int((int)controller.transform.position.x, (int)controller.transform.position.y);
        Vector2Int targetPos = new Vector2Int((int)controller.targetTransfrom.position.x, (int)controller.targetTransfrom.position.y);
        controller.astar.BuildPath(startPos, targetPos, movementSteps);
    }
}
[System.Serializable]
public class EnemyMeleeAttackState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    private bool isExecuteMelee;
    private bool isMeleeAnimator;
    private float timer;
    public EnemyMeleeAttackState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        isMeleeAnimator = false;
        isExecuteMelee = false;
        controller.animator.Play("Idle");
    }
    public void OnUpdate()
    {
        if (!controller.targetTransfrom)
            controller.Transition(StateType.Idle);
        else
        {
            if (timer >= 0.3f)
            {
                controller.animator.Play("Melee");
                isMeleeAnimator = true;
            }
            if (isMeleeAnimator)
            {
                isMeleeAnimator = false;
                controller.FlipTo(controller.targetTransfrom);
                info = controller.animator.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime >= 0.55f && !isExecuteMelee)
                {
                    MeleeAttack();
                    isExecuteMelee = true;
                }
                if (info.normalizedTime >= 0.95f)
                {
                    controller.Transition(StateType.Idle);
                }
            }
            timer++;
        }
    }
    public void OnExit()
    {
        timer = 0;
        isExecuteMelee = true;
    }
    private void MeleeAttack()
    {
        Collider2D[] colliders = new Collider2D[20];
        Vector3 pointPosition = CommandMethod.GetRelativeDirection(controller.targetTransfrom.position, controller.transform.position);
        int count = Physics2D.OverlapCircleNonAlloc(controller.gameObject.transform.position + pointPosition + enemyParameter.MeleeAttackPosition, enemyParameter.MeleeAttackRadius, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].GetComponent<PlayerParameter>())
            {
                PlayerParameter playerParameter = colliders[i].GetComponent<PlayerParameter>();
                playerParameter.HurtState(enemyParameter.MeleeAttackDamage, pointPosition);
            }
        }
    }
}
[System.Serializable]
public class EnemyRemoteAttackState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    private bool isRemoteAnimator;
    private bool isExecuteRemote;
    private float timer;
    public EnemyRemoteAttackState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        isRemoteAnimator = false;
        isExecuteRemote = false;
        controller.animator.Play("Idle");
    }
    public void OnUpdate()
    {
        if (!controller.targetTransfrom)
            controller.Transition(StateType.Idle);
        else
        {
            if (timer >= 0.3f)
            {
                controller.animator.Play("Remote");
                isRemoteAnimator = true;
            }
            if (isRemoteAnimator)
            {
                isRemoteAnimator = false;
                controller.FlipTo(controller.targetTransfrom);
                info = controller.animator.GetCurrentAnimatorStateInfo(0);
                if (info.normalizedTime >= 0.55f && !isExecuteRemote)
                {
                    RemoteAttack();
                    isExecuteRemote = true;
                }
                if (info.normalizedTime >= 0.95f)
                {
                    controller.Transition(StateType.Idle);
                }
            }
            timer++;
        }
    }
    public void OnExit()
    {
        timer = 0;
        isExecuteRemote = true;
    }
    private void RemoteAttack()
    {
        EventHandler.CallButtleGenerateEvent(enemyParameter.bulletType, controller.transform.position, controller.targetTransfrom.position, controller.transform);
    }
}
[System.Serializable]
public class EnemySummonState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    private bool isRemoteAnimator;
    private bool isExecuteRemote;
    public EnemySummonState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        isRemoteAnimator = false;
        isExecuteRemote = false;
        controller.animator.Play("Remote");
    }
    public void OnUpdate()
    {
        if (!controller.targetTransfrom)
            controller.Transition(StateType.Idle);
        else
        {
            isRemoteAnimator = false;
            controller.FlipTo(controller.targetTransfrom);
            info = controller.animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 0.55f && !isExecuteRemote)
            {
                RemoteAttack();
                isExecuteRemote = true;
            }
            if (info.normalizedTime >= 0.95f)
            {
                controller.Transition(StateType.Idle);
            }
        }
    }
    public void OnExit()
    {
        isExecuteRemote = true;
    }
    private void RemoteAttack()
    {
        EventHandler.CallGenerateParameterEvent(enemyParameter.generateParameterID, controller.transform.position);
    }
}
[System.Serializable]
public class EnemyDieState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    public EnemyDieState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        controller.isDie = true;
        controller.animator.Play("Die");
    }
    public void OnUpdate()
    {
        info = controller.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            controller.animator.speed = 0;
            controller.EnenyDieEvent();
        }
    }
    public void OnExit()
    {

    }
}
[System.Serializable]
public class EnemyHurtState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    private AnimatorStateInfo info;
    public EnemyHurtState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Hurt");
    }
    public void OnUpdate()
    {
        info = controller.animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            controller.Transition(StateType.Idle);
        }
    }
    public void OnExit()
    {

    }
}
[System.Serializable]
public class EnemyNoneState : IState
{
    private Parameter enemyParameter;
    private EnemyBaseController controller;
    public EnemyNoneState(EnemyBaseController enemyBaseController)
    {
        this.enemyParameter = enemyBaseController.enemyParameter;
        this.controller = enemyBaseController;
    }
    public void OnEnter()
    {

    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {

    }
}