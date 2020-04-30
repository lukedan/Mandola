using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimStatus
{
    IDLE,
    MOVE_FORWARD,
    MOVE_LEFT,
    MOVE_RIGHT,
    MOVE_BACK,
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

        if (Input.anyKey)
        {
            if (Input.GetMouseButton(0))
            {
                animator.SetInteger("Status", (int)AnimStatus.SHOOT);
            }
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetMouseButton(0))
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_SHOOT);
                    }
                    else
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_FORWARD);
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    if (Input.GetMouseButton(0))
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_SHOOT);
                    }
                    else
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_LEFT);
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    if (Input.GetMouseButton(0))
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_SHOOT);
                    }
                    else
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_RIGHT);
                    }
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (Input.GetMouseButton(0))
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_SHOOT);
                    }
                    else
                    {
                        animator.SetInteger("Status", (int)AnimStatus.MOVE_BACK);
                    }
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetInteger("Status", (int)AnimStatus.MOVE_LEFT);
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    animator.SetInteger("Status", (int)AnimStatus.JUMP);
                }

            }


            if (Input.GetKey(KeyCode.R))
            {
                animator.SetInteger("Status", (int)AnimStatus.RECHARGE);
            }
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

