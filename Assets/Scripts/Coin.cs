using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Coin : MonoBehaviour
{
    private BoxCollider coinCollider;

    [SerializeField, Range(1, 10)] private int cost = 1;

    void Awake() {
        coinCollider = GetComponent<BoxCollider>();
        coinCollider.isTrigger = true;
        coinCollider.size = new Vector3(0.5f, 1f, 0.5f);
        coinCollider.center = new Vector3(0f, 0.49f, 0f);
    }

    void FixedUpdate() {
        transform.Rotate(0f, transform.rotation.y + 5f, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            GetComponent<Collider>().enabled = false;
            FindObjectOfType<AudioManager>().Play(SoundName.Coin);
            player.GetCoin(cost);
            Destroy(gameObject);
        }
    }
}
