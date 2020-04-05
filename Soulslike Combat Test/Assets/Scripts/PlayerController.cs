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
public class PlayerController : MonoBehaviour
{
    public float baseMoveSpeed;
    public float dodgeSpeed;
    public float dodgeDuration;
    public float dodgeRecovery;
    public TrailRenderer trailRenderer;
    public Rigidbody2D rBody;

    private PlayerState currentState;
    private Vector2 dodgeDirection;
    private bool dodged;
    private Vector3 moveDirection;
    private int envromentalCollisions;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.Normal;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        rBody = GetComponent<Rigidbody2D>();
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
                break;
            case PlayerState.Attacking:
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
            if (Input.GetAxis("Dodge") == 0)
            {
                dodged = false;
            }
        }
    }

    #region NormalMovement
    private void DoMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal") * baseMoveSpeed / 10;
        float vertical = Input.GetAxisRaw("Vertical") * baseMoveSpeed / 10;
        transform.position = new Vector2(transform.position.x + horizontal, transform.position.y + vertical);
        rBody.velocity = new Vector2(0,0);

        /* float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, vertical).normalized;

        rBody.velocity = moveDirection * baseMoveSpeed; */
    }
    private void doFacing()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector3 pointPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 facingDirection = new Vector2(
            pointPos.x - transform.position.x,
            pointPos.y - transform.position.y
        );

        transform.up = facingDirection;
    }
    #endregion

    #region DodgeBehavior
    private void doDodgeStateSet()
    {
        if (Input.GetAxis("Dodge") != 0 && !dodged)
        {
            dodgeDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            currentState = PlayerState.Dodging;
            dodged = true;
            trailRenderer.emitting = true;
            StartCoroutine(DodgeActiveTimer());
        }
    }
    private void doDodgeMovement()
    {
        if (envromentalCollisions <= 0)
        {
            float horizontal = dodgeDirection.x;
            float vertical = dodgeDirection.y;
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
        StartCoroutine(DodgeCoolDownTimer());

        rBody.velocity = new Vector2(0,0);
    }
    IEnumerator DodgeCoolDownTimer()
    {
        yield return new WaitForSeconds(dodgeRecovery);
        currentState = PlayerState.Normal;
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
