using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask platformLayerMask;

    public bool IsGrounded { get; private set; }
    public bool IsOnPlatform { get; private set; }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            return;
        }
        else if (other.gameObject.CompareTag("Platform"))
        {
            IsOnPlatform = true;
        }

        IsGrounded = other != null && ((1 << other.gameObject.layer) & platformLayerMask) != 0;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IsGrounded = false;
        IsOnPlatform = false;
    }
}
