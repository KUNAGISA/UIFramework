# UIFramework

Unity引擎的一个基于栈管理的UI框架实现。

* `UIPanel`界面基类，用于实现界面逻辑

* `UIElement`控件基类，用于实现控件逻辑

* `UIStack`用于管理`UIPanel`的层级关系和出入栈事件调用

* `UIBindGenerator`用于生成自动绑定控件的代码，可以根据特定的标记生成对应控件的绑定代码，具体生成的代码如下，需要自己在`Awake`的时候调用`BindAllWidgets`函数。

  ```c#
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
  ```

* `CodeGenerate`目录下都是对应类型绑定的代码生成，可以自己扩展

### 关于控件绑定代码生成

+ 如果需要添加新的代码生成处理，可以继承`IBindableCodeGenerator`自己实现，然后注册到`UIBindGenerator.CodeGenerators`字典里
+ 如果需要修改生成的代码，可以修改`UIBindGenerator.GenerateBindCode`方法
+ 如果需要修改标记样式，可以修改`UIBindGenerator.BindableRegex`

### 如何使用

clone本项目或者单独下载所需要的部分，然后根据自己的项目需求去进行修改即可，`Test`目录仅作为参考和测试目录请自行删除