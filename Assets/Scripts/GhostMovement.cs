using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private float moveSpeed = 1f; // Slow movement
    private float screenBoundsX = 7f;
    private float screenBoundsY = 4f;
    private string answer;
    private Level2Manager level2Manager;
    private Rigidbody2D rb;

    void Start()
    {
        // Add Rigidbody2D for physics
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f; // No gravity
        rb.linearDamping = 0f; // No drag
        rb.freezeRotation = true; // Prevent rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Lock rotation
        
        // Set initial target position
        SetNewTargetPosition();
    }

    void Update()
    {
        // Move towards target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        // If reached target, set new target
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetNewTargetPosition();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"GhostMovement: Ghost clicked with answer: {answer}");
        if (level2Manager != null)
        {
            level2Manager.OnAnswerSelected(answer);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if colliding with another ghost by looking for GhostMovement component
        if (collision.gameObject.GetComponent<GhostMovement>() != null)
        {
            // Bounce off by setting a new target position in the opposite direction
            Vector3 bounceDirection = (transform.position - collision.transform.position).normalized;
            targetPosition = transform.position + bounceDirection * 3f;
            
            // Keep within screen bounds
            targetPosition.x = Mathf.Clamp(targetPosition.x, -screenBoundsX, screenBoundsX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, -screenBoundsY, screenBoundsY);
            
            // Ensure ghost keeps moving by setting velocity immediately
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            
            Debug.Log($"GhostMovement: Ghost bounced off another ghost");
        }
    }

    void SetNewTargetPosition()
    {
        // Generate random position within screen bounds
        float x = Random.Range(-screenBoundsX, screenBoundsX);
        float y = Random.Range(-screenBoundsY, screenBoundsY);
        targetPosition = new Vector3(x, y, 0);
    }

    public void SetAnswer(string answerText)
    {
        answer = answerText;
    }

    public void SetLevel2Manager(Level2Manager manager)
    {
        level2Manager = manager;
    }
} 