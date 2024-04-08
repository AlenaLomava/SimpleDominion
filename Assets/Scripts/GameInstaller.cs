using Assets.Scripts.Actions;
using Assets.Scripts.UI;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    internal class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private UIRoot _uiRoot;

        [SerializeField]
        private SelectableManager _selectableManager;

        [SerializeField]
        private UIConfig _uiConfig;

        public override void InstallBindings()
        {
            Container.Bind<IActionFactory>().To<ActionFactory>().AsSingle();
            Container.Bind<IActionProcessor>().To<ActionProcessor>().AsSingle();
            Container.Bind<IUIController>().To<UIController>().AsSingle();

            Container.Bind<ISelectableManager>().FromInstance(_selectableManager).AsSingle();
            Container.Bind<UIRoot>().FromInstance(_uiRoot).AsSingle();

            Container.BindInterfacesAndSelfTo<UIConfig>().FromInstance(_uiConfig).AsSingle();

            BindActionFactories();
        }

        private void BindActionFactories()
        {
            Container.BindFactory<ShowMovementGridAction, ShowMovementGridActionFactory>().FromNew();
            Container.BindFactory<ChangePositionAction, ChangePositionActionFactory>().FromNew();
            Container.BindFactory<ShowSettlementCaptureButtonAction, ShowSettlementCaptureButtonActionFactory>().FromNew();
            Container.BindFactory<SettlementCaptureAction, SettlementCaptureActionFactory>().FromNew();
            Container.BindFactory<HandleDamageAction, HandleDamageActionFactory>().FromNew();
            Container.BindFactory<SettlementClickedAction, SettlementClickedActionFactory>().FromNew();
            Container.BindFactory<HireAction, HireActionFactory>().FromNew();
            Container.BindFactory<NextTurnAction, NextTurnActionFactory>().FromNew();
        }
    }
}
