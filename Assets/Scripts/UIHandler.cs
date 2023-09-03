using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI startHint;
    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject playSetMenu;
    [SerializeField] private TextMeshProUGUI money;

    private Globals globals;
 
    void Start() {
        globals = FindObjectOfType<Globals>();

        startHint.gameObject.SetActive(true);

        FillPlaySetMenu();

        StartCoroutine(TextBlink(startHint, 20f, 28f, 1f));
    }

    void Update() {
        score.text = globals.Score.ToString();
        money.text = Globals.MoneyCount.ToString();

        if (globals.GameOver) {
            restartButton.gameObject.SetActive(true);
            startHint.gameObject.SetActive(false);
        }

        if (globals.IsStarted)
            StartCoroutine(FadeHintOut(startHint, .25f));
    }

    private void FillPlaySetMenu() {
        UnityEngine.GameObject template = playSetMenu.transform.GetChild(0).gameObject;
        UnityEngine.GameObject playSet;

        for (int i = 0; i < globals.PlaySets.Count; i++) {
            playSet = Instantiate(template, playSetMenu.transform);
            playSet.transform.GetChild(0).GetComponent<Image>().sprite = globals.PlaySets[i].Icon;

            // playSet.GetComponent<Button>().onClick.AddListener(delegate() { OnChangePlaySet(i); });
            playSet.GetComponent<Button>().AddEventListener(i, OnChangePlaySet);
        }

        Destroy(template);
    }

    /// <summary>
    /// Restarts current scene
    /// </summary>
    public void OnRestart() {
        SceneManager.LoadScene($"Scenes/{SceneManager.GetActiveScene().name}");
    }

    public void OnOpenPlaySetMenu() {
        bool menuActivity = playSetMenu.activeSelf;
        playSetMenu.SetActive(!menuActivity);
    }

    private void OnChangePlaySet(int id) {
        Debug.Log($"Item {id} was selected");
        Globals.CurrentPlaySet = (PlaySetName)id; 
        OnRestart();
    }

    /// <summary>
    /// Smoothly makes the text blinks
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="minSize">The minimum size value</param>
    /// <param name="maxSize">The maximum size velue</param>
    /// <param name="duration">Change duration (in seconds)</param>
    /// <returns></returns>
    private IEnumerator TextBlink(TextMeshProUGUI hint, float minSize, float maxSize, float duration) {
        while (true) {
            StartCoroutine(ChangeTextSize(hint, minSize, maxSize, duration));
            yield return new WaitForSecondsRealtime(duration);
            StartCoroutine(ChangeTextSize(hint, maxSize, minSize, duration));
            yield return new WaitForSecondsRealtime(duration);
        }
    }

    /// <summary>
    /// Smoothly changes text size
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="v1">The start value</param>
    /// <param name="v2">The end value</param>
    /// <param name="t">Change duration</param>
    /// <returns></returns>
    private IEnumerator ChangeTextSize(TextMeshProUGUI hint, float v1, float v2, float t) {
        float elapsedTime = 0f;
        while (elapsedTime < t) {
            float newSize = Mathf.Lerp(v1, v2, elapsedTime / t);
            hint.fontSize = newSize;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hint.fontSize = v2;
    }

    /// <summary>
    /// Hides the hint
    /// </summary>
    /// <param name="hint">Text reference</param>
    /// <param name="duration">Fade duration</param>
    /// <returns></returns>
    private IEnumerator FadeHintOut(TextMeshProUGUI hint, float duration) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            hint.color = new Color(hint.material.color.r, hint.material.color.g, hint.material.color.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hint.color = new Color(hint.material.color.r, hint.material.color.g, hint.material.color.b, 0f);

        yield return new WaitForSecondsRealtime(1f);
        hint.gameObject.SetActive(false);
    }
}

public static class ButtonExtention {
    public static void AddEventListener<T>(this Button btn, T param, Action<T> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param); });
    }
}