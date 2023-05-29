using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    /// <summary>
    /// 物体的位置信息
    /// </summary>
    private Transform spriteTrans;

    /// <summary>
    /// 物体的碰撞体
    /// </summary>
    private BoxCollider2D coll;

    /// <summary>
    /// 物体的重力，扔出后的下落速度
    /// </summary>
    [Header("物体的下落速度")]
    public float gravity = -3.5f;

    /// <summary>
    /// 物体是否已经到地面了
    /// </summary>
    private bool isGround;

    /// <summary>
    /// 物体与地面的距离
    /// </summary>
    private float distance;

    /// <summary>
    /// 物体下落的方向
    /// </summary>
    private Vector2 direction;

    /// <summary>
    /// 物体的落点
    /// </summary>
    private Vector3 targetPos;



    private void Awake()
    {
        //第一个子物体中上由Item，是显示物体用的，第二个是物体的阴影
        spriteTrans = transform.GetChild(0);
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
    }

    private void Update()
    {
        Bounce();
    }


    /// <summary>
    /// 初始化信息，投掷物体信息
    /// </summary>
    /// <param name="target">目标位置（落点）</param>
    /// <param name="dir">投掷方向</param>
    public void InitBounceItem(Vector3 target, Vector2 dir)
    {
        coll.enabled = false;
        direction = dir;
        targetPos = target;
        distance = Vector3.Distance(target, transform.position);

        spriteTrans.position += Vector3.up * 1.5f;
    }


    /// <summary>
    /// 投掷物体的操作，根据目标位置和方向修改物体的方向，实现投掷效果
    /// </summary>
    private void Bounce()
    {
        isGround = spriteTrans.position.y <= transform.position.y;

        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
        }

        if (!isGround)
        {
            spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
        }
        else
        {
            spriteTrans.position = transform.position;
            coll.enabled = true;
        }
    }

}

