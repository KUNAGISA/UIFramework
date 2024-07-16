using System.Collections;
using UnityEngine;

namespace UIFramework
{
    public interface IView
    {
        internal void Open(IViewData data);
        internal void Close();
        internal void Pause();
        internal void Resume();
        internal void SetBelong(IViewContainer container);
    }

    public abstract class View : MonoBehaviour, IView
    {
        void IView.Open(IViewData data) => throw new System.NotImplementedException();
        void IView.Close() => throw new System.NotImplementedException();
        void IView.Pause() => throw new System.NotImplementedException();
        void IView.Resume() => throw new System.NotImplementedException();
        void IView.SetBelong(IViewContainer container) => throw new System.NotImplementedException();

        [ContextMenu("Generate Bind Code", isValidateFunction: false, priority: 1000000)]
        private void GenerateBindCode() => CodeGenerator.OnGenerateBindCode?.Invoke(this);
    }

    [DisallowMultipleComponent, RequireComponent(typeof(CanvasGroup), typeof(CanvasRenderer), typeof(RectTransform))]
    public abstract class View<TViewData> : View, IView where TViewData : class, new()
    {
        public static int OpenPanelStateId = Animator.StringToHash("OpenPanel");
        public static int ClosePanelStateId = Animator.StringToHash("ClosePanel");

        public CanvasGroup CanvasGroup { get; private set; } = null;

        private IViewContainer m_belong = null;
        private Coroutine m_openPanelAnimationCoroutine = null;

        void IView.SetBelong(IViewContainer container) => m_belong = container;

        void IView.Open(IViewData data)
        {
            OnOpen(data as TViewData ?? new TViewData());

            if (m_openPanelAnimationCoroutine != null)
            {
                StopCoroutine(m_openPanelAnimationCoroutine);
                m_openPanelAnimationCoroutine = null;
            }

            m_openPanelAnimationCoroutine = StartCoroutine(OnShowAnimation());
        }

        void IView.Close()
        {
            OnClose();

            if (m_openPanelAnimationCoroutine != null)
            {
                StopCoroutine(m_openPanelAnimationCoroutine);
                m_openPanelAnimationCoroutine = null;
            }

            CanvasGroup.interactable = false;

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

        void IView.Pause()
        {
            OnPause();
            CanvasGroup.interactable = false;
        }

        void IView.Resume()
        {
            CanvasGroup.interactable = true;
            OnResume();
        }

        public void CloseSelf()
        {
            m_belong?.Pop(this);
        }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual IEnumerator OnShowAnimation()
        {
            if (TryGetComponent<Animator>(out var animator) && animator.HasState(0, OpenPanelStateId))
            {
                CanvasGroup.interactable = false;
                animator.Play(OpenPanelStateId, 0, 0f);
                animator.Update(0f);

                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                CanvasGroup.interactable = true;
            }
        }

        protected virtual void OnOpen(TViewData data) { }
        protected virtual void OnClose() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
    }
}
