using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Attacking,
    Dodging,
}

public class PlayerController : MonoBehaviour
{
    public float baseMoveSpeed;

    private PlayerState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = PlayerState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Normal:
                DoMovement();
                doFacing();
                break;
            case PlayerState.Attacking:
                break;
            case PlayerState.Dodging:
                break;
        }
    }

    private void DoMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal") * baseMoveSpeed / 10;
        float vertical = Input.GetAxisRaw("Vertical") * baseMoveSpeed / 10;
        transform.position = new Vector2(transform.position.x + horizontal, transform.position.y + vertical);
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
}
