using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    [SerializeField]
    private float walkSpeed = 2f;
    private Vector3 velocity;
    private Vector3 initialPosition = new Vector3((float)547.200012, (float)17.9599991, (float)208.699997);

    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // CharacterControllerの設定を調整
        characterController.center = new Vector3(0f, 1f, 0f); // Centerを少し上に設定
        characterController.height = 2f; // Heightを適切な値に設定
        characterController.radius = 0.5f; // Radiusを適切な値に設定
        characterController.stepOffset = 0.3f; // StepOffsetを適切な値に設定

        target.localPosition = new Vector3((float)initialPosition.x, (float)initialPosition.y, (float)initialPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded)
        {
            velocity = Vector3.zero;
            var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (input.magnitude > 0f)
            {
                velocity = input.normalized * walkSpeed;
                //transform.LookAt(transform.position + input.normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(input.normalized), 6f * Time.deltaTime);
                animator.SetFloat("Speed", input.magnitude);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
            }
        }
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
