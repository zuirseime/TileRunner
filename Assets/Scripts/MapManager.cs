using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // ToDo: Add a coin tile
public class MapManager : MonoBehaviour {
    [SerializeField] private GameObject prefab;

    [SerializeField] private Dictionary<Vector3, GameObject> tiles = new();

    private Globals globals;
    private GameObject currentTile;
    private GameObject previousTile;
    private GameObject firstTile;
    private Vector3 direction;

    private bool firstMove = true;

    private void OnValidate() {
        if (!prefab.TryGetComponent(out GameTile component))
            prefab = null;
    }

    void Start()
    {
        globals = FindObjectOfType<Globals>();

        firstMove = true;

        StartCoroutine(SpawnTile());
    }

    private void Update() {
        if (globals.GameOver) {
            StartCoroutine(RevealMap());
        }
    }

    private IEnumerator RevealMap() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    private IEnumerator SpawnTile()
    {
        if (firstMove)
        {
            firstMove = false;

            Vector3 zeroPos = Vector3.zero;
            firstTile = currentTile = AddTileToDictionary(zeroPos);

            yield return new WaitForSecondsRealtime(globals.Delay);

            ChangeCurrentTile();
        }

        while (!globals.GameOver)
        {
            if (globals.IsStarted) {
                if (firstTile.activeSelf)
                    firstTile.GetComponent<GameTile>().Hide();

                ChangeCurrentTile();

                previousTile!.GetComponent<GameTile>().Hide();
            }
        }
    }

    private IEnumerator ChangeCurrentTile() {
        direction = ChooseDirection();

        Vector3 currentTilePos = currentTile.transform.position;
        Vector3 newTilePos = currentTilePos + direction;

        currentTile = AddTileToDictionary(newTilePos);
        yield return new WaitForSecondsRealtime(globals.Delay);
    }

    private Vector3 ChooseDirection()
    {
        int axis = Random.Range(0, 2); // 0 for X, 1 for Z

        float move = Mathf.Sign(Random.Range(-1f, 1f));

        Vector3 newDirection;

        if (axis == 0)
            newDirection = (Vector3.right * move).normalized;
        else
            newDirection = (Vector3.forward * move).normalized;

        if (globals.IsStarted)
            if (currentTile.transform.position + newDirection * globals.MoveDistance == previousTile?.transform.position)
                return ChooseDirection();
            else return newDirection;
        else return newDirection;
    }

    private GameObject AddTileToDictionary(Vector3 position)
    {
        GameObject tile;

        previousTile = currentTile;

        if (!tiles.ContainsKey(position))
        {
            tile = Instantiate(prefab, position, Quaternion.identity, transform);
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
