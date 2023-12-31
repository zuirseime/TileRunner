using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IObserver {
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI startHint;
    [SerializeField] private TextMeshProUGUI gameOverCurrenScore;
    [SerializeField] private TextMeshProUGUI gameOverBestScore;
    [SerializeField] private TextMeshProUGUI globalMoney;
    [SerializeField] private TextMeshProUGUI localMoney;
    [SerializeField] private TextMeshProUGUI gameOverMoney;

    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryContent;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject speedUp;
    [SerializeField] private GameObject doubleCoins;

    [SerializeField] private Sprite unlockedFrame;
    [SerializeField] private Sprite lockedFrame;

    private World world;
    private Player player;

    private Vector3 inventoryDownPos = Vector3.down * 2000f;

    private float delay;
    private float inventoryMoveDuration = .5f;
    private float speedUpVisibilityDuration = .5f;

    private bool coinsWereDoubled = false;
 
    void Start() {
        world = FindObjectOfType<World>();
        world.AddObserver(this);

        player = FindAnyObjectByType<Player>();

        delay = world.Delay;
        startHint.gameObject.SetActive(true);

        FillInventory();
        inventory.transform.localPosition = inventoryDownPos;

        float lowerFontSize = startHint.fontSize - 4f;
        float upperFontSize = startHint.fontSize + 4f;
        StartCoroutine(TextBlink(startHint, lowerFontSize, upperFontSize, 1f));
    }

    private void OnDisable() {
        //world.RemoveObserver(this);
    }

    void Update() {
        score.text = world.Score.ToString();
        globalMoney.text = World.PlayerStats.Money.ToString();
        gameOverMoney.text = localMoney.text = world.localMoney.ToString();

        if (world.isPlaying) {
            buttonPanel.SetActive(false);
            inventory.SetActive(false);

            StartCoroutine(FadeHintOut(startHint, .25f));
        }

        if (delay != world.Delay) {
            ChangeSpeedUpVisibility();
            delay = world.Delay;
        }
    }

    private void GameOver() {
        Debug.Log("UI's game over");
        startHint.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);
        inventory.SetActive(false);
        gameOverCurrenScore.text = world.Score.ToString();
        gameOverBestScore.text = World.PlayerStats.BestScore.ToString();

        if (coinsWereDoubled || world.localMoney == 0) {
            doubleCoins.GetComponent<Button>().interactable = false;
        }
    }

    private void ChangeSpeedUpVisibility() {
        StartCoroutine(ChangeSpeedUpVisibility(100f));
        StartCoroutine(ChangeSpeedUpVisibility(0f));
    }

    private IEnumerator ChangeSpeedUpVisibility(float destinationAlpha) {
        float elapsedTime = 0f;

        if (destinationAlpha < 0f) destinationAlpha = 0f;
        else if (destinationAlpha > 100f) destinationAlpha = 100f;

        Color iconColor = speedUp.transform.GetChild(0).GetComponent<Image>().color;
        Color textColor = speedUp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color;

        while (elapsedTime < speedUpVisibilityDuration) {
            elapsedTime += Time.deltaTime;

            speedUp.transform.GetChild(0).GetComponent<Image>().color = GetColorLerp(iconColor, destinationAlpha, elapsedTime);
            speedUp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = GetColorLerp(textColor, destinationAlpha, elapsedTime);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSecondsRealtime(speedUpVisibilityDuration * 2f);
    }

    private Color GetColorLerp(Color baseColor, float destinationAlpha, float elapsedTime) => 
        Color.Lerp(baseColor, new Color(baseColor.r, baseColor.g, baseColor.b, destinationAlpha), elapsedTime / speedUpVisibilityDuration);

    private void FillInventory() {
        GameObject playSet, locker, price, frame;

        for (int i = 0; i < world.PlaySets.Count; i++) {
            playSet = Instantiate(inventoryItemPrefab, inventoryContent.transform);
            playSet.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = world.PlaySets[i].Icon;

            //frame = playSet.transform.GetChild(1).gameObject;
            //locker = playSet.transform.GetChild(2).gameObject;
            //price = locker.transform.GetChild(1).gameObject;

            //price.GetComponent<TextMeshProUGUI>().text = world.PlaySets[i].Price.ToString();
            //playSet.GetComponent<Button>().AddEventListener(i, frame, locker, OnChangePlaySet);
            Button button = playSet.transform.GetChild(1).GetComponent<Button>();
            TextMeshProUGUI buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();
            button.AddEventListener(i, button, OnChangePlaySet);
            //playSet.GetComponent<Button>().AddEventListener(i, OnChangePlaySet);

            //if (world.PlaySets[i].IsPurchased || world.PlaySets[i].Price == 0) {
            //    frame.GetComponent<Image>().sprite = unlockedFrame;
            //    Destroy(locker);
            //} else {
            //    frame.GetComponent<Image>().sprite = lockedFrame;
            //}
        }
    }

    /// <summary>
    /// Restarts current scene
    /// </summary>
    public void OnRestart() {
        StartCoroutine(OnRestartCoroutine(.1f));
    }

    private IEnumerator OnRestartCoroutine(float wait) {
        while(world.localMoney != 0) {
            world.localMoney--;
            World.PlayerStats.Money++;
            yield return new WaitForSecondsRealtime(wait);
            wait *= .75f;
        }

        yield return null;
        world.SerializeJson();
        SceneManager.LoadScene($"Scenes/{SceneManager.GetActiveScene().name}");
    }

    public void OnDoubleCoins() {
        if (coinsWereDoubled || world.localMoney == 0)
            return;

        world.localMoney *= 2;

        Debug.Log("Double coins were got");
        coinsWereDoubled = true;
        doubleCoins.GetComponent<Button>().interactable = false;
    }

    public void OnOpenInventory(bool animated = false) {
        //Transform parent = inventory.transform.parent;
        if (animated) {
            Vector3 position = inventory.transform.localPosition;
            Vector3 destination = Vector3.Distance(position, inventoryDownPos) < Vector3.Distance(position, Vector3.zero) ? Vector3.zero : inventoryDownPos;
            StartCoroutine(MoveInventory(destination, position));
        } else {
            bool menuActivity = inventory.activeSelf;
            player.enabled = menuActivity;
            inventory.SetActive(!menuActivity);
        }
    }
    
    private IEnumerator MoveInventory(Vector3 destination, Vector3 position) {
        float elapsedTime = 0f;
        while (elapsedTime < inventoryMoveDuration) {
            elapsedTime += Time.deltaTime;
            inventory.transform.localPosition = Vector3.Lerp(position, destination, elapsedTime / inventoryMoveDuration);
            yield return null;
        }

        yield return null;
        inventory.transform.localPosition = destination;

        player.enabled = Vector3.Distance(destination, inventoryDownPos) < Vector3.Distance(destination, Vector3.zero);
    }

    private void OnChangePlaySet(int id, Button button) {
        PlaySet set = world.PlaySets[id];
        Debug.Log($"Button-{id} was clicked");
        TextMeshProUGUI buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();

        if ((set.IsPurchased || set.Price == 0) && World.PlayerStats.CurrentPlaySet != set.Name) {
            World.PlayerStats.CurrentPlaySet = set.Name;
            buttonText.text = ButtonState.Choose.ToString();
            world.SerializeJson();
            OnRestart();
        } else if (set.IsPurchased && World.PlayerStats.CurrentPlaySet == set.Name) {
            buttonText.text = ButtonState.Chosen.ToString();
        } else {
            if (World.PlayerStats.Money >= set.Price) {
                set.IsPurchased = true;
                World.PlayerStats.Money -= set.Price;
                world.SerializeJson();
            }
        }
    }

    //private void OnChangePlaySet(int id, GameObject frame, GameObject locker) {
    //    PlaySet set = world.PlaySets[id];
    //    Debug.Log($"Button-{id} was clicked");

    //    if (set.IsPurchased || set.Price == 0) {
    //        World.PlayerStats.CurrentPlaySet = set.Name;
    //        world.SerializeJson();
    //        OnRestart();
    //    } else {
    //        if (World.PlayerStats.Money >= set.Price) {
    //            frame.GetComponent<Image>().sprite = unlockedFrame;
    //            set.IsPurchased = true;
    //            World.PlayerStats.Money -= set.Price;
    //            world.SerializeJson();
    //            Destroy(locker);
    //        }
    //    }
    //}

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

    public void OnNotify(NotificationType notification) {
        if (notification == NotificationType.Death)
            GameOver();
    }
}

public static class ButtonExtention {
    public static void AddEventListener<T>(this Button btn, T param, Action<T> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param); });
    }

    public static void AddEventListener<T1, T2>(this Button btn, T1 param1, T2 param2, Action<T1, T2> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param1, param2); });
    }

    public static void AddEventListener<T1, T2, T3>(this Button btn, T1 param1, T2 param2, T3 param3, Action<T1, T2, T3> OnClick) {
        btn.onClick.AddListener(() => { OnClick(param1, param2, param3); });
    }
}