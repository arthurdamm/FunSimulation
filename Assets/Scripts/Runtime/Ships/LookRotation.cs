using System;
using UnityEngine;

public class LookRotation : MonoBehaviour
{
    [SerializeField] public Vector3 forward;
    [SerializeField] private Transform target;

    private Vector3 _forward;

    private void Start()
    {
        _forward = forward;
    }

    private void Update()
    {
        if (!_forward.Equals(forward))
        {
            Debug.Log($"Changing to: {forward}");
            _forward = forward;
            Quaternion rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            target.rotation = rotation * target.rotation;
        }
    }
}
