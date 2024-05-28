using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class GameScene : MonoBehaviourPun
{
    [SerializeField] private float totalTime = 10f;
    
    private GameObject _player;
    private int _activePlayerCount;

    private void Awake()
    {
        Managers.Game.Init();
    }

    private void Start()
    {
        LoadPlayer();
        _activePlayerCount = PhotonNetwork.CurrentRoom.Players.Count;
        StartCoroutine(CoCountDown());
    }

    private void OnEnable()
    {
        if (PhotonNetwork.CurrentRoom == null) 
            PhotonNetwork.LoadLevel("LobbyScene");
        
        Managers.Game.AlertAction += Ranking;
    }

    private void OnDisable()
    {
        Managers.Game.AlertAction -= Ranking;
    }

    private void LoadPlayer()
    {
        var randPosition = Random.insideUnitCircle * 10;
        _player = PhotonNetwork.Instantiate("Player", new Vector3(randPosition.x, 5, randPosition.y),
            Quaternion.identity);

        _player.name = $"{PhotonNetwork.LocalPlayer.NickName}";
        Managers.Game.Player = _player;
    }

    private IEnumerator CoCountDown()
    {
        float currentTime = totalTime;
        var waitForSeconds = new WaitForSeconds(1f);
        
        while (currentTime > 0)
        {
            yield return waitForSeconds;
            currentTime -= 1f;
            Managers.Game.CountDownAction.Invoke((int)currentTime);
        }
    }

    private void Ranking(Player player)
    {
        Managers.Game.PlayerRankStack.Push(player);

        if (Managers.Game.PlayerRankStack.Count == PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            var players = PhotonNetwork.CurrentRoom.Players.Values.Except(Managers.Game.PlayerRankStack);

            foreach (var activePlayer in players) 
                Managers.Game.PlayerRankStack.Push(activePlayer);

            Managers.Game.FinishAction.Invoke();
            StartCoroutine(CoQuitGame());
        }

        // if (Managers.Game.PlayerRankStack.Count >= PhotonNetwork.CurrentRoom.PlayerCount - 1) 
        //     StartCoroutine(CoQuitGame());
    }

    private IEnumerator CoQuitGame()
    {
        yield return new WaitForSeconds(5f);
        Managers.Game.FinishGame();
    }
}

