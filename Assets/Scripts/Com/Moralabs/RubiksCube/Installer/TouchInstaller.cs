using Morautils.InputSystem.TouchSystem;
using Zenject;

namespace Com.Moralabs.RubiksCube.Installer {
    public class TouchInstaller : MonoInstaller {
        public override void InstallBindings() {
            var down = DownFactory.Create();
            var up = UpFactory.Create();
            var hold = HoldFactory.Create();

            Container.BindInterfacesTo<DragController>()
                .AsSingle()
                .WithArguments(down, up, hold);
        }
    }
}