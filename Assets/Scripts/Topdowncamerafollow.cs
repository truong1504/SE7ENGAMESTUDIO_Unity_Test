using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset & Follow (chỉnh offset để có góc top-down giống ảnh mẫu)")]
    public Vector3 offset = new Vector3(0f, 12f, -6f);
    public float followSmoothTime = 0.15f;
    public float lookSmoothSpeed = 5f;

    private Vector3 velocity = Vector3.zero;
    private Transform currentTarget;

    void Start()
    {
        // Nếu chưa có target, tự tìm nhân vật
        if (target == null)
        {
            GameObject player = GameObject.Find("Jammo_LowPoly");
            if (player != null)
            {
                target = player.transform;
                currentTarget = target;
            }
        }
        else
        {
            currentTarget = target;
        }
    }

    void LateUpdate()
    {
        if (currentTarget == null) return;

        Vector3 desiredPos = currentTarget.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, followSmoothTime);

        Quaternion desiredRot = Quaternion.LookRotation((currentTarget.position - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, lookSmoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
}