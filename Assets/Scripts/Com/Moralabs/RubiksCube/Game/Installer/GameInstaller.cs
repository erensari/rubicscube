using Com.Moralabs.RubiksCube.Game.Manager;
using UnityEngine;
using Zenject;
namespace Com.Moralabs.RubiksCube.Game.Installer {
    public class GameInstaller : MonoInstaller {
        [SerializeField]
        private GameManager gameManager;
        public override void InstallBindings() {
            Container.Bind<GameManager>()
                .FromInstance(gameManager);
        }
    }
}
