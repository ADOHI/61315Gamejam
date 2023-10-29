using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector2 wanderCenter;
    Vector2 wanderDist;
    Rigidbody2D rigid2D;
    Animator animator;
    SpriteRenderer spriteRenderer;
    [SerializeField] float sightRange = 8;
    [SerializeField] float trackRange = 10;
    [SerializeField] float trackSpeed = 4;
    [SerializeField] float wanderSpeed = 2;
    bool isTracking = false;
    bool isCaptured = false;
    float stunDuration = 2;
    float stunTimer = 0;
    bool isStun = false;
    RaycastHit2D hit;

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        wanderCenter = (Vector2)transform.position;
        wanderDist = (Vector2)transform.position;
        target = GameObject.Find("Target").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isStun)
        {
            if (stunTimer < stunDuration)
            {
                stunTimer += Time.deltaTime;
            }
            else
            {
                animator.speed = 1;
                isStun = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isCaptured && !isStun)
        {
            Vector2 dest;
            float speed;
            float dist;

            if (isTracking) dist = trackRange;
            else dist = sightRange;

            if (target != null && Vector2.Distance(rigid2D.position, target.position) <= dist && !Stage.instance.isInsideCircle)
            {
                animator.SetBool("isAngry", true);
                dest = target.position;
                speed = trackSpeed;
                isTracking = true;
            }
            else
            {
                animator.SetBool("isAngry", false);
                isTracking = false;

                if (Vector2.Distance(rigid2D.position, wanderDist) < 0.1f)
                {
                    wanderDist = wanderCenter + Random.insideUnitCircle * Random.Range(2, 6);
                }
                dest = wanderDist;
                speed = wanderSpeed;
                Vector2 norm = (dest-rigid2D.position).normalized;
                hit = Physics2D.Raycast(rigid2D.position + norm,  norm, 1f, LayerMask.GetMask("Enemy"));
                if (hit.collider != null)
                {
                    wanderDist = wanderCenter + Random.insideUnitCircle * Random.Range(2, 6);
                }
            }
            Vector2 dir = dest - rigid2D.position;
            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir.x), transform.localScale.y, transform.localScale.z);

            rigid2D.MovePosition(Vector2.MoveTowards(rigid2D.position, dest, speed * Time.deltaTime));
        }
    }

    public void Stun(int duration)
    {
        stunDuration = duration;
        stunTimer = 0;
        isStun = true;
        animator.speed = 0;
    }

    public void Captured(Vector2 posiition)
    {
        isCaptured = true;
        animator.speed = 0;
        rigid2D.simulated = false;
        //transform.position = posiition;
        spriteRenderer.DOFade(0,1).OnComplete(()=>Destroy(gameObject));
    }
}
