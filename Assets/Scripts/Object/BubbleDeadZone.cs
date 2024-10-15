using UnityEngine;

public class BubbleDeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(Tag.Bubble))
            return;

        var bubble = collision.GetComponent<Bubble>();
        bubble.Despawn();
    }
}
