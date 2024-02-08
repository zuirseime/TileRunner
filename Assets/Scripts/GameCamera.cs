using UnityEngine;

public class GameCamera : MonoBehaviour, IObserver
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform mapRef;
    [SerializeField] private Vector3 offset;

    private Camera mainCamera;
    private World world;

    private Vector3 jointCenter;
    private float cameraReturnDuration = 2f;
    private float elapsedTime = 0f;

    void Start() {
        if (player == null)
        {
            enabled = false;
            return;
        }

        mainCamera = GetComponent<Camera>();
        world = FindObjectOfType<World>();
        world.AddObserver(this);
    }

    private void OnDisable() {
        world.RemoveObserver(this);
    }

    private void Update() {
        
    }

    void LateUpdate()
    {
        if (!world.gameOver) {
            transform.position = FollowPlayer();
            transform.LookAt(player);
        }
    }

    private void GameOver() {
        Debug.Log("Camera's game over");
    }

    /// <summary>
    /// Makes camera follows player
    /// </summary>
    /// <returns>New camera position</returns>
    private Vector3 FollowPlayer() {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 3f);

        return smoothedPosition;
    }

    /// <summary>
    /// Makes camera returns to start position and increases it's size
    /// </summary>
    private void ReturnCameraToStart() {
        jointCenter = Vector3.zero;
        
        if (mapRef.childCount > 1) {
            for (int i = 0; i < mapRef.childCount; i++) {
            if (mapRef.GetChild(i).gameObject.activeSelf)
                jointCenter += mapRef.GetChild(i).transform.position;
            }

        jointCenter /= mapRef.childCount;
        jointCenter += offset;
        }

        if (transform.position != jointCenter) {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, jointCenter, elapsedTime / cameraReturnDuration);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 120f, elapsedTime / cameraReturnDuration);
        }
    }

    public void OnNotify(NotificationType notification) {
        if (notification == NotificationType.Death)
            GameOver();
    }
}
