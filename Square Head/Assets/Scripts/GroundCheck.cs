using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask platformLayerMask;

    public bool IsGrounded { get; private set; }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            return;
        }

        IsGrounded = other != null && ((1 << other.gameObject.layer) & platformLayerMask) != 0;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        IsGrounded = false;
    }
}
