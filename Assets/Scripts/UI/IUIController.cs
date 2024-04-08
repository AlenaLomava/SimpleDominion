using Assets.Scripts.UI.Context;

namespace Assets.Scripts.UI
{
    public interface IUIController
    {
        void Hide<TElement>() where TElement : UIElement;

        void HideAll();

        TElement Show<TElement>() where TElement : UIElement;

        TElement Show<TElement>(IUIElementContext context) where TElement : UIElement;
    }
}
