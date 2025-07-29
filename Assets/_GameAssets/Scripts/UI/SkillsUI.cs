
using System;
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

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetSkillToNone();
    }
    public void SetSkill(String skillName, Sprite skillSprite)
    {
        _skillIconImage.gameObject.SetActive(true);
        _skillNameText.text = skillName;
        _skillIconImage.sprite = skillSprite;
    }

    public void SetSkillToNone()
    {
        _skillIconImage.gameObject.SetActive(false);
        _skillNameText.text = string.Empty;

    }
}
