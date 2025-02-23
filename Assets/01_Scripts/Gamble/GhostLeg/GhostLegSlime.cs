using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class GhostLegSlime : MonoBehaviour
{
    public float speed;
    public bool isMove;
    public Animator animator;

    public Vector2 dir;

    void Start()
    {
        dir = Vector2.down;
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            animator.SetBool("isWalk", isMove);
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SetDir(collision.name);
    }

    void SetDir(string colliderName)
    {
        switch (colliderName)
        {
            case "Left":
                if (dir == Vector2.down)
                    dir = Vector2.right;
                else
                    dir = Vector2.down;
                break;

            case "Right":
                if (dir == Vector2.down)
                    dir = Vector2.left;
                else
                    dir = Vector2.down;
                break;
        }

    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SlimeSound() => MasterAudio.PlaySound("SlimeSound");
}
