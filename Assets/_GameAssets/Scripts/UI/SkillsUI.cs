
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance { get; private set; }
    [Header("Skill References")]
    [SerializeField] private Image _skillIconImage;
    [SerializeField] private TMP_Text _skillNameText;
    [SerializeField] private TMP_Text _skillCounterText;
    [SerializeField] private Transform _skillCounterBackgroundTransform;
    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetSkillToNone();

        _skillCounterBackgroundTransform.localScale = Vector3.zero;
        _skillCounterBackgroundTransform.gameObject.SetActive(false);
    }
    public void SetSkill(String skillName, Sprite skillSprite, SkillUsageType skillUsageType, int timerCounter)
    {
        _skillIconImage.gameObject.SetActive(true);
        _skillNameText.text = skillName;
        _skillIconImage.sprite = skillSprite;

        if (skillUsageType == SkillUsageType.Timer || skillUsageType == SkillUsageType.Amount)
        {
            SetTimerCounterAnimation(timerCounter);
        }


    }

    public void SetTimerCounterAnimation(int timerCounter)
    {
        if (_skillCounterBackgroundTransform.gameObject.activeInHierarchy) { return; }
        _skillCounterBackgroundTransform.gameObject.SetActive(true);
        _skillCounterBackgroundTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _skillCounterText.text = timerCounter.ToString();

    }

    public void SetSkillToNone()
    {
        _skillIconImage.gameObject.SetActive(false);
        _skillNameText.text = string.Empty;

        if (_skillCounterBackgroundTransform.gameObject.activeInHierarchy)
        {
            _skillCounterBackgroundTransform.gameObject.SetActive(false);
        }

    }
    
    public void SetTimerCounterText(int timerCounter)
    {
        _skillCounterText.text = timerCounter.ToString();
    }
}
