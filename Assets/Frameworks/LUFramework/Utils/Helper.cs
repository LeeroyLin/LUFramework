/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 帮助类
 */

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 帮助类
    /// </summary>
	public class Helper
	{
        /// <summary>
        /// 获得有效的名字
        /// 即首字母不为数字，没有特殊字符的字符串
        /// </summary>
        /// <param name="srcStr">原字符串</param>
        /// <param name="newStr">处理后的字符串</param>
        /// <returns>是否不为空</returns>
        public static bool GetAvailableString(string srcStr, out string newStr)
        {
            // 去掉特殊字符
            srcStr = Regex.Replace(srcStr, "[^A-Z|a-z|0-9|_]", "");

            // 是否为空
            if (string.IsNullOrEmpty(srcStr))
            {
                newStr = "";
                return false;
            }

            // 获得首字符ascii
            int ascii = srcStr[0];

            // 判断首字符是否为数字
            if (ascii >= 48 && ascii <= 57)
            {
                // 添加下划线
                newStr = "_" + srcStr;
                return true;
            }

            // 返回
            newStr = srcStr;
            return true;
        }

        /// <summary>
        /// 将字符串首字符大写
        /// </summary>
        /// <param name="srcStr">原字符串</param>
        /// <returns>首字符大写后的字符串</returns>
        public static string UpperFirstChar(string srcStr)
        {
            // 判空
            if (string.IsNullOrEmpty(srcStr))
            {
                return "";
            }

            // 获得首字符ascii
            int ascii = srcStr[0];

            // 如果不是小写字母
            if (ascii < 97 || ascii > 122)
            {
                return srcStr;
            }

            // 首字母大写
            return (srcStr[0] + "").ToUpper() + (srcStr.Length > 1 ? srcStr.Substring(1) : "");
        }

        /// <summary>
        /// 设置字符串开始的文本
        /// </summary>
        /// <param name="srcStr">原字符串</param>
        /// <param name="startStr">目标开始字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string SetStartWith(string srcStr, string startStr)
        {
            // 是否是由目标字符串开始
            if (srcStr.StartsWith(startStr))
            {
                // 直接返回
                return srcStr;
            }
            else
            {
                // 返回拼接后的字符串
                return startStr + srcStr;
            }
        }

        /// <summary>
        /// 获得有效的命名空间名
        /// </summary>
        /// <returns>有效的命名空间名</returns>
        public static string GetNamespaceName()
        {
            // 可用的名字
            string availableName = "";

            // 是否有配置信息
            if (Config.NAME_SPACE.Trim() != "" && Config.NAME_SPACE.Trim().ToLower() != "auto")
            {
                // 获得设置的名字的有效字符串
                if (GetAvailableString(Config.NAME_SPACE, out availableName))
                {
                    // 返回
                    return availableName;
                }
            }

            // 只留下大写字母
            string tempName = Regex.Replace(Application.productName, "[^A-Z]", "");

            // 获得有效名字
            if (GetAvailableString(tempName, out availableName))
            {
                // 返回
                return availableName;
            }
            else
            {
                // 直接获得有效名字
                if (GetAvailableString(Application.productName, out availableName))
                {
                    // 返回
                    return availableName;
                }
                else
                {
                    // 设置为"Space"
                    return "Space";
                }
            }
        }

        /// <summary>
        /// 获得处理后的模板内容
        /// </summary>
        /// <param name="pathName">路径</param>
        /// <param name="templatePath">模板文件路径</param>
        /// <param name="newPath">新路径</param>
        /// <param name="fileNameHandler">文件名处理方法</param>
        /// <returns>处理后的模板内容</returns>
        public static string GetTemplateContent(string pathName, string templatePath, out string newPath, Func<string, string> fileNameHandler = null)
        {
            newPath = "";

            // 是否没有模板资源
            if (!File.Exists(templatePath))
            {
                LogManager.Instance.Log("LU框架|获得模板", "没有找到模板文件{0}", ELogSeverity.Error, true, templatePath);
                return "";
            }

            // 获得模板
            string template = File.ReadAllText(templatePath);

            // 获得文件名
            string fileName = Path.GetFileNameWithoutExtension(pathName);

            // 获得有效字符串
            if (GetAvailableString(fileName, out string availableName))
            {
                fileName = UpperFirstChar(availableName);
            }

            // 获得命名空间名
            string namespaceStr = GetNamespaceName();

            // 是否有文件名处理函数
            if (fileNameHandler != null)
            {
                fileName = fileNameHandler(fileName);
            }

            // 替换对应标签
            template = template.Replace("#CREATE_DATE#", DateTime.Now.ToString("d"))
                .Replace("#DEVELOPER#", Config.DEVELOPER)
                .Replace("#NAMESPACE#", namespaceStr)
                .Replace("#PROJ_NAME#", Config.PROJ_NAME)
                .Replace("#SCRIPT_NAME#", fileName);

            // 新文件路径
            newPath = Path.Combine(Path.GetDirectoryName(pathName), fileName + Path.GetExtension(pathName));

            return template;
        }

        /// <summary>
        /// 获得注释拼接
        /// </summary>
        /// <param name="summary">注释内容 \n换行</param>
        /// <param name="returnMsg">返回注释</param>
        /// <param name="paramMsgArray">参数信息 传递参数名 再传递注释内容 即传递的始终为偶数</param>
        /// <returns>注释拼接</returns>
        public static string GetSummaryString(string summary, int tabNum = 2, string returnMsg = "", params string[] paramMsgArray)
        {
            StringBuilder sb = new StringBuilder();

            // 获得tab字符串
            string tab = "";
            tabNum = Mathf.Max(0, tabNum);
            for (int i = 0; i < tabNum; i++)
            {
                tab += "\t";
            }

            // 获得每行内容
            string[] lines = summary.Split('\n');

            // 拼接注释内容
            sb.Append(tab);
            sb.Append("/// <summary>\n");
            for (int i = 0; i < lines.Length; i++)
            {
                sb.Append(tab);
                sb.Append("/// ");
                sb.Append(lines[i]);
                sb.Append("\n");
            }
            sb.Append(tab);
            sb.Append("/// </summary>\n");

            // 是否有参数信息
            int count = Mathf.FloorToInt(paramMsgArray.Length * 0.5f);
            for (int i = 0; i < count; i++)
            {
                sb.Append(tab);
                sb.Append("/// <param name=\"");
                sb.Append(paramMsgArray[i * 2]);
                sb.Append("\"> ");
                sb.Append(paramMsgArray[i * 2 + 1]);
                sb.Append("</param>\n");
            }

            // 是否有返回信息
            if (!string.IsNullOrEmpty(returnMsg))
            {
                sb.Append(tab);
                sb.Append("/// <returns>");
                sb.Append(returnMsg);
                sb.Append("</returns>\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 递归获得组件
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="targetName">目标名</param>
        /// <returns>目标节点组件</returns>
        public static T GetComponentByRecursion<T>(Transform node, string targetName) where T : Component
        {
            T temp = null;

            // 是否是目标
            if (node.name == targetName)
            {
                return node.GetComponent<T>();
            }

            // 遍历子节点
            foreach (Transform item in node)
            {
                // 子节点再次调用
                temp = GetComponentByRecursion<T>(item, targetName);

                // 是否不为空
                if (temp != null)
                {
                    // 返回
                    return temp;
                }
            }

            return null;
        }
    }
}