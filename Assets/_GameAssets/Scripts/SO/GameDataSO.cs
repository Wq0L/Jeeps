using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "ScriptableObjects/Game Data")]
public class GameDataSO : ScriptableObject
{
    [SerializeField] private int _gameTimer;

    public int GameTimer => _gameTimer;
}
