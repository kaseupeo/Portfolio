using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GamePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private RectTransform playerEliminatedAlertLayout;
    [SerializeField] private TextMeshProUGUI playerEliminatedAlertTextPrefab;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI rankText;

    
    private void OnEnable()
    {
        Managers.Game.CountDownAction += CountDown;
        Managers.Game.AlertAction += Alert;
        Managers.Game.FinishAction += FinishGame;
        
        playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}";
    }

    private void OnDisable()
    {
        Managers.Game.CountDownAction -= CountDown;
        Managers.Game.AlertAction -= Alert;
        Managers.Game.FinishAction -= FinishGame;
    }

    private void CountDown(int time)
    {
        if (time <= 0)
        {
            countDownText.gameObject.SetActive(false);
            return;
        }

        countDownText.gameObject.SetActive(true);
        countDownText.text = time.ToString();
        countDownText.transform.DOLocalMoveY(250, 1).SetLoops(-1, LoopType.Restart).SetEase(Ease.InQuad);
        countDownText.DOFade(0, 1).SetLoops(-1, LoopType.Restart).SetEase(Ease.InQuad);
    }

    private void Alert(Player player)
    {
        var alertText = Instantiate(playerEliminatedAlertTextPrefab, playerEliminatedAlertLayout, false);
        alertText.text = $"{player.NickName} 탈락";
        alertText.DOFade(0, 3).SetEase(Ease.InQuad);
        Destroy(alertText.gameObject, 4f);
        
        playerCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount - Managers.Game.PlayerRankStack.Count}";
    }

    private void FinishGame()
    {
        int total = Managers.Game.PlayerRankStack.Count;
        
        for (int i = 0; i < total; i++)
        {
            if (Equals(Managers.Game.PlayerRankStack.Pop(), PhotonNetwork.LocalPlayer))
            {
                rankText.gameObject.SetActive(true);
                rankText.text = $"{i + 1}등";

                if (i == 0)
                {
                    var player = Managers.Game.Player;
                    player.GetComponentInChildren<PlayerCharacter>().Anim.Play("Dance");
                    Instantiate(Resources.Load("Particle System"), player.transform);
                }
                
                break;
            }
        }
    }
}

