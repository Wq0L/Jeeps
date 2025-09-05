
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _gameOverBackgroundImage;
    [SerializeField] private RectTransform _gameOverTransform;
    [SerializeField] private RectTransform _scoreTableTransform;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private Button _mainMenuButton;
    [Header("Settings")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _scaleDuration;

    private RectTransform _mainMenuButtonTransform;
    private RectTransform _winnerTextTransform;

    void Awake()
    {
        _mainMenuButtonTransform = _mainMenuButton.GetComponent<RectTransform>();
        _winnerTextTransform = _winnerText.GetComponent<RectTransform>();
    }

    void Start()
    {
        _scoreTableTransform.gameObject.SetActive(false);
        _scoreTableTransform.localScale = Vector3.zero;


        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;

    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.GameOver)
        {
            AnimateGameOver();
        }
    }

    private void AnimateGameOver()
    {
        _gameOverBackgroundImage.DOFade(0.8f, _animationDuration / 2);
        _gameOverTransform.DOAnchorPosY(0f ,_animationDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _gameOverTransform.GetComponent<TMP_Text>().DOFade(0f, _animationDuration / 2).SetDelay(1f).OnComplete(() =>
            {
               AnimateLeaderboardAndButtons();
            });
        });
    }

    private void AnimateLeaderboardAndButtons()
    {
        _scoreTableTransform.gameObject.SetActive(true);
        _scoreTableTransform.DOScale(0.8f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _mainMenuButtonTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _winnerTextTransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
            });
        });
    }
}
