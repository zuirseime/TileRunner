using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : MonoBehaviour, IObserver {
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private bool isGrounded = true;

    private Vector3 targetPosition;
    private Vector3 rotationTarget;

    private PlayerState playerState = PlayerState.Standing;
    private Rigidbody body;
    private Vector2 swipeStartPosition;
    private float swipeThreshold = 25f;

    private World world;

    private GameObject playerSkin;

    void Start() {
        world = FindObjectOfType<World>();
        world.AddObserver(this);

        Input.multiTouchEnabled = false;

        body = GetComponent<Rigidbody>();
        body.drag = 0f;

        playerSkin = world.PlaySets.Find(n => n.Name == World.PlayerStats.CurrentPlaySet).Player;
        Instantiate(playerSkin, transform.localPosition, transform.rotation, transform);
    }

    //private void OnDisable() {
    //    world.RemoveObserver(this);
    //}

    void Update() {
        if (!world.gameOver) {
            InputHandler();
            CheckPosition();
        }
    }

    /// <summary>
    /// Checks player position after it moves
    /// </summary>
    private void CheckPosition() {
        if (playerState == PlayerState.Moving) {
            float speed = world.MoveDistance * .05f;
            Quaternion lookRotation = Quaternion.LookRotation(rotationTarget);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 15f);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                transform.position = targetPosition;
            if (transform.position == targetPosition) {
                playerState = PlayerState.Standing;

                transform.position = Vector3Int.RoundToInt(transform.position);
                transform.rotation = RoundRotation();

                if (isGrounded) world.Score++;
            }
        }
    }

    /// <summary>
    /// Rounds player rotation to int
    /// </summary>
    /// <param name="lookRotation">The target that the player rotates towards to</param>
    /// <param name="maxDegreesDelta"></param>
    /// <returns>Rounded rotation</returns>
    private Quaternion RoundRotation() => Quaternion.Euler(new Vector3(0, Mathf.RoundToInt(transform.eulerAngles.y), 0));

    /// <summary>
    /// Handles user's input
    /// </summary>
    private void InputHandler() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, .3f, tileLayer);

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                swipeStartPosition = touch.position;
            else if (touch.phase == TouchPhase.Ended && isGrounded) {
                Vector2 swipeDelta = touch.position - swipeStartPosition;

                if (Mathf.Abs(swipeDelta.x) > swipeThreshold || Mathf.Abs(swipeDelta.y) > swipeThreshold) {
                    Vector3 destination = Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) ? 
                                          Vector3.right * swipeDelta.x : Vector3.forward * swipeDelta.y;

                    targetPosition = transform.position + destination.normalized * world.MoveDistance;
                    rotationTarget = transform.position + destination.normalized * 1000f;

                    FindObjectOfType<AudioManager>().Play(SoundName.Move);
                    playerState = PlayerState.Moving;
                }
            }
        }
    }

    public void GetCoin(int worth) => world.localMoney += worth;

    public void OnNotify(NotificationType notification) {
        if (notification == NotificationType.Death)
            GameOver();
    }

    private void GameOver() {
        Debug.Log("Player's game over");
        FindObjectOfType<AudioManager>().Play(SoundName.Death);
        gameObject.SetActive(false);
        //world.RemoveObserver(this);
    }
}
