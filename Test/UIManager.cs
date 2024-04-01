using System;
using UnityEngine;
using UIFramework;

namespace Gameplay
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance = null;
        public static UIManager Instance => instance;

        private readonly UIStack m_uiStack = new UIStack();

        [SerializeField]
        private Transform m_uiRoot = null;

        private void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(instance);
                instance = null;
            }
            instance = this;
        }

        private void Start()
        {
            foreach (var panel in GetComponentsInChildren<UIPanel>())
            {
                m_uiStack.Push(panel);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            m_uiStack.Clear();
        }

        private T CreatePanel<T>(string uipath) where T : UIPanel
        {
            var prefab = Resources.Load<T>(uipath);
            var panel = Instantiate(prefab, m_uiRoot, false);
            panel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            return panel;
        }

        private void OpenPanel(string uipath)
        {
            var panel = CreatePanel<UIPanel>(uipath);
            m_uiStack.Push(panel);
        }

        public void ClosePanel<T>() where T : UIPanel
        {
            m_uiStack.Pop<T>();
        }

        public void CloseToTop()
        {
            m_uiStack.Pop(m_uiStack.Count - 1);
        }

        public void OpenTestPanel()
        {
            OpenPanel("UIPanel/TestPanel");
        }
    }
}
