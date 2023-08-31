using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        tmp.text = MovementController.Score.ToString();
    }
}
