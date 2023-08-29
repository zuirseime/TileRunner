using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private int score = 0;

    private TextMeshProUGUI tmp;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        StartCoroutine(IncreaseScore(1f));
    }

    void Update()
    {
        tmp.text = score.ToString();
    }

    private IEnumerator IncreaseScore(float delay)
    {
        // ToDo: Fix the player score and change it from increasing per second if it's not a game over to increasing if player is moving from prevoious tile to new one
        if (TileGenerator.GameOver)
            yield return null;

        score++;

        yield return new WaitForSecondsRealtime(delay);
    }
}
