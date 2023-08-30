using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

    // ToDo: Add a coin tile
public class TileGenerator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private Dictionary<Vector3, GameObject> tiles = new();

    [SerializeField] private int spawnDistance = 2;

    private GameObject currentTile;
    private GameObject previousTile;
    private Vector3 direction;

    public static bool GameOver { get; set; } = false;
    private bool firstMove = true;

    private void OnValidate()
    {
        if (!prefab.TryGetComponent(out GameTile component))
            prefab = null;
    }

    void Start()
    {
        StartCoroutine(SpawnTile(1f));
    }

    private IEnumerator SpawnTile(float delay)
    {
        if (firstMove)
        {
            firstMove = false;

            Vector3 zeroPos = Vector3.zero;
            currentTile = AddTileToDictionary(zeroPos);

            yield return new WaitForSecondsRealtime(delay * 2f);
        }

        while (!GameOver)
        {
            direction = ChooseDirection();

            Vector3 currentTilePos = currentTile.transform.position;
            Vector3 newTilePos = currentTilePos + direction;

            currentTile = AddTileToDictionary(newTilePos);

            yield return new WaitForSecondsRealtime(delay);

            previousTile!.GetComponent<GameTile>().Hide();
        }
    }

    private Vector3 ChooseDirection()
    {
        int axis = Random.Range(0, 2); // 0 for X, 1 for Z

        float move = Mathf.Sign(Random.Range(-1f, 1f));

        Vector3 newDirection;

        if (axis == 0)
            newDirection = (Vector3.right * move).normalized * spawnDistance;
        else
            newDirection = (Vector3.forward * move).normalized * spawnDistance;

        if (currentTile.transform.position + newDirection == previousTile?.transform.position)
            return ChooseDirection();
        
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
