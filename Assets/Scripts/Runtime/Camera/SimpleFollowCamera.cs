using UnityEngine;

namespace FunSimulation
{
    public static class SimpleFollowCamera
    {
        private static CameraMovement _cameraMovement;
        
        static void Follow(Camera camera, Transform target)
        {
            camera.transform.SetParent(target);
            GameObject.Find("CameraMovement")?.SetActive(false);
            camera.transform.position = camera.transform.parent.position + (camera.transform.parent.forward * -2.0f) + (camera.transform.parent.up * .25f);
            camera.transform.rotation = Quaternion.LookRotation(camera.transform.parent.forward, camera.transform.parent.up);
        }    
    }
}
