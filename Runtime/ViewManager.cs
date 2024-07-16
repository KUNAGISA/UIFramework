using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// Simple View Manager
    /// 只作为测试用，功能不够的自己实现一套即可
    /// </summary>
    public class ViewManager : IViewContainer, IAssetLoader
    {
        private static ViewManager instance = null;
        public static ViewManager Instance => instance ??= new ViewManager();

        private readonly Stack<IView> m_views = new Stack<IView>();
        private readonly Transform m_root;

        private ViewManager()
        {
            m_root = new GameObject("[ViewManager]", typeof(Canvas), typeof(GraphicRaycaster)).transform;
            m_root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            GameObject.DontDestroyOnLoad(m_root);
        }

        public void Open(IViewData data)
        {
            var prefab = data.Load(this);
            var view = GameObject.Instantiate(prefab as MonoBehaviour, m_root, false) as IView;
            view.SetBelong(this);

            if (m_views.TryPeek(out var topView))
            {
                topView.Pause();
            }

            m_views.Push(view);
            view.Open(data);
        }

        void IViewContainer.Pop(IView target)
        {
            if (!m_views.TryPeek(out var topView) || topView != target)
            {
                return;
            }

            m_views.Pop();

            if (m_views.TryPeek(out topView))
            {
                 topView.Resume();
            }

            target.SetBelong(null);
            target.Close();
        }

        T IAssetLoader.Load<T>(string path)
        {
            return Resources.Load<T>(path);
        }
    }
}