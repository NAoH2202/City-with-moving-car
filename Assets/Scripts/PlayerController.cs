using UnityEngine;
using UnityEngine.SceneManagement; // Cần thiết để reset lại Scene khi xe hỏng

public class PlayerController : MonoBehaviour
{
    [Header("Cấu hình Tốc độ")]
    public float speed = 5f;

    [Header("Cấu hình Đường đua (Checkpoints)")]
    public Vector3[] checkPoints;
    public float arrivalDistance = 0.2f;
    private int currentTargetIndex = 0;

    [Header("Chỉ số của Xe (Player Stats)")]
    [Tooltip("Mức độ hư hại (%) - Ban đầu là 0")]
    public float damaged = 0f;
    [Tooltip("Lượng xăng hiện tại")]
    public float fuel = 100f;
    [Tooltip("Tổng dung tích xăng - Ban đầu là 100")]
    public float capacity = 100f;
    [Tooltip("Số vòng đua đã hoàn thành")]
    public int laps = 0;

    private Vector3 startPosition; // Lưu vị trí xuất phát ban đầu

    void Start()
    {
        // 1. Đặt lượng xăng ban đầu bằng dung tích tối đa
        fuel = capacity;

        // 2. Lưu lại vị trí xuất phát ban đầu khi vừa Start Scene
        startPosition = transform.position;

        if (checkPoints == null || checkPoints.Length == 0)
        {
            Debug.LogWarning("Vui lòng thiết lập các điểm Checkpoints!");
        }
    }

    void Update()
    {
        if (checkPoints.Length == 0) return;

        // Xe chỉ di chuyển được nếu chưa bị hỏng hoàn toàn và còn xăng
        if (damaged < 100f && fuel > 0)
        {
            MoveTowardsTarget();
            
            // Giảm xăng theo thời gian (Tùy chọn để logic thực tế hơn)
            fuel -= Time.deltaTime * 0.5f; 
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 targetPosition = checkPoints[currentTargetIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Kiểm tra đến đích Checkpoint bằng .magnitude
        float distance = (targetPosition - transform.position).magnitude;
        if (distance < arrivalDistance)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= checkPoints.Length)
            {
                currentTargetIndex = 0;
            }
        }
    }

    // Xử lý va chạm (Sử dụng Trigger để xe không bị nảy hoặc lệch hướng bay ra ngoài)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[XUẤT HIỆN VA CHẠM] Xe đã chạm vào một Object tên là: {other.gameObject.name} | Tag của nó là: {other.tag}");
        // 1. Nếu va chạm với Vật cản (Obstacle) hoặc Tường biên (Wall)
        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            damaged += 5f; // Tăng mức độ hư hại lên 5%
            Debug.Log($"Xe bị va chạm! Độ hư hại hiện tại: {damaged}%");

            // Nếu hư hại đạt hoặc vượt quá 100%, reset lại Scene
            if (damaged >= 100f)
            {
                Debug.LogError("Xe đã bị hỏng hoàn toàn! Đang khởi động lại màn chơi...");
                ResetScene();
            }
        }

        // 2. Nếu đi qua vạch xuất phát/đích (FinishLine)
        if (other.CompareTag("FinishLine"))
        {
            laps++; // Tăng số vòng đua lên 1
            Debug.Log($"Chúc mừng! Bạn đã hoàn thành vòng thứ: {laps}");
        }
    }

    private void ResetScene()
    {
        // Lấy tên của Scene hiện tại và load lại nó
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

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