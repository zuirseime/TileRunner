using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Globals : MonoBehaviour {
    private IDataService dataService = new JsonDataService();
    private int previousScore;
    private Player player;

    public static PlaySetName CurrentPlaySet { get; set; }
    public static int MoneyCount { get; set; }
    public static int BestScore { get; set; }

    public int Score { get; set; } = 0;
    public bool IsStarted { get; set; } = false;
    public bool GameOver { get; set; } = false;

    [field: SerializeField] public int MoveDistance { get; set; } = 1;
    [field: SerializeField] public float Delay { get; set; } = 1.2f;
    [field: SerializeField] public List<PlaySet> PlaySets { get; set; }

    [SerializeField] private PlayerStats playerStats;

    private void Awake() {
        Application.targetFrameRate = 60;

        player = FindObjectOfType<Player>();

        DeserializeJson();

        CurrentPlaySet = playerStats.CurrentPlaySet;
        MoneyCount = playerStats.Money;
        BestScore = playerStats.BestScore;
        for (int i = 0; i < playerStats.SetsInfo.Count; i++) {
            PlaySets[i].IsPurchased = playerStats.SetsInfo[i];
        }

        previousScore = Score;
    }

    private void Update() {
        if (Score != 0)
            IsStarted = true;

        if (player.transform.position.y < -1f) {
            GameOver = true;
            FindObjectOfType<AudioManager>().Play(SoundName.Death);

            playerStats.CurrentPlaySet = CurrentPlaySet;
            playerStats.Money = MoneyCount;
            playerStats.BestScore = BestScore;
            for (int i = 0; i < playerStats.SetsInfo.Count; i++) {
                playerStats.SetsInfo[i] = PlaySets[i].IsPurchased;
            }

            SerializeJson();
        }

        if (Score > BestScore) {
            BestScore = Score;
        }

        if (Score > previousScore && Score % 5 == 0) {
            Delay *= 0.95f;
            previousScore = Score;
        }
    }

    public void SerializeJson() {
        if (dataService.SaveData(playerStats, "/player-stats.json", true)) {
            Debug.Log("Player stats were saved");
        } else {
            Debug.LogError("Couldn't save file!");
        }
    }

    public void DeserializeJson() {
        try {
            playerStats = dataService.LoadData<PlayerStats>("/player-stats.json", true);
        } catch (Exception ex) {
            Debug.LogError($"Couldn't read file!");
        }
    }
}