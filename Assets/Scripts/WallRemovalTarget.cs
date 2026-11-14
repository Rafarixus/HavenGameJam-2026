using UnityEngine;

public class WallRemovalTarget : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    private bool activated;

    private void OnCollisionEnter(Collision collision)
    {
        if (activated || !collision.gameObject.GetComponentInParent<Ball>()) return;

        activated = true;
        // Disable the whole barrier so both its visuals and all colliders disappear.
        if (wall) wall.SetActive(false);
    }
}
