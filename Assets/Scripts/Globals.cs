using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {
    [SerializeField] private float baseDelay = 2f;
    [SerializeField] private float delay;
    private int previousScore = -1;

    public static PlaySetName CurrentPlaySet { get; set; } = PlaySetName.Standard;
    public static int MoneyCount { get; set; } = 0;

    [field: SerializeField] public Vector3 ZeroPosition { get; set; } = Vector3.zero;
    [field: SerializeField] public bool IsStarted { get; set; } = false;
    [field: SerializeField] public int Score { get; set; } = 0;
    [field: SerializeField] public bool GameOver { get; set; } = false;
    [field: SerializeField] public int MoveDistance { get; set; } = 1;
    public float Delay {
        get => delay;
        set {
            if (value < 0.05f) delay = 0.05f;
            else delay = value;
        }
    }

    [SerializeField] private Player player;

    [field: SerializeField] public List<PlaySet> PlaySets { get; set; }

    private void Start() {
        Delay = baseDelay;
    }

    private void Update() {
        if (Score != 0)
            IsStarted = true;

        if (player.transform.position.y < -1f)
            GameOver = true;

        if (Score % 5 == 0 && Score > previousScore) {
            float reductionFactor = 1f - ((Score * .2f) / 100f);
            Delay *= reductionFactor;
            Delay = Mathf.Max(delay, .02f);

            previousScore = Score;
        }
    }

    private void FixedUpdate() {
    }
}