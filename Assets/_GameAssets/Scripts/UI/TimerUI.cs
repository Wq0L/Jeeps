using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance { get; private set; }
    [SerializeField] private TMP_Text _timerText;

    void Awake()
    {
        Instance = this;
    }

    public void SetTimerUI(int timerCounter)
    {
        _timerText.text = timerCounter.ToString();
    }
}
