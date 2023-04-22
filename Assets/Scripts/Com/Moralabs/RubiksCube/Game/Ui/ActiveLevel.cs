using Com.Moralabs.RubiksCube.Manager;
using TMPro;
using UnityEngine;
using Zenject;

public class ActiveLevel : MonoBehaviour
{

    [SerializeField]
    private TMP_Text levelText;
    [Inject]
    private ProjectManager projectManager;

    private void Awake() {
        levelText.text = projectManager.ActiveLevelData.currentLevel.ToString();
    }

}
