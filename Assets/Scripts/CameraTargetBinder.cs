using UnityEngine;

public class CameraTargetBinder : MonoBehaviour
{
    public Transform target;
    public string cameraRootPath = "_Scene/Camera";

    private void Awake()
    {
        Bind();
    }

    public void Bind()
    {
        if (target == null)
        {
            CarController3D car = FindFirstObjectByType<CarController3D>();
            if (car != null) target = car.transform;
        }

        Camera camera = FindSceneCamera();
        if (camera == null) return;

        camera.enabled = true;
        camera.tag = "MainCamera";
        EnsureSingleAudioListener(camera.gameObject);

        CarFollowCamera follow = camera.GetComponent<CarFollowCamera>();
        if (follow == null) follow = camera.gameObject.AddComponent<CarFollowCamera>();
        follow.target = target;
    }

    private static void EnsureSingleAudioListener(GameObject cameraObject)
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < listeners.Length; i++)
        {
            if (listeners[i].gameObject != cameraObject)
                listeners[i].enabled = false;
        }

        AudioListener activeListener = cameraObject.GetComponent<AudioListener>();
        if (activeListener == null) activeListener = cameraObject.AddComponent<AudioListener>();
        activeListener.enabled = true;
    }

    private Camera FindSceneCamera()
    {
        GameObject cameraRoot = GameObject.Find(cameraRootPath);
        if (cameraRoot != null)
        {
            Camera nested = cameraRoot.GetComponentInChildren<Camera>(true);
            if (nested != null) return nested;

            Camera rootCamera = cameraRoot.GetComponent<Camera>();
            if (rootCamera == null) rootCamera = cameraRoot.AddComponent<Camera>();
            return rootCamera;
        }

        Camera main = Camera.main;
        if (main != null) return main;

        GameObject fallback = new GameObject("Main Camera");
        fallback.transform.position = new Vector3(0f, 4f, -8f);
        fallback.transform.rotation = Quaternion.Euler(20f, 0f, 0f);
        return fallback.AddComponent<Camera>();
    }
}
