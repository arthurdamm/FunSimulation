using System;
using UnityEngine;

public class ClickRay : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject foodPrefab;
    
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
                TryProcessClick(world, screen, shift, alt);
            }
        }
    }

    void TryProcessClick(Vector3 world, Vector2 screen, bool shift, bool alt)
    {
        Debug.Log($"HIT world: {world}, screen: {screen}, shift: {shift}, alt: {alt}");
        Vector3 pos = world;
        pos.y = 0.05f;
        GameObject food = Instantiate(foodPrefab, pos, Quaternion.identity);
        food.transform.parent = transform;
    }
}
