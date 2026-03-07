using UnityEngine;

public class FaceVelocity : MonoBehaviour
{
    [SerializeField] float turnSpeedDegPerSec = 360f;
    public Vector3 Velocity { get; set; }  // set this from your movement script

    void Update()
    {
        Debug.Log($"Face t: {transform.gameObject.name}");
        
        Vector3 v = Velocity;
        v.y = 0f; // keep ships level for now

        if (v.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(v.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            turnSpeedDegPerSec * Time.deltaTime
        );
    }
}