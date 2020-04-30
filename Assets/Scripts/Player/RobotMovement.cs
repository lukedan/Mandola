using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimStatus
{
    IDLE,
    MOVE,
    MOVE_SHOOT,
    JUMP,
    SHOOT,
    RECHARGE
};

public class RobotMovement : MonoBehaviour
{
	float speed = 4;
	float rotSpeed = 80;
	float gravity = 8;

	Vector3 moveDir = Vector3.zero;

	CharacterController controller;
	Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        // Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        // Get the angle between the points
        float angle = -AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen) - 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                animator.Play("MoveShoot", -1, 0f);
            }
            else
            {
                animator.Play("Shoot", -1, 0f);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.Play("Jump", -1, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            animator.Play("Reload", -1, 0f);
        }
        else if (Input.GetMouseButton(0))
        {
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
            {
                animator.SetInteger("Status", (int)AnimStatus.MOVE_SHOOT);
            }
            else
            {
                animator.SetInteger("Status", (int)AnimStatus.SHOOT);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S))
        {
            animator.Play("Move", -1, 0f);
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
        {
            //animator.Play("Move", -1, 0f);
            animator.SetInteger("Status", (int)AnimStatus.MOVE);
        }
        else
        {
            animator.SetInteger("Status", (int)AnimStatus.IDLE);
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}

