using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Transform player;

    private float yConst;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            enabled = false;
            return;
        }

        yConst = transform.position.y;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x + offset.x, yConst, player.position.z + offset.z);
    }
}
