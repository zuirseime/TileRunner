using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Transform player;

    private Camera mainCamera;
    private Globals globals;
    private Vector3 offset;

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
        if (globals.GameOver && !player) {
            if (transform.position != globals.CameraStartPosition)
                //transform.position = Vector3.Lerp(transform.position, globals.CameraStartPosition, Time.deltaTime * 1.5f);
                transform.position = Vector3.MoveTowards(transform.position, globals.CameraStartPosition, Time.deltaTime * 1.5f);
            else
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, 7f, Time.deltaTime * 5f);
        }
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * 3f);
    }
}
