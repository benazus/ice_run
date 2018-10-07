using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {
    private const float LANE_DISTANCE = 2.5f;
    private const float turnSpeed = 0.05f;

    private bool isRunning = false;

    // Animation
    private Animator animator;

    // Movement
    private CharacterController controller;
    private float jumpForce = 4.5f;
    private float gravity = 12f;
    private float verticalVelocity;
    private int desiredLane = 1; // Left = 0, Middle = 1, Right = 2

    // Speed Modifier
    private float originalSpeed = 8f;
    private float speed = 8f;
    private float speedIncreaseLastTick;
    private float speedIncreaseTime = 3f;
    private float speedIncreaseAmount = 0.1f;

    private void Start() {
        speed = originalSpeed;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Update() {
        if (isRunning == false)
            return;

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime) {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        // Gather input on which lane we should be
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveLane(false);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveLane(true);

        // Calculate which lane we should be in future
        Vector3 targetPosition = this.transform.position.z * Vector3.forward;
        switch(desiredLane) {
            case 0:
                targetPosition += Vector3.left * LANE_DISTANCE;
                break;
            case 2:
                targetPosition += Vector3.right * LANE_DISTANCE;
                break;
            default:
                break;
        }

        // Calculate move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - this.transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        // Calculate Y
        if (isGrounded) {
            verticalVelocity = -0.1f;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;           
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                StartSliding();
                Invoke("StopSliding", 1f);
            }
        }
        else {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast falling
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                verticalVelocity = -1 * jumpForce;
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move Player
        controller.Move(moveVector * Time.deltaTime);

        // Rotate the player in the direction he's moving      
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero) {
            dir.y = 0;
            this.transform.forward = Vector3.Lerp(this.transform.forward, dir, turnSpeed);
        }
    }

    private void MoveLane(bool goingRight) {
        // left
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded() {
        Ray groundRay = new Ray(
            new Vector3(
                controller.bounds.center.x,
                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
                controller.bounds.center.z), 
            Vector3.down);

        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1f);
        return (Physics.Raycast(groundRay, 0.3f));
    }

    public void StartRunning() {
        isRunning = true;
        animator.SetTrigger("StartRunning");
    }

    private void StartSliding() {
        animator.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    private void Crash() {
        animator.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        switch(hit.gameObject.tag) {
            case "Obstacle":
                Crash();
                break;
        }
    }

    private void StopSliding() {
        animator.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }
}
