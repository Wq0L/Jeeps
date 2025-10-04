using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR;

public class LeaderboardUI : NetworkBehaviour
{
    [SerializeField] private LeaderboardRanking _leaderboardRankingPrefab;
    [SerializeField] private Transform _rankingsParent;
    [SerializeField] private TMP_Text _rankText;
    private NetworkList<LeaderboardEntitiesSerialzable> _leaderboardEntityList;
    private List<LeaderboardRanking> _leaderboardRankingList = new List<LeaderboardRanking>();

    void Awake()
    {
        _leaderboardEntityList = new NetworkList<LeaderboardEntitiesSerialzable>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _leaderboardEntityList.OnListChanged += HandleLeaderboardEntitiseChanged;
            foreach (LeaderboardEntitiesSerialzable entity in _leaderboardEntityList)
            {
                HandleLeaderboardEntitiseChanged(new NetworkListEvent<LeaderboardEntitiesSerialzable>
                {
                    Type = NetworkListEvent<LeaderboardEntitiesSerialzable>.EventType.Add,
                    Value = entity
                });
            }

        }
        if (IsServer)
        {
            PlayerNetworkController[] players = FindObjectsByType<PlayerNetworkController>(FindObjectsSortMode.None);
            foreach (PlayerNetworkController player in players)
            {
                HandlePlayerSpawned(player);
            }
            PlayerNetworkController.OnPlayerSpawned += HandlePlayerSpawned;
            PlayerNetworkController.OnPlayerDespawned += HandlePlayerDespawned;

        }
    }

    private void HandleLeaderboardEntitiseChanged(NetworkListEvent<LeaderboardEntitiesSerialzable> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntitiesSerialzable>.EventType.Add:
                if (!_leaderboardRankingList.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderboardRanking leaderboardRankingInstance
                     = Instantiate(_leaderboardRankingPrefab, _rankingsParent);

                    leaderboardRankingInstance.SetData(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Score
                        );

                    _leaderboardRankingList.Add(leaderboardRankingInstance);
                }
                UpdatePlayerRankText();

                break;

            case NetworkListEvent<LeaderboardEntitiesSerialzable>.EventType.Remove:
                LeaderboardRanking leaderboardRankingToRemove
        = _leaderboardRankingList.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (leaderboardRankingToRemove != null)
                {
                    leaderboardRankingToRemove.transform.SetParent(null);
                    Destroy(leaderboardRankingToRemove.gameObject);
                    _leaderboardRankingList.Remove(leaderboardRankingToRemove);
                    UpdatePlayerRankText();
                }
                break;

            case NetworkListEvent<LeaderboardEntitiesSerialzable>.EventType.Value:
                LeaderboardRanking leaderboardRankingToUpdate
                    = _leaderboardRankingList.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (leaderboardRankingToUpdate != null)
                {
                    leaderboardRankingToUpdate.UpdateScore(changeEvent.Value.Score);
                }
                break;
        }

        UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        _leaderboardRankingList.Sort((x, y) => y.Score.CompareTo(x.Score));

        for (int i = 0; i < _leaderboardRankingList.Count; i++)
        {
            _leaderboardRankingList[i].transform.SetSiblingIndex(i);
            _leaderboardRankingList[i].UpdateOrder();
        }

        UpdatePlayerRankText();
    }

    private void HandlePlayerSpawned(PlayerNetworkController playerNetworkController)
    {
        _leaderboardEntityList.Add(new LeaderboardEntitiesSerialzable
        {
            ClientId = playerNetworkController.OwnerClientId,
            PlayerName = playerNetworkController.PlayerName.Value,
            Score = 0
        });

        playerNetworkController.GetPlayerScoreController().PlayerScore.OnValueChanged
            += (oldScore, newScore) =>
            HandleScoreChanged(playerNetworkController.OwnerClientId, newScore);
    }


    private void HandleScoreChanged(ulong clientId, int newScore)
    {
        for (int i = 0; i < _leaderboardEntityList.Count; ++i)
        {
            if (_leaderboardEntityList[i].ClientId != clientId) { continue; }

            _leaderboardEntityList[i] = new LeaderboardEntitiesSerialzable
            {
                ClientId = _leaderboardEntityList[i].ClientId,
                PlayerName = _leaderboardEntityList[i].PlayerName,
                Score = newScore,
            };

            UpdatePlayerRankText();

            return;
        }
    }
    private void UpdatePlayerRankText()
    {
        LeaderboardRanking myRanking = _leaderboardRankingList
            .FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myRanking == null)
        {
            return;
        }
        int rank = myRanking.transform.GetSiblingIndex() + 1;
        string rankSuffix = GetRankSuffix(rank);

        _rankText.text = $"{rank}<sup>{rankSuffix}</sup>/{_leaderboardRankingList.Count}";
    }
    private string GetRankSuffix(int rank)
    {
        return rank switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }

    public List<LeaderboardEntitiesSerialzable> GetLeaderboardData()
    {
        List<LeaderboardEntitiesSerialzable> leaderboardData = new List<LeaderboardEntitiesSerialzable>();

        foreach (var entity in _leaderboardEntityList)
        {
            leaderboardData.Add(entity);
        }

        return leaderboardData;
    }


    public string GetWinnersName()
    {
        if (_leaderboardEntityList.Count > 0)
        {
            return _leaderboardRankingList[0].GetPlayerName();
        }

        return "No Winner";
        
    }

    private void HandlePlayerDespawned(PlayerNetworkController playerNetworkController)
    {
        if (_leaderboardEntityList == null) { return; }
        foreach (LeaderboardEntitiesSerialzable entity in _leaderboardEntityList)
        {
            if (entity.ClientId != playerNetworkController.OwnerClientId) { continue; }
            _leaderboardEntityList.Remove(entity);
            break;
        }

        playerNetworkController.GetPlayerScoreController().PlayerScore.OnValueChanged
            -= (oldScore, newScore) =>
            HandleScoreChanged(playerNetworkController.OwnerClientId, newScore);
    }
    

   public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _leaderboardEntityList.OnListChanged -= HandleLeaderboardEntitiseChanged;
        }
        if (IsServer)
        {
            PlayerNetworkController.OnPlayerSpawned -= HandlePlayerSpawned;
            PlayerNetworkController.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    } 
}
