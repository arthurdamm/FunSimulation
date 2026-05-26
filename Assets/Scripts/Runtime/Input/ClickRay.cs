using System;
using UnityEngine;

public class ClickRay : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SimpleSwarm swarm;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var world = hit.point;
                var screen = (Vector2)Input.mousePosition;
                var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var alt = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                mainCamera.transform.SetParent(hit.collider.gameObject.transform.parent.parent);
                GameObject.Find("CameraMovement")?.SetActive(false);
                mainCamera.transform.position = mainCamera.transform.parent.position + (mainCamera.transform.parent.forward * -2.0f) + (mainCamera.transform.parent.up * .25f);
                mainCamera.transform.rotation = Quaternion.LookRotation(mainCamera.transform.parent.forward, mainCamera.transform.parent.up);
                Debug.Log($"HIT world: {world}, screen: {screen}, shift: {shift}, alt: {alt} name: {mainCamera.transform.parent.name}");
                // TryProcessClick(world, screen, shift, alt);
            }
        }
    }

    void TryProcessClick(Vector3 world, Vector2 screen, bool shift, bool alt)
    {
        Debug.Log($"HIT world: {world}, screen: {screen}, shift: {shift}, alt: {alt}");
        Vector3 pos = world;
        pos.y = 0.05f;
        swarm.MoveOrbitCenter(pos);

    }
}
