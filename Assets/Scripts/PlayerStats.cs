using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats {
    [field: SerializeField] public PlaySetName CurrentPlaySet { get; set; } = PlaySetName.Standard;
    [field: SerializeField] public int Money { get; set; } = 0;
    [field: SerializeField] public int BestScore { get; set; } = 0;
    public Dictionary<int, bool> SetsInfo { get; set; } = new Dictionary<int, bool>() {
        { 0, true },
        { 1, false },
        { 2, false },
        { 3, false },
        { 4, false },
    };
}