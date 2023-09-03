using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GameTile : MonoBehaviour {
    public bool HasCoin { get; private set; } = false;

    private Color[] baseColors;
    private Color[] targetColors;

    private float startTime;
    private float fadeDuration;

    private Globals globals;
    private GameObject coinSkin;
    private GameObject coin;

    void Start() {
        globals = FindObjectOfType<Globals>();

        gameObject.layer = LayerMask.NameToLayer("Tile");

        baseColors = new Color[GetComponent<Renderer>().materials.Length];
        targetColors = new Color[baseColors.Length];

        coinSkin = globals.PlaySets.Find(n => n.Name == Globals.CurrentPlaySet).Coin;

        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++) {
            baseColors[i] = GetComponent<Renderer>().materials[i].color;
            targetColors[i] = new Color(baseColors[i].r / 2f, baseColors[i].g / 2f, baseColors[i].b / 2f, 0f);
        }
    }

    public bool TrySpawnCoin(int chance) {
        if (Random.Range(0, 100) < chance) {
            coin = Instantiate(coinSkin, transform.localPosition, transform.rotation, transform);
            HasCoin = true;
            return true;
        }

        return false;
    }

    void Update() {
        fadeDuration = globals.Delay / 2f;
    }

    /// <summary>
    /// Hides the tile
    /// </summary>
    public void Hide() => StartCoroutine(FadeOut());

    /// <summary>
    /// Smoothly makes the tile fades out
    /// </summary>
    /// <returns>Nothing</returns>
    private IEnumerator FadeOut() {
        startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime = Time.time - startTime;
            float normalizeTime = elapsedTime / fadeDuration;

            for (int i = 0; i < baseColors.Length; i++)
            {
                Color newColor = Color.Lerp(baseColors[i], targetColors[i], normalizeTime);
                GetComponent<Renderer>().materials[i].color = newColor;
            }

            yield return null;
        }
        GetComponent<Collider>().enabled = false;

        yield return null;
        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;

        for (int i = 0; i < baseColors.Length; i++)
        {
            GetComponent<Renderer>().materials[i].color = baseColors[i];
        }
    }
}
