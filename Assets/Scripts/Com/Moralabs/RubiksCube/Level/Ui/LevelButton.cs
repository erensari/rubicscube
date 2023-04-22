using Com.Moralabs.RubiksCube.LevelSystem;
using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AudioSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Com.Moralabs.RubiksCube.Level.Ui {
    public class LevelButton : MonoBehaviour {
        [SerializeField]
        private TMP_Text buttonName;
        [SerializeField]
        private TMP_Text levelCompletionTime;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image image;
        [SerializeField]
        private Sprite completedSprite;
        [SerializeField]
        private GameObject timeFrame;

        [Inject]
        private ProjectManager projectManager;
        [Inject]
        private ISoundController2d soundController;

        private int level;

        public void Initialze(int level, string time, bool isLocked, bool isCompleted) {
            this.level = level;
            buttonName.text = (level).ToString();
            button.interactable = isLocked;
            levelCompletionTime.text = time;

            if (isCompleted) {
                image.sprite = completedSprite;
            }
            else {
                timeFrame.SetActive(false);
            }
        }


        public void OpenScene() {
            soundController.PlaySound(new Sound(Sounds.CLICK, volume: 0.5f));
            projectManager.PlayLevel(level);
        }

    }
}

