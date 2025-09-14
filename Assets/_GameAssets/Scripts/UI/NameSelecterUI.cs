using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelecterUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _connectButton;

    void Awake()
    {
        _connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(Consts.SceneNames.LOADING_SCENE);
            return;
        }

        _nameInputField.text = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME, string.Empty);
    }

    private void OnConnectButtonClicked()
    {
        PlayerPrefs.SetString(Consts.PlayerData.PLAYER_NAME, _nameInputField.text);
        SceneManager.LoadScene(Consts.SceneNames.LOADING_SCENE);
    }
}
