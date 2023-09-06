using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    void Start() {
        GetComponent<Collider>().isTrigger = true;
    }

    void FixedUpdate() {
        transform.Rotate(0f, transform.rotation.y + 5f, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            GetComponent<Collider>().enabled = false;
            FindObjectOfType<AudioManager>().Play(SoundName.Coin);
            Globals.MoneyCount++;
            Destroy(gameObject);
        }
    }
}
