using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform mapRef;

    private Camera mainCamera;
    private Globals globals;
    private Vector3 offset;

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
        globals = FindObjectOfType<Globals>();
        offset = transform.position - player.position;
    }

    private void Update() {
        
    }

    void LateUpdate()
    {
        if (!globals.GameOver)
            transform.position = FollowPlayer();
        else ReturnCameraToStart();
    }

    /// <summary>
    /// Makes camera follows player
    /// </summary>
    /// <returns>New camera position</returns>
    private Vector3 FollowPlayer() => Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * 3f);

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
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 10f, elapsedTime / cameraReturnDuration);
        }
    }
}
