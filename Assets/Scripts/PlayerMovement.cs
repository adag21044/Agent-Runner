using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 lastTouchPosition;
    private bool isTouching = false;
    private Rigidbody rb;
    private bool isGrounded = true;

    private int currentPositionIndex = 1; // 0: Sol, 1: Orta, 2: Sağ
    private Vector3 targetPosition;       // Hedef pozisyon

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float swipeThreshold = 50f; // Kaydırma için eşik değer (piksellerde)
    [SerializeField] private float moveDistance = 0.7f;   // Sağa veya sola kayma mesafesi
    [SerializeField] private float smoothSpeed = 5f;      // Yumuşak geçiş hızı
    [SerializeField] private float forwardSpeed = 2f;     // Z eksenindeki ileri hareket hızı

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position; // Başlangıç hedef pozisyonunu mevcut pozisyona ayarla
    }

    private void Update()
    {
        // Z ekseninde sürekli ileri hareket
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.deltaTime;
        transform.position += forwardMovement;

        // X ekseninde hedef pozisyona yumuşak geçiş
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(targetPosition.x, transform.position.y, transform.position.z),
            smoothSpeed * Time.deltaTime
        );

        // Ekrana dokunma var mı kontrol et
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // İlk dokunuşu al

            if (touch.phase == TouchPhase.Began)
            {
                // Dokunma başladığında pozisyonu kaydet
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                // Parmağın yeni pozisyonunu hesapla
                Vector2 currentTouchPosition = touch.position;
                Vector2 deltaPosition = currentTouchPosition - lastTouchPosition;

                if (Mathf.Abs(deltaPosition.x) > swipeThreshold)
                {
                    // Sağa kaydırma
                    if (deltaPosition.x > 0 && currentPositionIndex < 2)
                    {
                        Debug.Log("Right Swipe");
                        targetPosition += new Vector3(moveDistance, 0, 0); // Yeni hedef pozisyon sağa kaydırılır
                        currentPositionIndex++; // Pozisyonu bir sağa kaydır
                    }
                    // Sola kaydırma
                    else if (deltaPosition.x < 0 && currentPositionIndex > 0)
                    {
                        Debug.Log("Left Swipe");
                        targetPosition += new Vector3(-moveDistance, 0, 0); // Yeni hedef pozisyon sola kaydırılır
                        currentPositionIndex--; // Pozisyonu bir sola kaydır
                    }
                    isTouching = false; // Tek seferlik kaydırma için
                }
                else if (deltaPosition.y > swipeThreshold && isGrounded)
                {
                    Debug.Log("Jump");
                    // Yukarı kaydırma (Zıplama)
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false; // Zıpladıktan sonra yere değmediğini belirt
                    isTouching = false;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // Dokunma bittiğinde
                isTouching = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nesne yere temas ettiğinde isGrounded'ı true yap
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
