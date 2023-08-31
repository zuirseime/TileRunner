using System.Collections;
using UnityEngine;

public class GameTile : MonoBehaviour {
    private Color baseColor;
    private Color targetColor;

    private float startTime;
    private float fadeDuration;

    private Globals globals;

    void Start() {
        globals = FindObjectOfType<Globals>();

        baseColor = Random.ColorHSV(0 / 360f, 360 / 360f, 75 / 100f, 75 / 100f, 100 / 100f, 100 / 100f);
        GetComponent<Renderer>().material.color = baseColor;

        targetColor = new Color(baseColor.r / 2f, baseColor.g / 2f, baseColor.b / 2f, 0f);
    }

    void Update() {
        fadeDuration = globals.Delay / 2f;
    }

    public void Hide() => StartCoroutine(FadeOut());

    private IEnumerator FadeOut() {
        startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime = Time.time - startTime;
            float normalizeTime = elapsedTime / fadeDuration;

            Color newColor = Color.Lerp(baseColor, targetColor, normalizeTime);

            GetComponent<Renderer>().material.color = newColor;

            yield return null;
        }

        GetComponent<Collider>().enabled = false;

        yield return null;
        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().material.color = baseColor;
    }
}
