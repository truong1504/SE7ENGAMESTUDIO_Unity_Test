using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Field Boundary (giới hạn sân bóng, chỉnh theo kích thước sân thực tế)")]
    public Vector2 boundaryMin = new Vector2(-20f, -20f); // X, Z min
    public Vector2 boundaryMax = new Vector2(20f, 20f);   // X, Z max

    [Header("Animation")]
    public Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDir;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleMovement();
        ClampToField();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A / D
        float v = Input.GetAxisRaw("Vertical");   // W / S

        moveDir = new Vector3(h, 0f, v);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 horizontalMove = moveDir * moveSpeed;

        // Gravity đơn giản để CharacterController bám đất
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove;
        finalMove.y = velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        // Xoay mặt theo hướng di chuyển
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Cập nhật Animator: Speed = 0 -> Idle, > 0 -> Run (dùng Blend Tree hoặc Transition theo threshold)
        if (animator != null)
            animator.SetFloat(SpeedHash, moveDir.magnitude);
    }

    void ClampToField()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, boundaryMin.x, boundaryMax.x);
        pos.z = Mathf.Clamp(pos.z, boundaryMin.y, boundaryMax.y);
        transform.position = pos;
    }
}