using System;
using System.Collections.Generic;
using UnityEngine;

public class World : Subject {
    private IDataService dataService = new JsonDataService();
    private int previousScore;

    public static PlayerStats PlayerStats { get; set; }

    public int Score { get; set; } = 0;
    public int localMoney = 0;
    public bool isPlaying = false;
    public bool gameOver = false;

    [field: SerializeField] public int MoveDistance { get; set; } = 1;
    [field: SerializeField] public float Delay { get; set; } = 1.2f;
    [field: SerializeField] public List<PlaySet> PlaySets { get; set; }

    private Player player;

    private bool gameOverWasInvoked = false;

    private void Awake() {
        player = FindObjectOfType<Player>();

        Application.targetFrameRate = 60;

        DeserializeJson();
        //File.Delete(Application.persistentDataPath + "/player-stats.json");

        previousScore = Score;
    }

    private void Update() {
        isPlaying = player.transform.position != new Vector3(0f, player.transform.position.y, 0f) && !gameOver;

        if (!gameOverWasInvoked && player.transform.localPosition.y < -1f) {
            GameOver();
        }

        int bestScore = PlayerStats.BestScore;
        PlayerStats.BestScore = gameOver && Score > bestScore ? Score : bestScore;

        if ((Score > previousScore) && (Score < 10 && Score % 5 == 0) ^ (Score < 100 && Score % 10 == 0) ^ (Score <= 1000 && Score % 50 == 0) ^ (Score > 1000 && Score % 100 == 0)) {
            Delay *= 0.95f;
            previousScore = Score;
        }
    }

    private void GameOver() {
        gameOverWasInvoked = true;
        gameOver = true;
        isPlaying = false;

        NotifyObservers(NotificationType.Death);

        SerializeJson();
    }

    public void SerializeJson() {
        for (int i = 0; i < PlayerStats.SetsInfo.Count; i++) {
            PlayerStats.SetsInfo[i] = PlaySets[i].IsPurchased;
        }

        if (dataService.SaveData(PlayerStats, "/player-stats.json", true)) {
            Debug.Log("Player stats were saved");
        } else {
            Debug.LogError("Couldn't save file!");
        }
    }

    public void DeserializeJson() {
        try {
            PlayerStats = dataService.LoadData<PlayerStats>("/player-stats.json", true);

            for (int i = 0; i < PlayerStats.SetsInfo.Count; i++) {
                PlaySets[i].IsPurchased = PlayerStats.SetsInfo[i];
            }
        } catch {
            Debug.LogError($"Couldn't read file!");
        }
    }
}