using Com.Moralabs.RubiksCube.AnalyticsSystem;
using Com.Moralabs.RubiksCube.LevelSystem;
using Com.Moralabs.RubiksCube.Manager;
using Com.Moralabs.RubiksCube.NotificationSystem;
using Com.Moralabs.RubiksCube.Util;
using Morautils.AssetSystem;
using Morautils.AudioSystem;
using Morautils.FacebookWrapper;
using Morautils.Factory.Prefab;
using Morautils.IronsourceWrapper;
using Morautils.LanguageSystem;
using Morautils.PopupSystem;
using Zenject;
using UnityEngine;

namespace Com.Moralabs.RubiksCube.Installer {
    public class ProjectInstaller : MonoInstaller {

        [SerializeField]
        private AudioConfig audioConfig;

        public override void InstallBindings() {


            AudioSystemInstaller<Sound2dInstaller, MusicInstaller>.Install(Container, audioConfig);


            Container.BindInterfacesAndSelfTo<ProjectManager>()
                    .AsSingle()
                    .NonLazy();
            Container.BindInterfacesAndSelfTo<LevelController>()
                    .AsSingle();

            Container.BindInterfacesTo<AssetLoader>()
                    .AsSingle()
                    .NonLazy();
            Container.BindInterfacesTo<AtlasLoader>()
                    .AsSingle()
                    .NonLazy();
            Container.BindInterfacesTo<AssetManager>()
                    .AsSingle()
                    .NonLazy();

            Container.BindInterfacesTo<PrefabFactory>()
                    .AsSingle();

            Container.BindInterfacesTo<PopupController>()
                    .AsSingle();
            Container.BindInterfacesTo<LanguageController>()
                    .AsSingle()
                    .NonLazy();

            Container.BindInterfacesAndSelfTo<FirebaseWrapper>().AsSingle();

            Container.BindInterfacesAndSelfTo<AnalyticsManager>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<OneSignalWrapper>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<PushNotifications>().AsSingle().NonLazy();

            IronsourceAdInstaller.Install(Container);

            FacebookInstaller.Install(Container);


        }

    }
}
