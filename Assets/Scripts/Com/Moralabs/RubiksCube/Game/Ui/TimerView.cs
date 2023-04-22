using Com.Moralabs.RubiksCube.Game.Manager;
using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Com.Moralabs.RubiksCube.Game.Ui {
    public class TimerView : MonoBehaviour {
        [SerializeField]
        private TMP_Text timerText;
        [Inject]
        private GameManager gameManager;
        private void Start() {
            gameManager.CurrentTime
                .TakeUntilDestroy(gameObject)
                .Subscribe(SetTime);
        }
        private void SetTime(float time) {           
            
            

            var seconds = (time % 60).ToString("00.0", CultureInfo.InvariantCulture);
            var minutes = Mathf.Floor(time / 60).ToString("00", CultureInfo.InvariantCulture);
            timerText.text = minutes + ":" + seconds;

        }
    }
}
