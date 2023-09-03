using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ToDo: Add a coin tile
public class MapManager : MonoBehaviour {
    [SerializeField] private Dictionary<Vector3, GameObject> tiles = new();

    private Globals globals;
    private GameObject currentTile;
    private GameObject previousTile;
    private GameObject firstTile;
    private Vector3 direction;
    private MapState mapState = MapState.Playable;

    private GameObject tileSkin;

    private bool firstMove = true;

    private void OnValidate() {
        
    }

    void Start()
    {
        globals = FindObjectOfType<Globals>();

        tileSkin = globals.PlaySets.Find(n => n.Name == Globals.CurrentPlaySet).Tile;

        StartCoroutine(SpawnTile());
    }

    private void Update() {
        if (globals.GameOver) {
            CheckMapState();
        }
    }

    /// <summary>
    /// Checks the map state and performs certain actions depending on its value
    /// </summary>
    private void CheckMapState() {
        bool[] tilesActivities = new bool[tiles.Count];

        if (tiles.Count > 0) {
            if (mapState == MapState.Playable) {
            for (int i = 0; i < tiles.Count; i++) {
                transform.GetChild(i).gameObject.SetActive(false);
                tilesActivities[i] = transform.GetChild(i).gameObject.activeSelf;
            }
            if (!tilesActivities.Contains(true))
                mapState = MapState.Hidden;
        } else if (mapState == MapState.Hidden) {
            if (!transform.GetChild(tiles.Count - 1).gameObject.activeSelf)
                StartCoroutine(RevealMap());
        }
        }
    }

    /// <summary>
    /// Reveals map after the game over
    /// </summary>
    /// <returns></returns>
    private IEnumerator RevealMap() {
        for (int i = 0; i < tiles.Count; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(.1f);
        }
        mapState = MapState.Revealed;
    }

    /// <summary>
    /// Spawns a new tile
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnTile()
    {
        if (firstMove)
        {
            firstMove = false;

            Vector3 zeroPos = Vector3.zero;
            firstTile = currentTile = AddTileToDictionary(zeroPos);

            yield return new WaitForSecondsRealtime(globals.Delay);

            StartCoroutine(ChangeCurrentTile());
        }

        while (!globals.GameOver)
        {
            if (globals.IsStarted) {
                if (firstTile.activeSelf)
                    firstTile.GetComponent<GameTile>().Hide();

                StartCoroutine(ChangeCurrentTile());

                previousTile!.GetComponent<GameTile>().Hide();
            }

            yield return new WaitForSecondsRealtime(globals.Delay);
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

        currentTile = AddTileToDictionary(newTilePos);

        yield return new WaitForSecondsRealtime(globals.Delay);
    }

    /// <summary>
    /// Chooses the direction of a new tile spawning
    /// </summary>
    /// <returns>Spawning direction</returns>
    private Vector3 ChooseDirection()
    {
        Vector3 newDirection = Vector3.zero;

        while(true) {
            int axis = Random.Range(0, 2); // 0 for X, 1 for Z

            float move = Mathf.Sign(Random.Range(-1f, 1f));

            if (axis == 0)
                newDirection = (Vector3.right * move).normalized * globals.MoveDistance;
            else
                newDirection = (Vector3.forward * move).normalized * globals.MoveDistance;

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
    private GameObject AddTileToDictionary(Vector3 position)
    {
        GameObject tile;

        previousTile = currentTile;

        if (!tiles.ContainsKey(position))
        {
            tile = Instantiate(tileSkin, position, Quaternion.identity, transform);
            tile.name = $"Tile [{position.x}|{position.z}]";
            tiles.Add(position, tile);
        }
        else
        {
            tile = tiles[position];
            tile.SetActive(true);
        }

        return tile;
    }
}
