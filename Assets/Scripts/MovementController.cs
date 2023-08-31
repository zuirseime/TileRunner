using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private bool isGrounded = true;

    [SerializeField] private float moveDistance = 1f;
    private Vector3 targetPosition;

    private static int score = 0;
    public static int Score { get => score; }

    private Vector2 swipeStartPosition;
    private Rigidbody body;
    private float swipeMultiplier = 10f;
    private float swipeThreshold = 25f;
    private float deathHeight = -5f;

    void Start()
    {
        Input.multiTouchEnabled = false;

        score = 0;

        body = GetComponent<Rigidbody>();
        body.drag = 0f;
    }

    void Update()
    {
        InputHandler();

        if (transform.position.y < deathHeight)
        {
            TileGenerator.GameOver = true;
            gameObject.SetActive(false);
        }
    }

    private void InputHandler()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, tileLayer);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                swipeStartPosition = touch.position;
            else if (touch.phase == TouchPhase.Ended && isGrounded)
            {
                Vector2 swipeDelta = touch.position - swipeStartPosition;

                if (Mathf.Abs(swipeDelta.x) > swipeThreshold || Mathf.Abs(swipeDelta.y) > swipeThreshold)
                    Move(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) ? Vector3.right * swipeDelta.x * swipeMultiplier : Vector3.forward * swipeDelta.y * swipeMultiplier);
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            transform.position = Vector3Int.RoundToInt(targetPosition);
    }

    private void Move(Vector3 direction)
    {
        targetPosition = Vector3Int.RoundToInt(transform.position) + direction.normalized;
        print(targetPosition);

        body.AddForce(direction.normalized * moveDistance, ForceMode.Impulse);

        if (isGrounded)
            score++;
    }
}
