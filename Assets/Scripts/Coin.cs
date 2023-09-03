using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0f, transform.rotation.y + 5f, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            GetComponent<Collider>().enabled = false;
            Globals.MoneyCount++;
            Destroy(gameObject);
        }
    }
}
