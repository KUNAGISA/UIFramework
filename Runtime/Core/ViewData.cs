using UnityEngine;

namespace UIFramework
{
    public interface IAssetLoader
    {
        T Load<T>(string path) where T : Object;
    }

    public interface IViewData
    {
        string ViewPath { get; }
        internal IView Load(IAssetLoader loader);
    }

    public abstract class ViewData<TView> : IViewData where TView : MonoBehaviour, IView
    {
        public abstract string ViewPath { get; }

        IView IViewData.Load(IAssetLoader loader)
        {
            return loader.Load<TView>(ViewPath);
        }
    }
}