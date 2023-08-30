using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.0f;


    private Color baseColor;
    private Color targetColor;

    private float startTime;

    void Start()
    {
        baseColor = GetComponent<Renderer>().material.color;
        targetColor = new Color(0f, 0f, 0f, 0f);
    }

    void Update()
    {
        
    }

    public void Hide()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        GetComponent<Collider>().enabled = false;

        startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime = Time.time - startTime;
            float normalizeTime = elapsedTime / fadeDuration;

            Color newColor = Color.Lerp(baseColor, targetColor, normalizeTime);

            GetComponent<Renderer>().material.color = newColor;

            yield return null;
        }

        GetComponent<Renderer>().material.color = targetColor;
        yield return null;

        gameObject.SetActive(false);
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>().material.color = baseColor;
    }
}
