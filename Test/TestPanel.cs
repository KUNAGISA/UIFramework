namespace Gameplay
{
    public partial class TestPanel : UIFramework.UIPanel
    {
        protected override void Awake()
        {
            base.Awake();
            BindAllWidgets();
        }

        protected override void OnOpenAnimationFinished()
        {
            m_text.text = "测试文本";
        }
    }
}

#region Auto Generate UI Bind
namespace Gameplay
{
	public partial class TestPanel : UIFramework.UIPanel
	{
		private UnityEngine.UI.Button m_close = null;
		private TMPro.TextMeshProUGUI m_text = null;

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private void BindAllWidgets()
		{
			var transform = this.transform;

			m_close = transform.Find("btn_Close").GetComponent<UnityEngine.UI.Button>();
			m_close.onClick.AddListener(CloseSelf);

			m_text = transform.Find("btn_Close/txt_Text").GetComponent<TMPro.TextMeshProUGUI>();
		}
	}
}
#endregion Auto Generate UI Bind