using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalAppearState : IState
{
    private Parameter towerParameter;
    private AnimalBaseController controller;
    private AnimatorStateInfo info;
    public AnimalAppearState(AnimalBaseController animalBaseController)
    {
        this.towerParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
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
public class AnimalIdleState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    private AnimatorStateInfo info;
    public AnimalIdleState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Idle");
    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {

    }
}
[System.Serializable]
public class AnimalMoveState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    private AnimatorStateInfo info;
    public AnimalMoveState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Move");
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
public class AnimalMeleeState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    public AnimalMeleeState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Melee");
    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {

    }
}

[System.Serializable]
public class AnimalSkillState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    public AnimalSkillState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
    }
    public void OnEnter()
    {
        controller.animator.Play("Remote");
    }
    public void OnUpdate()
    {

    }
    public void OnExit()
    {

    }
}
[System.Serializable]
public class AnimalHurtState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    private AnimatorStateInfo info;
    public AnimalHurtState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
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
public class AnimalDieState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    private AnimatorStateInfo info;
    public AnimalDieState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
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
public class AnimalNoneState : IState
{
    private Parameter animalParameter;
    private AnimalBaseController controller;
    public AnimalNoneState(AnimalBaseController animalBaseController)
    {
        this.animalParameter = animalBaseController.animalParameter;
        this.controller = animalBaseController;
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