using UnityEngine;

[ExecuteAlways]   // runs in Edit Mode and Play Mode
public class LookAtOnce : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (!target) return;

        transform.LookAt(target);

        // Destroy immediately in editor
#if UNITY_EDITOR
        DestroyImmediate(this);
#else
        Destroy(this);
#endif
    }
}