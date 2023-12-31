using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tile : MonoBehaviour {
    private Color[] baseColors;
    private Color[] targetColors;

    private float startTime;
    private float fadeDuration;

    private World world;

    void Start() {
        world = FindObjectOfType<World>();

        gameObject.layer = LayerMask.NameToLayer("Tile");

        baseColors = new Color[GetComponent<Renderer>().materials.Length];
        targetColors = new Color[baseColors.Length];

        for (int i = 0; i < GetComponent<Renderer>().materials.Length; i++) {
            baseColors[i] = GetComponent<Renderer>().materials[i].color;
            targetColors[i] = new Color(baseColors[i].r / 2f, baseColors[i].g / 2f, baseColors[i].b / 2f, 0f);
        }
    }

    void Update() {
        fadeDuration = world.Delay / 2f;
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

            for (int i = 0; i < baseColors.Length; i++) {
                Color newColor = Color.Lerp(baseColors[i], targetColors[i], normalizeTime);
                GetComponent<Renderer>().materials[i].color = newColor;
            }

            yield return null;
        }
        GetComponent<Collider>().enabled = false;

        yield return null;
        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;

        for (int i = 0; i < baseColors.Length; i++) {
            GetComponent<Renderer>().materials[i].color = baseColors[i];
        }
    }
}
