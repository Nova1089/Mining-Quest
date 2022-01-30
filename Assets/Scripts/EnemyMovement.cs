// GameDev.tv Challenge Club. Got questions or want to share your nifty solution?
// Head over to - http://community.gamedev.tv

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] Animator animator;

    [SerializeField] private int changeDirectionTime;

    [SerializeField] private GameObject currentTouchingBlock;

    private float attackTime;
    private Vector2 moveDirection;

    private int randomNum;
    private int randomNum2;

    private float moveX;
    private float moveY;

    [SerializeField] private GameObject gameOver;

    private void Awake()
    {
        gameOver = GameObject.Find("GameOver");
    }

    private void Start()
    {

        StartCoroutine(CalcMovement());
        StartCoroutine(Attack());

        attackTime = Random.Range(1f, 2f);

    }

    void Update()
    {
        Move();
    }

    private IEnumerator CalcMovement()
    {
        randomNum = Random.Range(-1, 2);
        moveX = randomNum;

        randomNum2 = Random.Range(-1, 2);
        moveY = randomNum2;

        moveDirection = new Vector2(moveX, moveY).normalized;

        yield return new WaitForSeconds(changeDirectionTime);
        StartCoroutine(CalcMovement());
    }

    private IEnumerator Attack()
    {
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(attackTime);
        StartCoroutine(Attack());
    }


    private void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        if (moveX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void Hit()
    {
        animator.SetBool("Attack", false);

        Destroy(currentTouchingBlock);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            currentTouchingBlock = collision.gameObject;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            gameOver.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
