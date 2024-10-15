using UnityEngine;

public class DropPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(Tag.Bubble))
            return;

        var bubble = collision.gameObject.GetComponent<Bubble>();
        bubble.Despawn();
    }
}
