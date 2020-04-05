using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Attacking,
    Dodging,
    Recovering,
}

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float baseMoveSpeed;
    public float dodgeSpeed;
    public float dodgeDuration;
    public float normalRecovery;
    public float pingRecovery;
    public float attackDuration;
    public float attackSpeed;
    public TrailRenderer trailRenderer;
    public Rigidbody2D rBody;
    public Animator animator;

    private PlayerState currentState;
    private Vector2 lockedMoveDirection;
    private Vector3 moveDirection;
    private int envromentalCollisions;
    private bool recentAttackPinged;
    private float modifiedAttackSpeed;

    //Input Variables
    private bool dodged;
    private bool attacked;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.Normal;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        dodged = false;
        attacked = false;
        recentAttackPinged = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                DoMovement();
                doFacing();
                doDodgeStateSet();
                DoAttackStateSet();
                break;
            case PlayerState.Attacking:
                DoAttackMovement();
                break;
            case PlayerState.Dodging:
                doDodgeMovement();
                break;
            case PlayerState.Recovering:
                break;
        }
        doKeyResets();
    }

    private void doKeyResets()
    {
        if (dodged)
        {
            if (Input.GetAxis("Dodge") == 0) { dodged = false; }
        }
        if (attacked)
        {
            if (Input.GetAxis("Attack") == 0)
            {
                attacked = false;
                Debug.Log("Attacked Reset");
            }
        }
    }

    private Vector2 GetFacingDirection()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 pointPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 facingDirection = new Vector2(
            pointPos.x - transform.position.x,
            pointPos.y - transform.position.y
        );

        return facingDirection;
    }

    #region NormalMovement
    private void DoMovement()
    {
        /* float horizontal = Input.GetAxisRaw("Horizontal") * baseMoveSpeed / 10;
        float vertical = Input.GetAxisRaw("Vertical") * baseMoveSpeed / 10;
        transform.position = new Vector2(transform.position.x + horizontal, transform.position.y + vertical);
        rBody.velocity = new Vector2(0,0); */

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, vertical).normalized;

        rBody.velocity = moveDirection * baseMoveSpeed;
    }
    private void doFacing()
    {
        Vector2 facingDirection = GetFacingDirection();

        transform.up = facingDirection;
    }
    #endregion

    #region DodgeBehavior
    private void doDodgeStateSet()
    {
        if (Input.GetAxis("Dodge") != 0 && !dodged)
        {
            lockedMoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            currentState = PlayerState.Dodging;
            dodged = true;
            trailRenderer.emitting = true;
            rBody.freezeRotation = true;

            gameObject.layer = LayerMask.NameToLayer("PlayerUninteractable");
            StartCoroutine(DodgeActiveTimer());
        }
    }
    private void doDodgeMovement()
    {
        if (envromentalCollisions <= 0)
        {
            float horizontal = lockedMoveDirection.x;
            float vertical = lockedMoveDirection.y;
            moveDirection = new Vector3(horizontal, vertical).normalized;

            rBody.velocity = moveDirection * dodgeSpeed;

            //transform.position = new Vector2(transform.position.x + horizontal, transform.position.y + vertical);
        }
    }
    IEnumerator DodgeActiveTimer()
    {
        yield return new WaitForSeconds(dodgeDuration);
        currentState = PlayerState.Recovering;
        trailRenderer.emitting = false;
        StartCoroutine(CoolDownTimer(normalRecovery));

        rBody.velocity = new Vector2(0, 0);
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    #endregion

    IEnumerator CoolDownTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        rBody.freezeRotation = false;
        currentState = PlayerState.Normal;

        if (recentAttackPinged)
        {
            recentAttackPinged = false;
            animator.SetBool("Pinged", false);
        }
    }

    #region AttackBehavior
    private void DoAttackStateSet()
    {
        if (Input.GetAxis("Attack") != 0 && !attacked)
        {
            Debug.Log("Attacking");
            lockedMoveDirection = GetFacingDirection();
            currentState = PlayerState.Attacking;
            attacked = true;
            trailRenderer.emitting = true;
            rBody.freezeRotation = true;
            modifiedAttackSpeed = attackSpeed;

            StartCoroutine(AttackActiveTimer());
            StartCoroutine(AttackMovementTimer());
            animator.SetBool("Attacking", true);
        }
    }
    IEnumerator AttackActiveTimer()
    {
        yield return new WaitForSeconds(attackDuration);
        currentState = PlayerState.Recovering;
        trailRenderer.emitting = false;
        rBody.velocity = new Vector2(0, 0);

        if (!recentAttackPinged) { StartCoroutine(CoolDownTimer(normalRecovery)); }
        else { StartCoroutine(CoolDownTimer(pingRecovery)); }
        animator.SetBool("Attacking", false);
    }
    private void DoAttackMovement()
    {
        if (envromentalCollisions <= 0)
        {
            float horizontal = lockedMoveDirection.x;
            float vertical = lockedMoveDirection.y;
            moveDirection = new Vector3(horizontal, vertical).normalized;

            rBody.velocity = moveDirection * modifiedAttackSpeed;
        }
        else
        {
            animator.SetBool("Pinged", true);
            recentAttackPinged = true;
        }
    }
    IEnumerator AttackMovementTimer()
    {
        yield return new WaitForSeconds(attackDuration / 2);
        modifiedAttackSpeed = 0;
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Envroment"))
        {
            envromentalCollisions++;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Envroment"))
        {
            envromentalCollisions--;
        }
    }
}
