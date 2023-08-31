using UnityEngine;

public class Globals : MonoBehaviour {
    [SerializeField] private float baseDelay = 2f;
    private float delay;
    private int previousScore = -1;

    [field: SerializeField] public bool IsStarted { get; set; } = false;
    [field: SerializeField] public Vector3 CameraStartPosition { get; set; } = Vector3.zero; 
    [field: SerializeField] public int Score { get; set; } = 0;
    [field: SerializeField] public bool GameOver { get; set; } = false;
    [field: SerializeField] public int MoveDistance { get; set; } = 1;
    [field: SerializeField] public float Delay {
        get => delay;
        set {
            if (value < 0.05f) delay = 0.05f;
            else delay = value;
        }
    }

    [SerializeField] private PlayerController player;

    private void Start() {
        Delay = baseDelay;

        CameraStartPosition = Camera.main.transform.position;
    }

    private void Update() {
        if (player.transform.position.y < -1f)
            GameOver = true;

        if (Score % 5 == 0 && Score > previousScore) {
            float reductionFactor = 1f - ((Score * .2f) / 100f);
            Delay *= reductionFactor;
            Delay = Mathf.Max(delay, .02f);

            previousScore = Score;
        }
    }
}