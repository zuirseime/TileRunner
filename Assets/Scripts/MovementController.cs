using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private GameObject collider;

    private float xInput;
    private float zInput;

    void Start()
    {
        
    }

    void Update()
    {
        if (!TileGenerator.GameOver)
            InputHandler();

        if (collider.transform.localPosition.y < -5f)
        {
            TileGenerator.GameOver = true;
            collider.gameObject.SetActive(false);
        }
    }

    private void InputHandler()
    {
        // ToDo: Add a uniform moving by one integer coordinate per pressing button or swiping

        if (Input.GetKeyDown(KeyCode.W)) zInput = 1;
        else if (Input.GetKeyDown(KeyCode.S)) zInput = -1;
        else if (Input.GetKeyDown(KeyCode.A)) xInput = -1;
        else if (Input.GetKeyDown(KeyCode.D)) xInput = 1;
        else xInput = zInput = 0;

        Vector3Int xMove = Vector3Int.right * (int)xInput;
        Vector3Int zMove = Vector3Int.forward * (int)zInput;

        Vector3Int move = xMove + zMove;

        transform.Translate(move, Space.World);
    }
}
