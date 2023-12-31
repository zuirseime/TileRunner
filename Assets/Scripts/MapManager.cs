using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ToDo: Add a coin tile
public class MapManager : MonoBehaviour, IObserver {
    private Dictionary<Vector3, GameObject> tiles = new();

    private World world;
    private GameObject currentTile;
    private GameObject previousTile;
    private GameObject firstTile;
    private Vector3 direction;

    private GameObject tileSkin;
    private GameObject coinSkin;

    //private bool firstMove = true;

    [SerializeField] private int leftToCoin;
    [SerializeField] private int coinInterval = 3;

    [Header("Chance settings")]
    [SerializeField, Range(0, 100)] private int baseCoinChance = 40; // Normal - 35-40
    [SerializeField] private int coinChance;
    [SerializeField, Range(0, 100)] private int intervalRestoreChance = 75; // Normal - 75
    private int chanceIncreaseFactor = 40;

    private void OnValidate() {
        if (coinChance < baseCoinChance)
            coinChance = baseCoinChance;
        else if (coinChance > 100)
            coinChance = 100;
    }

    void Start()
    {
        world = FindObjectOfType<World>();
        world.AddObserver(this);

        PlaySet set = world.PlaySets.Find(n => n.Name == World.PlayerStats.CurrentPlaySet);

        (tileSkin, coinSkin) = (set.Tile, set.Coin);

        leftToCoin = coinInterval;
        coinChance = baseCoinChance;

        StartCoroutine(SpawnTile());
    }

    private void Update() {
        
    }

    private void GameOver() {
        Debug.Log("Map's game over");

        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Spawns a new tile
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnTile()
    {
        if (!world.isPlaying)
        {
            //firstMove = false;

            Vector3 zeroPos = Vector3.zero;
            firstTile = currentTile = SpawnTile(zeroPos);

            yield return null;

            StartCoroutine(ChangeCurrentTile());
        }

        while (!world.gameOver)
        {
            if (world.isPlaying) {
                if (firstTile.activeSelf)
                    firstTile.GetComponent<Tile>().Hide();

                StartCoroutine(ChangeCurrentTile());

                previousTile!.GetComponent<Tile>().Hide();
            }

            yield return new WaitForSecondsRealtime(world.Delay);
        }
    }

    /// <summary>
    /// Changes the previous tile to a new one
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeCurrentTile() {
        direction = ChooseDirection();

        Vector3 currentTilePos = currentTile.transform.position;
        Vector3 newTilePos = currentTilePos + direction;

        currentTile = SpawnTile(newTilePos);
        SpawnCoin(newTilePos);

        yield return new WaitForSecondsRealtime(world.Delay);
    }

    /// <summary>
    /// Chooses the direction of a new tile spawning
    /// </summary>
    /// <returns>Spawning direction</returns>
    private Vector3 ChooseDirection()
    {
        Vector3 newDirection;

        while(true) {
            int axis = Random.Range(0, 2); // 0 for X, 1 for Z

            float move = Mathf.Sign(Random.Range(-1f, 1f));

            if (axis == 0)
                newDirection = (Vector3.right * move).normalized * world.MoveDistance;
            else
                newDirection = (Vector3.forward * move).normalized * world.MoveDistance;

            if (currentTile.transform.position + newDirection == previousTile?.transform.position)
                continue;

            return newDirection;
        }
    }

    /// <summary>
    /// Spawns a new tile and adds it to the tile dictionary
    /// </summary>
    /// <param name="position">Key of the tile and position where the tile has to be spawned</param>
    /// <returns>A new tile</returns>
    private GameObject SpawnTile(Vector3 position)
    {
        GameObject tile;

        previousTile = currentTile;

        if (!tiles.ContainsKey(position))
        {
            tile = Instantiate(tileSkin, position, Quaternion.identity, transform);
            tile.name = $"Tile [{position.x}, {position.z}]";
            tiles.Add(position, tile);
        }
        else
        {
            tile = tiles[position];
            tile.SetActive(true);
        }

        return tile;
    }

    private void SpawnCoin(Vector3 position) {

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

    public void OnNotify(NotificationType notification) {
        if (notification == NotificationType.Death)
            GameOver();
    }
}
