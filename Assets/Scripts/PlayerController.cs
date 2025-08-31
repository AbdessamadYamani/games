using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f; // Adjusted to jump exactly to letter Y level (0.1)
    bool isGrounded = false;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // horizontal movement
        float h = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(h * speed, rb.linearVelocity.y);

        // Flip sprite based on direction
        if (h != 0)
        {
            spriteRenderer.flipX = h < 0;
            Debug.Log($"PlayerController: Moving horizontally with input {h}");
        }

        // jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("PlayerController: Jumping!");
        }
        
        // Debug ground state
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"PlayerController: Grounded state: {isGrounded}, Position: {transform.position}");
        }
        
        // Check if player is actually touching ground or letters using raycast
        CheckGroundedState();
    }
    
    void CheckGroundedState()
    {
        // Cast a ray downward to check if we're touching ground or letters
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == "Ground" || hit.collider.gameObject.name.StartsWith("Letter_"))
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    Debug.Log($"PlayerController: Raycast detected ground on {hit.collider.gameObject.name}");
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
                Debug.Log("PlayerController: Raycast detected no ground");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log($"PlayerController: Collision with {col.gameObject.name} at position {transform.position}");
        if (col.gameObject.name == "Ground" || col.gameObject.name.StartsWith("Letter_")) 
        {
            isGrounded = true;
            Debug.Log($"PlayerController: Now grounded on {col.gameObject.name} at position {transform.position}");
        }
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        Debug.Log($"PlayerController: Exit collision with {col.gameObject.name}");
        if (col.gameObject.name == "Ground" || col.gameObject.name.StartsWith("Letter_")) 
        {
            isGrounded = false;
            Debug.Log($"PlayerController: No longer grounded on {col.gameObject.name}");
        }
    }
} 