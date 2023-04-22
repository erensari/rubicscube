using Com.Moralabs.RubiksCube.Game.Manager;
using Morautils.AdSystem;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HistoryButton : MonoBehaviour {
    [Inject]
    GameManager gameManager;
    [Inject]
    private IAdController adController;

    [SerializeField]
    Button previousButton;
    [SerializeField]
    Button nextButton;

    private static int countForAds;

    private void Start() {
        countForAds = 0;

    }
    private void Update() {
        if (gameManager.PreviousMoves.Count < 1) {
            previousButton.interactable = false;

        }
        else {
            previousButton.interactable = true;
        }

        if (gameManager.NextMoves.Count < 1) {
            nextButton.interactable = false;

        }
        else {
            nextButton.interactable = true;
        }
      
    }

    public void Previous() {

        CheckAd();
        gameManager.PreviousPosition();

    }

    private void UnpauseGame(bool success) {
        Time.timeScale = 1f;
    }

    private void CheckAd() {
        countForAds++;
        if (countForAds == 5) {
            Time.timeScale = 0f;
            if (adController.ShowInterstitial()) {
                adController.OnInterstitialCompleted.Take(1).Subscribe(UnpauseGame);
                countForAds = 0;
            }
        }
    }

    public void Next() {

        CheckAd();
        gameManager.NextPositions();
    }

}
