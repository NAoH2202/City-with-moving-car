using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Cấu hình Tốc độ")]
    [Tooltip("Tốc độ di chuyển của Player")]
    public float speed = 5f;

    [Header("Cấu hình Đường đua (Checkpoints)")]
    [Tooltip("Mảng chứa tọa độ các góc của đường đua")]
    public Vector3[] checkPoints;

    [Tooltip("Khoảng cách sai số chấp nhận được để tính là đã đến đích")]
    public float arrivalDistance = 0.2f;

    // Chỉ số của điểm checkpoint hiện tại mà xe đang hướng tới
    private int currentTargetIndex = 0;

    void Start()
    {
        // Kiểm tra xem người dùng đã thiết lập các điểm checkpoint trong Inspector chưa
        if (checkPoints == null || checkPoints.Length == 0)
        {
            Debug.LogWarning("Vui lòng thiết lập các điểm Checkpoints trong bảng Inspector!");
        }
    }

    void Update()
    {
        // Nếu không có điểm checkpoint nào, không thực hiện di chuyển
        if (checkPoints.Length == 0) return;

        MoveTowardsTarget();
    }

    /// <summary>
    /// Hàm xử lý di chuyển Player hướng về phía Checkpoint mục tiêu
    /// </summary>
    private void MoveTowardsTarget()
    {
        // Lấy vị trí đích đến hiện tại từ mảng
        Vector3 targetPosition = checkPoints[currentTargetIndex];

        // 1. Tính hướng di chuyển từ vị trí hiện tại đến đích
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 2. Di chuyển Player theo hướng đã tính dựa trên speed và deltaTime
        transform.position += direction * speed * Time.deltaTime;

        // 3. (Tùy chọn) Xoay Player nhìn về phía mục tiêu cho mượt mà
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 4. Kiểm tra xem Player đã đến đích chưa bằng cách tính hiệu số 2 vector (.magnitude)
        float distance = (targetPosition - transform.position).magnitude;

        if (distance < arrivalDistance)
        {
            // Chuyển sang điểm checkpoint tiếp theo
            currentTargetIndex++;

            // Tránh tràn mảng: Nếu vượt quá điểm cuối cùng thì quay về điểm 0
            if (currentTargetIndex >= checkPoints.Length)
            {
                currentTargetIndex = 0;
            }

            Debug.Log($"Đã đến checkpoint! Đích tiếp theo: checkPoints[{currentTargetIndex}]");
        }
    }

    // Vẽ các đường nối checkpoint trong Editor để dễ quan sát hệ thống đường đua
    private void OnDrawGizmos()
    {
        if (checkPoints == null || checkPoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < checkPoints.Length; i++)
        {
            Gizmos.DrawSphere(checkPoints[i], 0.3f);
            
            int nextIndex = (i + 1) % checkPoints.Length;
            Gizmos.DrawLine(checkPoints[i], checkPoints[nextIndex]);
        }
    }
}