using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private bool isGrounded = true;

    private Vector3 targetPosition;

    public GameTile currentTile;

    private PlayerState playerState = PlayerState.Standing;
    private Rigidbody body;
    private Vector2 swipeStartPosition;
    private float swipeThreshold = 25f;

    private Globals globals;

    void Start() {
        Input.multiTouchEnabled = false;

        body = GetComponent<Rigidbody>();
        body.drag = 0f;

        globals = FindObjectOfType<Globals>();
    }

    void Update() {
        InputHandler();

        if (globals.GameOver) {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate() {
        if (playerState == PlayerState.Moving) {
            globals.IsStarted = true;
            float speed = globals.MoveDistance / globals.Delay * .1f;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f) {
                transform.position = targetPosition;
                playerState = PlayerState.Standing;

                if (isGrounded) globals.Score++;
            }
        }
    }

    private void InputHandler() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, tileLayer);

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                swipeStartPosition = touch.position;
            else if (touch.phase == TouchPhase.Ended && isGrounded) {
                Vector2 swipeDelta = touch.position - swipeStartPosition;

                if (Mathf.Abs(swipeDelta.x) > swipeThreshold || Mathf.Abs(swipeDelta.y) > swipeThreshold) {
                    Vector3 destination = Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) ? 
                                          Vector3.right * swipeDelta.x : Vector3.forward * swipeDelta.y;

                    targetPosition = transform.position + destination.normalized * globals.MoveDistance;
                    playerState = PlayerState.Moving;
                }
            }
        }
    }
}
