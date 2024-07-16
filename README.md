# UIFramework

Unity引擎的一个基于栈管理的UI框架实现。

* `View<T>`界面基类，用于实现界面逻辑

* `Widget`控件基类，用于实现控件逻辑

* `ViewData<T>`界面数据基类，用于定义界面打开时的数据

* `UIBindGenerator`用于生成自动绑定控件的代码，可以根据特定的标记生成对应控件的绑定代码，具体生成的代码如下，需要自己在`Awake`的时候调用`BindAllWidgets`函数。

* `CodeGenerate`目录下都是对应类型绑定的代码生成，可以自己扩展

### 关于控件绑定代码生成

+ 如果需要添加新的代码生成处理，可以继承`IBindableCodeGenerator`自己实现，然后注册到`UIBindGenerator.CodeGenerators`字典里
+ 如果需要修改生成的代码，可以修改`UIBindGenerator.GenerateBindCode`方法
+ 如果需要修改标记样式，可以修改`UIBindGenerator.BindableRegex`

### 如何使用

clone本项目或者单独下载所需要的部分，然后根据自己的项目需求去进行修改即可