using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("角色移动的速度")]
    public float speed;

    private Rigidbody2D rb;
    private Animator[] animators;
    private float inputX;
    private float inputY;
    private float mouseX;
    private float mouseY;
    private Vector2 movementInput;
    private Camera mainCamera;
    private bool isMoving;
    private bool inputDisable;
    private bool useTool = false;


    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickEvent += OnMouseClickEvent;
        EventHandler.IslandMapClickEvent += OnIslandMapClickEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickEvent -= OnMouseClickEvent;
        EventHandler.IslandMapClickEvent -= OnIslandMapClickEvent;
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
    }

    private void OnEndGameEvent()
    {
        inputDisable = true;
    }

    private void Update()
    {
        if (!inputDisable)
            PlayerInput();
        else
            isMoving = false;
        SwitchAnimation();
    }
    private void FixedUpdate()
    {
        if (!inputDisable)
            Movement();
    }
    private void OnBeforeSceneLoadEvent()
    {
        Debug.Log("禁用玩家移动");
        inputDisable = true;
    }
    private void OnAfterSceneDataLoadEvent()
    {
        Debug.Log("恢复移动");
        inputDisable = false;
    }
    private void OnIslandMapClickEvent(bool isClick)
    {
        inputDisable = isClick;
    }
    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
    private void PlayerInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }

        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;
    }

    private void OnMouseClickEvent(Vector3 mouseWorldPos, ItemDetail itemDetail, int index)
    {
        if (useTool)
            return;
        //WORKFLOW:如需添加物品类型，此处也要添加
        if (itemDetail.itemType == ItemType.AxeTool || itemDetail.itemType == ItemType.PickAxe || itemDetail.itemType == ItemType.Sword || itemDetail.itemType == ItemType.Throw)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.5f);

            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0f;
            else
                mouseX = 0;
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetail, index));
        }
        else
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetail, index);
    }
    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetail itemDetail, int index)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        AnimatorStateInfo info = new AnimatorStateInfo();
        foreach (var anim in animators)
        {
            anim.SetTrigger("UseItem");
            anim.SetFloat("MouseX", mouseX);
            anim.SetFloat("MouseY", mouseY);
            if (anim.gameObject.name == "Tool")
            {
                info = anim.GetCurrentAnimatorStateInfo(0);
            }
        }
        yield return new WaitForSeconds(info.length / 2);

        if (itemDetail.itemType == ItemType.Sword)
        {
            Vector3 position = CommandMethod.GetRelativeDirection(mouseWorldPos, transform.position);
            EventHandler.CallExecuteActionAfterAnimation(gameObject.transform.position + position, itemDetail, index);
        }
        else if (itemDetail.itemType == ItemType.PickAxe || itemDetail.itemType == ItemType.AxeTool)
            if (Mathf.Abs(mouseWorldPos.x - transform.position.x) < itemDetail.itemUseRadius && Mathf.Abs(mouseWorldPos.y - transform.position.y) < itemDetail.itemUseRadius)
                EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetail, index);
        if (info.normalizedTime <= 0.85f)
            yield return new WaitForSeconds(info.normalizedTime - info.length);
        useTool = false;
        inputDisable = false;
    }

    private void Movement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    private void OnMoveToPosition(Vector3 targetPosition)
    {
        Debug.Log("玩家传送至:" + targetPosition);
        transform.position = targetPosition;
    }

    public void HurtState(Vector3 attackDirection)
    {

        StartCoroutine(TriggerPlayerHurtAnimator(attackDirection));
    }
    public IEnumerator TriggerPlayerHurtAnimator(Vector3 attackDirection)
    {
        useTool = true;
        inputDisable = true;
        yield return null;

        rb.AddForce(attackDirection, ForceMode2D.Impulse);

        foreach (var anim in animators)
        {
            if (anim.gameObject.name != "Tool")
            {
                anim.SetTrigger("UseItem");
                anim.SetFloat("MouseX", attackDirection.x);
                anim.SetFloat("MouseY", attackDirection.y);
            }
        }

        yield return new WaitForSeconds(0.75f);
        rb.velocity = Vector2.zero;
        EventHandler.CallUpdatePlayerAnimatorEvent();
        yield return null;
        useTool = false;
        inputDisable = false;
    }
    public void DieState(Vector3 attackDirection)
    {
        StartCoroutine(TriggerPlayerDieAnimator(attackDirection));
    }
    public IEnumerator TriggerPlayerDieAnimator(Vector3 attackDirection)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rb.AddForce(attackDirection, ForceMode2D.Impulse);

        foreach (var anim in animators)
        {
            if (anim.gameObject.name != "Tool")
            {
                anim.SetTrigger("UseItem");
                anim.SetFloat("MouseX", attackDirection.x);
                anim.SetFloat("MouseY", attackDirection.y);
            }
        }
        rb.velocity = Vector2.zero;
        EventHandler.CallClearAllEnemyTargetTransfromEvent();
        yield return new WaitForSeconds(1f);

        yield return null;
    }
    public void ResetPlayerParameter()
    {
        useTool = false;
        inputDisable = false;
    }
}
