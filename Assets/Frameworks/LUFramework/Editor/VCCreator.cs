/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : VC创建类
 */

using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LUFramework
{
    /// <summary>
    /// VC创建类
    /// </summary>
	public class VCCreator
	{
        #region 私有字段
        /// <summary>
        /// 存储节点和UI类型
        /// </summary>
        private static Dictionary<RectTransform, string> _uiTypeDic = new Dictionary<RectTransform, string>();
        #endregion

        #region 右键菜单方法
        /// <summary>
        ///  自动创建MVC
        /// </summary>
        [MenuItem("GameObject/LUFramework/AutoCreateMVC", priority = 0)]
        public static void CreateMVC()
        {
            // 获得选择的物体
            GameObject selectionObj = Selection.activeGameObject;

            // 判空
            if (selectionObj == null)
            {
                return;
            }

            // 获得UI控件
            GetUI(selectionObj);

            // 创建视图脚本
            CreateViewScript(selectionObj);

            // 创建控制脚本
            CreateControllerScript(selectionObj);
            
            // 设置配置项
            SetConfigItems(selectionObj);
        }
        #endregion

        #region 创建视图脚本
        /// <summary>
        /// 获得UI控件
        /// </summary>
        /// <param name="selection">选择的物体</param>
        private static void GetUI(GameObject selection)
        {
            // 清空字典
            _uiTypeDic.Clear();

            // 递归获得UI控件
            GetUIByRecursion(selection.transform);
        }
        
        /// <summary>
        /// 获得UI控件 - 递归
        /// </summary>
        private static void GetUIByRecursion(Transform node)
        {
            // 遍历子节点
            foreach (Transform item in node)
            {
                // 递归调用
                GetUIByRecursion(item);
            }

            // 获得变换组件
            RectTransform rectTrans = node.GetComponent<RectTransform>();

            // 判空
            if (rectTrans == null)
            {
                return;
            }

            // 名字是否不为下划线开始
            if (!node.name.StartsWith("_"))
            {
                return;
            }

            // 尝试获得UIBehaviour组件
            string uiType;
            if (TryGetUIBehaviour(node, out uiType))
            {
                // 存入字典
                _uiTypeDic.Add(rectTrans, uiType);
            }
        }

        /// <summary>
        /// 尝试获得UIBehaviour组件
        /// </summary>
        /// <param name="item">选择的物体</param>
        /// <param name="uiType">ui类型</param>
        /// <returns>是否是UI组件</returns>
        private static bool TryGetUIBehaviour(Transform item, out string uiType)
        {
            // 初始化ui类型
            uiType = "";

            // 获得所有的对应组件
            UIBehaviour[] components = item.GetComponents<UIBehaviour>();

            // 如果没有该组件
            if (components.Length == 0)
            {
                return false;
            }

            // 类型名
            uiType = "UnityEngine.UI.Image";

            // 类型
            string currentType = "";

            // 遍历
            foreach (var component in components)
            {
                // 获得类型名
                currentType = component.GetType().ToString();

                // 如果不是图片
                if (uiType != currentType && currentType != "UnityEngine.UI.Mask" && currentType != "UnityEngine.UI.HorizontalLayoutGroup" && currentType != "UnityEngine.UI.VerticalLayoutGroup" && currentType != "UnityEngine.UI.GridLayoutGroup")
                {
                    // 记录
                    uiType = currentType;
                    break;
                }
            }

            // 只要类型名
            uiType = Path.GetExtension(uiType).Replace(".", "");

            return true;
        }

        /// <summary>
        /// 创建视图脚本
        /// </summary>
        /// <param name="selection">选择物体</param>
        private static void CreateViewScript(GameObject selection)
        {
            // 选择物体名
            string selectionName = selection.name;

            // 获得路径
            string path = Config.VIEW_SCRIPT_PATH;
            
            // 如果路径不存在
            if (!Directory.Exists(Application.dataPath + path.Replace("Assets", "")))
            {
                // 创建路径
                Directory.CreateDirectory(Application.dataPath + path.Replace("Assets", ""));
            }

            // 是否有合格的视图名
            selectionName = GetAvailableViewName(selectionName);

            // 获得文件路径
            path = Path.Combine(path, selectionName + ".cs");

            // 文件是否存在
            if (File.Exists(path))
            {
                LogManager.Instance.Log("LU框架|自动创建脚本", "{0}文件已经存在。不再自动生成该脚本。", ELogSeverity.Warning, true, path);
                return;
            }

            // 获得模板处理后的内容
            string newPath;
            string content = Helper.GetTemplateContent(path, Config.VIEW_TEMPLATE_PATH, out newPath);

            // 替换
            content = SetUIContent(content);

            // 创建资源
            File.WriteAllText(newPath, content, new UTF8Encoding(true, false));
            AssetDatabase.ImportAsset(newPath);
           
            // 获得该脚本
            Component script = selection.GetComponent(selectionName);

            // 判断该物体上是否有该脚本
            if (script != null)
            {
                Object.DestroyImmediate(script);
            }

            // 记录选择物体的id和脚本类型
            EditorPrefs.SetInt("selectionObjInstanceID", selection.GetInstanceID());
            EditorPrefs.SetString("viewScriptType", Helper.GetNamespaceName() + "." + Path.GetFileNameWithoutExtension(newPath));
        }

        /// <summary>
        /// 设置配置项
        /// </summary>
        /// <param name="selection">选中的物体</param>
        private static void SetConfigItems(GameObject selection)
        {
            // 获得选中物体名
            string selectionName = selection.name;

            // 是否有合格的视图名
            selectionName = GetAvailableViewName(selectionName);

            // 组件名
            string componentName = "";

            // 遍历字典
            foreach (KeyValuePair<RectTransform, string> item in _uiTypeDic)
            {
                // 如果是按钮
                if (item.Value == "Button")
                {
                    // 获得组件名
                    string zhName;
                    componentName = GetComponentName(item.Value, item.Key.name, out zhName);

                    // 获得首字母大写的名字
                    string newName = Helper.UpperFirstChar(componentName.Replace("_", ""));

                    // 添加配置项
                    AddConfigItem(selectionName, "UI_" + selectionName + "_" + newName, "点击" + newName + "按钮 通知");
                }
            }
        }

        /// <summary>
        /// 获得合格的视图名
        /// 补全Form 或 View
        /// </summary>
        /// <param name="srcName">原名</param>
        /// <returns>合格的视图名</returns>
        private static string GetAvailableViewName(string srcName)
        {
            // 是否没有视图名
            if (!srcName.ToLower().Contains("form") && !srcName.ToLower().Contains("view") && !srcName.ToLower().Contains("page"))
            {
                // 结尾增加
                srcName += "Form";
            }

            // 特征首字母大写
            return srcName.Replace("form", "Form").Replace("view", "View").Replace("page", "Page");
        }

        /// <summary>
        /// 设置有UI的内容
        /// </summary>
        /// <param name="srcContent">原内容</param>
        /// <returns>有UI的内容</returns>
        private static string SetUIContent(string srcContent)
        {
            // 对应的字符串构造器
            StringBuilder sbComponent = new StringBuilder();
            StringBuilder sbGet = new StringBuilder();
            StringBuilder sbBindEvent = new StringBuilder();
            StringBuilder sbHandler = new StringBuilder();

            // 组件名
            string componentName = "";

            // 遍历字典
            foreach (KeyValuePair<RectTransform, string> item in _uiTypeDic)
            {
                // 获得组件名
                string zhName;
                componentName = GetComponentName(item.Value, item.Key.name, out zhName);

                // 拼接组件内容
                JointComponentContent(sbComponent, componentName, item.Value, zhName);

                // 拼接获取UI的内容
                JointUIContent(sbGet, componentName, item.Key.name, item.Value);

                // 拼接事件绑定的内容
                JointBindEventContent(sbBindEvent, componentName, item.Value);

                // 拼接事件处理的内容
                JointHandlerContent(sbHandler, componentName, item.Value, zhName);
            }

            // 移除最后的换行
            if (sbComponent.Length > 1)
            {
                sbComponent.Remove(sbComponent.Length - 1, 1);
            }
            if (sbGet.Length > 1)
            {
                sbGet.Remove(sbGet.Length - 1, 1);
            }
            if (sbBindEvent.Length > 1)
            {
                sbBindEvent.Remove(sbBindEvent.Length - 1, 1);
            }
            if (sbHandler.Length > 1)
            {
                sbHandler.Remove(sbHandler.Length - 1, 1);
            }

            // 替换为对应内容
            return srcContent.Replace("#UI_COMPONENTS#", sbComponent.ToString())
                .Replace("#GET_UI#", sbGet.ToString())
                .Replace("#BIND_EVENT#", sbBindEvent.ToString())
                .Replace("#UI_EVENT#", sbHandler.ToString());
        }

        /// <summary>
        /// 拼接组件内容
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="componentName">组件名</param>
        /// <param name="type">类型名</param>
        /// <param name="zhName">中文名</param>
        /// <returns>拼接后的组件内容</returns>
        private static void JointComponentContent(StringBuilder sb, string componentName, string type, string zhName)
        {
            // 拼接UI组件声明内容
            sb.Append(Helper.GetSummaryString(zhName));
            sb.Append("\t\tprivate ");
            sb.Append(type.Replace(" ", ""));
            sb.Append(" ");
            sb.Append(componentName);
            sb.Append(";\n");
        }

        /// <summary>
        /// 拼接获取UI的内容
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="componentName">组件名</param>
        /// <param name="uiName">ui名</param>
        /// <param name="type">类型名</param>
        /// <returns>获取UI的内容</returns>
        private static void JointUIContent(StringBuilder sb, string componentName, string uiName, string type)
        {
            sb.Append("\t\t\t");
            sb.Append(componentName);
            sb.Append(" = Helper.GetComponentByRecursion<");
            sb.Append(type);
            sb.Append(">(transform, \"");
            sb.Append(uiName);
            sb.Append("\");\n");
        }

        /// <summary>
        /// 拼接绑定事件的内容
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="componentName">组件名</param>
        /// <param name="type">类型名</param>
        /// <returns>绑定事件的内容</returns>
        private static void JointBindEventContent(StringBuilder sb, string componentName, string type)
        {
            // 如果是按钮
            if (type == "Button")
            {
                sb.Append("\t\t\t// 绑定");
                sb.Append(componentName);
                sb.Append("事件\n");
                sb.Append("\t\t\t");
                sb.Append(componentName);
                sb.Append(".onClick.AddListener(OnClick");
                sb.Append(Helper.UpperFirstChar(componentName.Replace("_", "")));
                sb.Append(");\n");
            }
            // 如果是开关
            else if (type == "Toggle")
            {
                sb.Append("\t\t\t// 绑定");
                sb.Append(componentName);
                sb.Append("事件\n");
                sb.Append("\t\t\t");
                sb.Append(componentName);
                sb.Append(".onValueChanged.AddListener(On");
                sb.Append(Helper.UpperFirstChar(componentName.Replace("_", "")));
                sb.Append("ValueChanged);\n");
            }
            // 如果是下拉
            else if (type == "Dropdown")
            {
                sb.Append("\t\t\t// 绑定");
                sb.Append(componentName);
                sb.Append("事件\n");
                sb.Append("\t\t\t");
                sb.Append(componentName);
                sb.Append(".onValueChanged.AddListener(On");
                sb.Append(Helper.UpperFirstChar(componentName.Replace("_", "")));
                sb.Append("ValueChanged);\n");
            }
        }

        /// <summary>
        /// 拼接事件处理的内容
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="componentName">组件名</param>
        /// <param name="type">类型名</param>
        /// <param name="zhName">中文名</param>
        /// <returns>事件处理的内容</returns>
        private static void JointHandlerContent(StringBuilder sb, string componentName, string type, string zhName)
        {
            // 获得首字母大写的名字
            string newName = Helper.UpperFirstChar(componentName.Replace("_", ""));

            // 如果是按钮
            if (type == "Button")
            {
                sb.Append(Helper.GetSummaryString(zhName + componentName + "点击事件处理方法"));
                sb.Append("\t\tpublic void OnClick");
                sb.Append(newName);
                sb.Append("()\n\t\t{\n");
                sb.Append("\t\t\t// 发送通知\n");
                sb.Append("\t\t\tSendNotification(NotificationConfig.UI_");
                sb.Append(GetAvailableViewName(Selection.activeGameObject.name).ToUpper());
                sb.Append("_");
                sb.Append(newName.ToUpper());
                sb.Append(");\n\t\t}\n");
            }
            // 如果是开关
            else if (type == "Toggle")
            {
                sb.Append(Helper.GetSummaryString(zhName + componentName + "值改变处理方法", 2, "", "isOn", "是否打开"));
                sb.Append("\t\tpublic void On");
                sb.Append(newName);
                sb.Append("ValueChanged(bool isOn)\n\t\t{\n\t\t\t\n\t\t}\n");
            }
            // 如果是下拉
            else if (type == "Dropdown")
            {
                sb.Append(Helper.GetSummaryString(zhName + componentName + "值改变处理方法", 2, "", "index", "当前选择的下标"));
                sb.Append("\t\tpublic void On");
                sb.Append(newName);
                sb.Append("ValueChanged(int index)\n\t\t{\n\t\t\t\n\t\t}\n");
            }
        }

        /// <summary>
        /// 获得组件名
        /// </summary>
        /// <param name="uiType">类型名</param>
        /// <param name="uiName">UI名</param>
        /// <param name="zhName">中文名</param>
        /// <returns>组件名</returns>
        private static string GetComponentName(string uiType, string uiName, out string zhName)
        {
            // 类型缩写名
            string typeAttr = "";

            // 去掉类型的纯名字
            string pureName = "";

            // 获得对应的类型名缩写 和 中文名
            zhName = "";
            switch (uiType)
            {
                case "Image":
                {
                    typeAttr = "_img";
                    zhName = "图片";
                    pureName = GetPureName(uiName, "image|img");
                }
                break;
                case "Button":
                {
                    typeAttr = "_btn";
                    zhName = "按钮";
                    pureName = GetPureName(uiName, "button|btn");
                }
                break;
                case "Text":
                {
                    typeAttr = "_text";
                    zhName = "文本";
                    pureName = GetPureName(uiName, "text");
                }
                break;
                case "Toggle":
                {
                    typeAttr = "_toggle";
                    zhName = "开关";
                    pureName = GetPureName(uiName, "toggle");
                }
                break;
                case "Slider":
                {
                    typeAttr = "_slider";
                    zhName = "滑动条";
                    pureName = GetPureName(uiName, "slider");
                }
                break;
                case "ScrollBar":
                {
                    typeAttr = "_bar";
                    zhName = "滚动条";
                    pureName = GetPureName(uiName, "scrollbar|scroll|bar");
                }
                break;
                case "Dropdown":
                {
                    typeAttr = "_drop";
                    zhName = "下拉菜单";
                    pureName = GetPureName(uiName, "dropdown|droplist|down|drop");
                }
                break;
                case "InputField":
                {
                    typeAttr = "_input";
                    zhName = "输入框";
                    pureName = GetPureName(uiName, "input");
                }
                break;
                case "ScrollRect":
                {
                    typeAttr = "_scroll";
                    zhName = "滚动栏";
                    pureName = GetPureName(uiName, "scrollrect|scrollview|scroll");
                }
                break;
            }
            
            // 返回组件名
            return typeAttr + Helper.UpperFirstChar(pureName);
        }

        /// <summary>
        /// 获得没有类型名的名字
        /// </summary>
        /// <param name="srcName">原名字</param>
        /// <param name="pattern">剔除的字符串 |分割/param>
        /// <returns>没有类型名的名字</returns>
        private static string GetPureName(string srcName, string pattern)
        {
            // 将匹配的移除
            return Regex.Replace(srcName, pattern + "|_", "", RegexOptions.IgnoreCase);
        }
        #endregion

        #region 创建配置脚本
        /// <summary>
        /// 创建配置脚本
        /// </summary>
        private static void CreateConfigScript()
        {
            // 获得路径
            string path = Config.NOTIFICATION_CONFIG_PATH;

            // 如果路径不存在
            if (!Directory.Exists(Application.dataPath + path.Replace("Assets", "")))
            {
                // 创建路径
                Directory.CreateDirectory(Application.dataPath + path.Replace("Assets", ""));
            }

            // 拼接文件路径
            path = Path.Combine(path, "NotificationConfig.cs");

            // 获得模板处理后的内容
            string newPath;
            string content = Helper.GetTemplateContent(path, Config.NOTIFICATION_CONFIG_TEMPLATE_PATH, out newPath);
            
            // 创建资源
            File.WriteAllText(newPath, content, new UTF8Encoding(true, false));
            AssetDatabase.ImportAsset(newPath);
        }

        /// <summary>
        /// 添加配置项
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="configName">配置名</param>
        /// <param name="summary">注释</param>
        private static void AddConfigItem(string title, string configName, string summary)
        {
            // 拼接文件路径
            string path = Path.Combine(Config.NOTIFICATION_CONFIG_PATH, "NotificationConfig.cs");

            // 是否没有配置文件
            if (!File.Exists(path))
            {
                // 创建资源
                CreateConfigScript();
            }

            // 读取配置文件
            string[] lines = File.ReadAllLines(path);

            // 每行信息
            string lineText = "";

            // 标记是否找到了目标标题块
            bool isFindTargetTitle = false;

            // 遍历
            for (int i = 0; i < lines.Length; i++)
            {
                // 获得该行信息
                lineText = lines[i];
                
                // 是否是标记
                if (lineText.Replace(" ", "") == "#Config_Item#")
                {
                    // 修改为配置项目
                    lines[i] = GetConfigItemString(title, configName, summary, true);
                    break;
                }
                else if (lineText.Replace(" ", "").Replace("\t", "").Replace("\n", "") == "#region" + title)
                {
                    // 标记
                    isFindTargetTitle = true;
                }
                // 是否有该项了
                else if (isFindTargetTitle && lineText.Contains(configName))
                {
                    break;
                }
                else if (lineText.Replace(" ", "").Replace("\t", "").Replace("\n", "") == "#endregion")
                {
                    // 是否找到了该标题
                    if (isFindTargetTitle)
                    {
                        // 添加配置项目
                        lines[i] = GetConfigItemString(title, configName, summary, false) + lines[i];
                        break;
                    }
                    else
                    {
                        // 是否是最后了
                        if (lines[i + 1].Contains("}"))
                        {
                            // 添加新的标题组
                            lines[i] += "\n\n" + GetConfigItemString(title, configName, summary, true);
                            break;
                        }
                    }
                }
            }

            // 写到文件
            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// 获得配置项字符串
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="configName">配置名</param>
        /// <param name="summary">注释</param>
        /// <param name="isNewTitle">是否是新的标题</param>
        /// <returns>配置项字符串</returns>
        private static string GetConfigItemString(string title, string configName, string summary, bool isNewTitle)
        {
            // 字符串构造类
            StringBuilder sb = new StringBuilder();

            // 是否是新标题
            if (isNewTitle)
            {
                sb.Append("\t\t#region ");
                sb.Append(title);
                sb.Append("\n");
            }

            // 拼接字符串内容
            sb.Append(Helper.GetSummaryString(summary));
            sb.Append("\t\tpublic const string ");
            sb.Append(configName.ToUpper());
            sb.Append(" = \"");
            sb.Append(configName);
            sb.Append("\";\n");

            // 是否是新标题
            if (isNewTitle)
            {
                sb.Append("\t\t#endregion");
            }

            return sb.ToString();
        }
        #endregion

        #region 创建控制脚本
        /// <summary>
        /// 创建控制脚本
        /// </summary>
        /// <param name="selection">选择物体</param>
        private static void CreateControllerScript(GameObject selection)
        {
            // 选择物体名
            string selectionName = selection.name;

            // 获得路径
            string path = Config.CONTROLLER_SCRIPT_PATH;

            // 如果路径不存在
            if (!Directory.Exists(Application.dataPath + path.Replace("Assets", "")))
            {
                // 创建路径
                Directory.CreateDirectory(Application.dataPath + path.Replace("Assets", ""));
            }

            // 是否有合格的视图名
            selectionName = GetAvailableControllerName(selectionName);

            // 获得文件路径
            path = Path.Combine(path, selectionName + ".cs");

            // 文件是否存在
            if (File.Exists(path))
            {
                LogManager.Instance.Log("LU框架|自动创建脚本", "{0}文件已经存在。不再自动生成该脚本。", ELogSeverity.Warning, true, path);
                return;
            }

            // 获得模板处理后的内容
            string newPath;
            string content = Helper.GetTemplateContent(path, Config.CONTROLLER_TEMPLATE_PATH, out newPath);

            // 替换
            content = SetHandlerFunc(content);

            // 创建资源
            File.WriteAllText(newPath, content, new UTF8Encoding(true, false));
            AssetDatabase.ImportAsset(newPath);

            // 获得该脚本
            Component script = selection.GetComponent(selectionName);

            // 判断该物体上是否有该脚本
            if (script != null)
            {
                Object.DestroyImmediate(script);
            }

            // 记录选择物体的id和脚本类型
            EditorPrefs.SetInt("selectionObjInstanceID", selection.GetInstanceID());
            EditorPrefs.SetString("controllerScriptType", Helper.GetNamespaceName() + "." + Path.GetFileNameWithoutExtension(newPath));
        }

        /// <summary>
        /// 设置处理方法
        /// </summary>
        /// <param name="srcContent">原内容</param>
        /// <returns>添加处理方法后的内容</returns>
        private static string SetHandlerFunc(string srcContent)
        {
            // 字符串构造器
            StringBuilder sbRegister = new StringBuilder();
            StringBuilder sbHandler = new StringBuilder();

            // 组件名
            string componentName = "";

            // 遍历字典
            foreach (KeyValuePair<RectTransform, string> item in _uiTypeDic)
            {
                // 如果是按钮
                if (item.Value == "Button")
                {
                    // 获得组件名
                    string zhName;
                    componentName = GetComponentName(item.Value, item.Key.name, out zhName);

                    // 获得首字母大写的名字
                    string newName = Helper.UpperFirstChar(componentName.Replace("_", ""));

                    // 注册处理方法
                    sbRegister.Append("\t\t\t// 注册");
                    sbRegister.Append(newName);
                    sbRegister.Append("处理方法\n\t\t\tRegisterHandler(NotificationConfig.UI_");
                    sbRegister.Append(GetAvailableViewName(Selection.activeGameObject.name).ToUpper());
                    sbRegister.Append("_");
                    sbRegister.Append(newName.ToUpper());
                    sbRegister.Append(", OnClick");
                    sbRegister.Append(newName);
                    sbRegister.Append(");\n");

                    // 处理方法
                    sbHandler.Append(Helper.GetSummaryString(newName + "处理方法", 2, "", "args", "参数"));
                    sbHandler.Append("\t\tpublic void OnClick");
                    sbHandler.Append(newName);
                    sbHandler.Append("(object args)\n\t\t{\n\t\t\t\n\t\t}\n\n");
                }
            }

            // 移除最后的两个换行
            if (sbHandler.Length > 2)
            {
                sbHandler.Remove(sbHandler.Length - 2, 2);
            }

            return srcContent.Replace("#Register_Handler#", sbRegister.ToString())
                .Replace("#Handler_Func#", sbHandler.ToString());
        }

        /// <summary>
        /// 获得合格的控制类名
        /// </summary>
        /// <param name="srcName">原名</param>
        /// <returns>合格的控制类名</returns>
        private static string GetAvailableControllerName(string srcName)
        {
            // 是否没有控制类名
            if (!srcName.ToLower().Contains("controller"))
            {
                // 结尾增加
                srcName += "Controller";
            }

            // 特征首字母大写
            return srcName.Replace("controller", "Controller");
        }
        #endregion

        #region 当脚本加载完毕回调
        /// <summary>
        /// 当脚本重加载
        /// </summary>
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // 获得选中物体id 和 脚本类名
            int selectionObjInstanceID = EditorPrefs.GetInt("selectionObjInstanceID");
            string viewScriptTypeName = EditorPrefs.GetString("viewScriptType");
            string controllerScriptTypeName = EditorPrefs.GetString("controllerScriptType");

            // 判空
            if (selectionObjInstanceID == 0 || string.IsNullOrEmpty(viewScriptTypeName) || string.IsNullOrEmpty(controllerScriptTypeName))
            {
                return;
            }

            // 找到对应物体和类
            GameObject selectionObj = EditorUtility.InstanceIDToObject(selectionObjInstanceID) as GameObject;
            System.Type viewScriptType = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(viewScriptTypeName);
            System.Type controllerScriptType = System.Reflection.Assembly.Load("Assembly-CSharp").GetType(controllerScriptTypeName);

            // 判空
            if (selectionObj == null || viewScriptType == null || controllerScriptType == null)
            {
                return;
            }

            // 该脚本添加到物体上
            selectionObj.AddComponent(viewScriptType);
            selectionObj.AddComponent(controllerScriptType);

            // 移除数据
            EditorPrefs.DeleteKey("selectionObj");
            EditorPrefs.DeleteKey("viewScriptType");
            EditorPrefs.DeleteKey("controllerScriptType");

            // 移除物体上的重复脚本
            RemoveDuplicatedScript(selectionObj);
        }
        #endregion

        #region 移除重复的脚本
        /// <summary>
        /// 移除重复的脚本
        /// </summary>
        /// <param name="obj">游戏物体</param>
        private static void RemoveDuplicatedScript(GameObject obj)
        {
            List<string> _typeList = new List<string>();
            List<Component> _removeList = new List<Component>();

            // 获得所有组件
            Component[] components = obj.GetComponents<Component>();

            // 遍历所有组件
            foreach (var component in components)
            {
                // 是否该类在列表中存在
                if (_typeList.Contains(component.GetType().Name))
                {
                    // 添加到移除列表
                    _removeList.Add(component);
                }
                else
                {
                    // 添加到列表
                    _typeList.Add(component.GetType().Name);
                }
            }

            // 遍历移除列表
            foreach (var item in _removeList)
            {
                // 移除组件
                Object.DestroyImmediate(item);
            }
        }
        #endregion
    }
}