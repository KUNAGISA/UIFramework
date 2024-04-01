using System.Collections;
using UnityEngine;

namespace UIFramework
{
    public interface IUIPanel
    {
        void Open();
        void Close();
        void Pause();
        void Resume();
        void SetBelong(UIStack stack);
    }

    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup), typeof(CanvasRenderer), typeof(RectTransform))]
    public abstract class UIPanel : MonoBehaviour, IUIPanel
    {
        public static int OpenPanelStateId = Animator.StringToHash("OpenPanel");
        public static int ClosePanelStateId = Animator.StringToHash("ClosePanel");

        public CanvasGroup CanvasGroup { get; private set; } = null;

        private UIStack m_belong = null;
        private Coroutine m_openPanelAnimationCoroutine = null;

        void IUIPanel.SetBelong(UIStack stack)
        {
            m_belong = stack;
        }

        void IUIPanel.Open()
        {
            OnOpen();

            if (m_openPanelAnimationCoroutine != null)
            {
                StopCoroutine(m_openPanelAnimationCoroutine);
                m_openPanelAnimationCoroutine = null;
            }

            if (TryGetComponent<Animator>(out var animator) && animator.HasState(0, OpenPanelStateId))
            {
                CanvasGroup.interactable = false;
                animator.Play(OpenPanelStateId, 0); animator.Update(0f);
                m_openPanelAnimationCoroutine = StartCoroutine(WaitForOpenAnimationFinish(animator.GetCurrentAnimatorStateInfo(0).length));
            }
            else
            {
                OnOpenAnimationFinished();
            }
        }

        void IUIPanel.Close()
        {
            OnClose();

            if (m_openPanelAnimationCoroutine != null)
            {
                StopCoroutine(m_openPanelAnimationCoroutine);
                m_openPanelAnimationCoroutine = null;
            }

            CanvasGroup.interactable = false;

            if (TryGetComponent<UnRegisterWhenDestroy>(out var component))
            {
                DestroyImmediate(component);
            }

            if (TryGetComponent<Animator>(out var animator) && animator.HasState(0, ClosePanelStateId))
            {
                animator.Play(ClosePanelStateId, 0); animator.Update(0f);
                Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void IUIPanel.Pause()
        {
            OnPause();
            CanvasGroup.interactable = false;
        }

        void IUIPanel.Resume()
        {
            CanvasGroup.interactable = true;
            OnResume();
        }

        public void CloseSelf()
        {
            m_belong?.Pop(this);
        }

        private IEnumerator WaitForOpenAnimationFinish(float time)
        {
            yield return new WaitForSeconds(time);
            OpenPanelAnimationFinished();
        }

        private void OpenPanelAnimationFinished()
        {
            CanvasGroup.interactable = true;
            OnOpenAnimationFinished();
        }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
        protected virtual void OnOpenAnimationFinished() { }
    }
}
