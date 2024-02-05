using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats {
    [field: SerializeField] public PlaySetName CurrentPlaySet { get; set; }
    [field: SerializeField] public int Money { get; set; }
    [field: SerializeField] public int BestScore { get; set; }
    public Dictionary<int, bool> SetsInfo { get; set; }

    public static PlayerStats Default => new PlayerStats() {
        CurrentPlaySet = PlaySetName.Standard,
        Money = 0,
        BestScore = 0,
        SetsInfo = new Dictionary<int, bool>() {
            { 0, true },
            { 1, false },
            { 2, false },
            { 3, false },
            { 4, false },
        }
    };
}