using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private GameObject playerCollider;
    [SerializeField] private AnimationCurve jumpCurve;

    [SerializeField] private int moveDistance = 1;
    [SerializeField] private float jumpDuration = 0.5f;

    [SerializeField] private LayerMask tileLayer;

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isJumping = false;
    private float jumpStartTime;

    private static int score = 0;
    public static int Score { get => score; }

    private Vector2 startingTouchPos;

    void Start()
    {
        isJumping = false;
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, tileLayer);

        if (isGrounded)
            InputHandler();

        if (playerCollider.transform.position.y < -5f)
        {
            TileGenerator.GameOver = true;
            playerCollider.gameObject.SetActive(false);
        }
    }

    private void InputHandler()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                startingTouchPos = touch.position;
            else if (touch.phase == TouchPhase.Ended)
            {
                isJumping = true;
                jumpStartTime = Time.time;

                if (isJumping)
                {
                    float elapsedTime = Time.time - jumpStartTime;
                    float normalizedTime = Mathf.Clamp01(elapsedTime / jumpDuration);
                    float jumpHeight = jumpCurve.Evaluate(normalizedTime);

                    Vector3 newPos = transform.position;
                    newPos.y = jumpHeight;
                    transform.position = newPos;

                    if (normalizedTime >= jumpDuration)
                        isJumping = false;
                }

                Vector2 swipeDelta = touch.position - startingTouchPos;
                float swipeThreshold = 50f;

                if (Mathf.Abs(swipeDelta.x) > swipeThreshold ||
                    Mathf.Abs(swipeDelta.y) > swipeThreshold)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        Move(Vector3.right * moveDistance * Mathf.Sign(swipeDelta.x));
                    else
                        Move(Vector3.forward * moveDistance * Mathf.Sign(swipeDelta.y));
                }
            }
        }
    }

    private void Move(Vector3 direction)
    {
        float moveAmount = 1f;

        transform.Translate(direction * moveAmount, Space.World);

        if (isGrounded)
            score++;
    }
}
