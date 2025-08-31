using UnityEngine;

public class DraggablePart : MonoBehaviour
{
    private Level3Manager level3Manager;
    private string partText;
    private bool isDragging = false;
    private Vector3 offset;
    public bool IsCurrentlyInDropZone { get; private set; }
    private Vector3 initialGridPosition;

    void Awake()
    {
        // Add a collider if it doesn't exist
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"DraggablePart: Mouse down on part '{partText}' at position {transform.position}");
        isDragging = true;
        IsCurrentlyInDropZone = false;
        offset = transform.position - GetMouseWorldPosition();
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        Debug.Log($"DraggablePart: Mouse up on part '{partText}' at position {transform.position}");
        isDragging = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        bool inDropZone = level3Manager.IsInDropZone(transform.position);
        Debug.Log($"DraggablePart: Part '{partText}' in drop zone: {inDropZone}");

        if (inDropZone)
        {
            IsCurrentlyInDropZone = true;
            Debug.Log($"DraggablePart: Setting IsCurrentlyInDropZone = true for part '{partText}'");
            level3Manager.OnPartDropped();
        }
        else
        {
            IsCurrentlyInDropZone = false;
            Debug.Log($"DraggablePart: Returning part '{partText}' to initial position {initialGridPosition}");
            transform.position = initialGridPosition;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void SetLevel3Manager(Level3Manager manager)
    {
        level3Manager = manager;
    }

    public void SetPartText(string text)
    {
        partText = text;
    }

    public string GetPartText()
    {
        return partText;
    }

    public void SetInitialGridPosition(Vector3 pos)
    {
        initialGridPosition = pos;
    }

    public void ReturnToInitialPosition()
    {
        IsCurrentlyInDropZone = false;
        transform.position = initialGridPosition;
    }
} 