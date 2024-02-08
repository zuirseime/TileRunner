using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour, IObserver {
    private Dictionary<Vector3, GameObject> map = new();

    [SerializeField] private Player player;
    private World world;

    private GameObject currentTile;

    private Vector3 direction;
    private bool firstMove = true;

    private GameObject tileSkin;
    private GameObject coinSkin;

    [SerializeField] private int leftToCoin;
    [SerializeField] private int coinInterval = 3;

    [Header("Chance settings")]
    [SerializeField, Range(0, 100)] private int baseCoinChance = 40; // Normal - 35-40
    [SerializeField] private int coinChance;
    [SerializeField, Range(0, 100)] private int intervalRestoreChance = 75; // Normal - 75
    private int chanceIncreaseFactor = 40;

    private void Start() {
        world = FindObjectOfType<World>();
        world.AddObserver(this);

        PlaySet set = world.PlaySets.Find(n => n.Name == World.PlayerStats.CurrentPlaySet);
        (tileSkin, coinSkin) = (set.Tile, set.Coin);

        leftToCoin = coinInterval;
        coinChance = baseCoinChance;

        SpawnTile(Vector3.zero, false);
        SpawnTile(RandomDirection(), firstMove);
    }

    private void Update() {
        if (!firstMove && Vector3.Distance(player.transform.position, currentTile.transform.position) < 0.05f) {
            direction = RandomDirection();

            Vector3 newPosition = currentTile.transform.position + direction * world.MoveDistance;

            SpawnTile(newPosition);
        }
    }

    private void OnValidate() {
        if (coinChance < baseCoinChance)
            coinChance = baseCoinChance;
        else if (coinChance > 100)
            coinChance = 100;
    }

    private Vector3 RandomDirection() {
        List<Vector3> directions = new() {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        return directions[Random.Range(0, 4)];
    }

    private void SpawnTile(Vector3 position, bool firstMove = false) {
        GameObject tile;

        if (!map.ContainsKey(position)) {
            tile = Instantiate(tileSkin, position, Quaternion.identity, transform);
            tile.name = $"Tile [{position.x}, {position.z}]";
            map.Add(position, tile);
        } else {
            tile = map[position];
            tile.gameObject.SetActive(true);
        }

        currentTile = tile;

        if (!firstMove) {
            foreach (var pair in map) {
                if (pair.Value != currentTile && pair.Value.activeSelf) {
                    pair.Value.GetComponent<Tile>().Hide();
                }
            }
        }

        if (firstMove)
            this.firstMove = false;

        TrySpawnCoin(position);
    }

    private void TrySpawnCoin(Vector3 position) {
        if (leftToCoin == 0) {
            Instantiate(coinSkin, position, Quaternion.identity, transform);

            if (Random.Range(0, 100) < intervalRestoreChance) {
                coinChance = baseCoinChance;
                leftToCoin = coinInterval;
            } else if (coinChance < 100 - chanceIncreaseFactor)
                coinChance += chanceIncreaseFactor;
        } else if (leftToCoin > 0)
            leftToCoin--;
    }

    private void GameOver() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OnNotify(NotificationType notification) {
        if (notification == NotificationType.Death)
            GameOver();
    }
}