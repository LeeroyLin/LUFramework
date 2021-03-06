/*
 * 时间 : 2019/7/16
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 表转换器
 */

using LUFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 表转换器
    /// </summary>
	public class TableTransformer : EditorWindow
	{
        #region 公共字段
        #endregion

        #region 私有字段
        /// <summary>
        /// 表原路径
        /// </summary>
        private string _sourcePath = "";

        /// <summary>
        /// 表目标路径
        /// </summary>
        private string _targetPath = "";
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        TableTransformer()
        {
            titleContent = new GUIContent("Table Transformer");
        }
        #endregion

        #region 菜单栏方法
        /// <summary>
        /// 转换表菜单栏
        /// </summary>
        [MenuItem("LUFramework/TableTransformer/Transform")]
        public static void TransformTable()
        {
            GetWindow(typeof(TableTransformer));
        }
        #endregion

        #region UI
        /// <summary>
        /// GUI渲染
        /// </summary>
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            
            // 原路径标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 18;
            GUILayout.Label("Table Transformer");

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            // 原路径
            _sourcePath = EditorGUILayout.TextField("Table source path : ", _sourcePath);

            // 原路径选择按钮
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                // 选择路径
                _sourcePath = FileSelector.GetSelectFileName();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            // 目标路径
            _targetPath = EditorGUILayout.TextField("Table target path : ", _targetPath);

            // 目标路径选择按钮
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                // 选择路径
                _targetPath = FileSelector.GetSelectFileName();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            // 如果有空的
            if (string.IsNullOrEmpty(_sourcePath) || string.IsNullOrEmpty(_targetPath))
            {
                // 错误提示
                GUI.skin.label.fontSize = 12;
                GUILayout.Label("Field can't be empty.");

            }
            else
            {
                // 转换按钮
                if (GUILayout.Button("Start Transform", GUILayout.Height(20)))
                {
                    // 转换
                    StartTransform();
                }
            }

            GUILayout.EndVertical();
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 开始转换
        /// </summary>
        public void StartTransform()
        {
            // 是否没有原路径
            if (!Directory.Exists(_sourcePath))
            {
                LogManager.Instance.Log("[LU框架|转表]", "没有目录{0}", ELogSeverity.Log, true, _sourcePath);
                return;
            }

            // 遍历表原路径的所有xlsx文件
            string[] filePathArray = Directory.GetFiles(_sourcePath, "*.xlsx");

            // 如果没有xlsx文件
            if (filePathArray.Length == 0)
            {
                LogManager.Instance.Log("[LU框架|转表]", "目录{0}没有xlsx文件", ELogSeverity.Log, true, _sourcePath);
                return;
            }

            LogManager.Instance.Log("[LU框架|转表]", "将{0}中的xlsx表转到{1}中...", ELogSeverity.Log, true, _sourcePath, _targetPath);

            // 表名
            string tableName = "";
            // 文件名
            string fileName = "";
            // 标题列表
            List<string> titleList = new List<string>();
            // 结构体内容
            string structContent = "";
            // 列表内容
            string listContent = "";
            // 数据内容
            string dataContent = "";

            // 遍历所有路径
            foreach (var filePath in filePathArray)
            {
                // 读取文件
                DataTable table = ExcelReader.ReadExcel(filePath);

                // 是否行数小于5
                if (table.Rows.Count < 5)
                {
                    continue;
                }

                // 获得表名
                tableName = table.Rows[0][0].ToString();

                // 初始化列表
                titleList = new List<string>();

                // 获得文件名
                fileName = Path.GetFileNameWithoutExtension(filePath);

                // 获得路径
                string path = Path.Combine(_targetPath.Substring(_targetPath.IndexOf("Assets")), fileName + ".cs");

                // 修改
                fileName = GetAvailableName(fileName);

                // 拼接结构体内容
                structContent = JointStructContent(table, titleList, fileName);

                // 拼接列表内容
                listContent = JointListContent(table, titleList, fileName);

                // 拼接模版
                string newPath;
                dataContent = Helper.GetTemplateContent(path, Config.TABLE_TEMPLATE_PATH, out newPath, null);

                // 替换内容
                dataContent = dataContent.Replace("#Item_Data#", listContent).Replace("#Item_Struct#", structContent);

                // 写入文件
                File.WriteAllText(newPath, dataContent);
                AssetDatabase.ImportAsset(newPath);
            }
        }

        /// <summary>
        /// 拼接结构体内容
        /// </summary>
        /// <param name="table">表对象</param>
        /// <param name="titleList">标题列表</param>
        /// <param name="fileName">文件名</param>
        /// <returns>拼接后的结构体内容</returns>
        private string JointStructContent(DataTable table, List<string> titleList, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            List<string> typeList = new List<string>();

            string cellValue = "";

            string valueTypeStr = "";

            for (int i = 0; i < table.Columns.Count; i++)
            {
                // 获得单元格内容
                cellValue = GetAvailableName(table.Rows[3][i].ToString());

                // 添加到标题列表
                titleList.Add(cellValue);

                valueTypeStr = table.Rows[2][i].ToString();
                typeList.Add(valueTypeStr);
                // 是否是数组
                if (valueTypeStr.Contains("[") || valueTypeStr.Contains("]"))
                {
                    valueTypeStr = valueTypeStr.Replace("[", "").Replace("]", "");
                    valueTypeStr = "IReadOnlyList<" + valueTypeStr + ">";
                }
                
                // 添加到结构体内容中
                sb.Append(Helper.GetSummaryString(table.Rows[1][i].ToString()));
                sb.Append("\t\tpublic readonly ");
                sb.Append(valueTypeStr);
                sb.Append(" ");
                sb.Append(cellValue);
                sb.Append(";");

                sb.Append("\n");

                // 是否是最后
                if (i == table.Columns.Count - 1)
                {
                    sb.Append("\n\t\tpublic ");
                    sb.Append(fileName);
                    sb.Append("Item(");

                    for (int t = 0; t < typeList.Count; t++)
                    {
                        sb.Append(typeList[t]);
                        sb.Append(" ");
                        sb.Append(titleList[t]);

                        // 如果不为最后
                        if (t != typeList.Count - 1)
                        {
                            sb.Append(", ");
                        }
                    }

                    sb.Append(")\n");
                    sb.Append("\t\t{\n");

                    for (int t = 0; t < typeList.Count; t++)
                    {
                        sb.Append("\t\t\tthis.");
                        sb.Append(titleList[t]);
                        sb.Append(" = ");
                        sb.Append(titleList[t]);
                        sb.Append(";\n");
                    }

                    sb.Append("\t\t}\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 拼接列表内容
        /// </summary>
        /// <param name="table">表对象</param>
        /// <param name="titleList">标题列表</param>
        /// <param name="fileName">文件名</param>
        /// <returns>拼接后的列表内容</returns>
        private string JointListContent(DataTable table, List<string> titleList, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            string cellType = "";

            // 遍历项
            for (int i = 4; i < table.Rows.Count; i++)
            {
                sb.Append("\t\t\t// ");
                sb.Append(i - 4);
                sb.Append("\n\t\t\tnew ");
                sb.Append(fileName);
                sb.Append("Item(\n");

                for (int t = 0; t < table.Columns.Count; t++)
                {
                    // 获得项类型
                    cellType = table.Rows[2][t].ToString().ToLower();

                    sb.Append("\t\t\t\t");

                    // 是否是数组
                    if (cellType.Contains("["))
                    {
                        SetArrayString(sb, cellType, table.Rows[i][t].ToString().Replace("\"", "'"));
                    }
                    else
                    {
                        // 判断类型
                        switch (cellType)
                        {
                            case "string":
                            {
                                SetCenterString(sb, table.Rows[i][t].ToString().Replace("\"", "'"), "\"", "\"", str => str);
                            }
                            break;
                            case "int":
                            {
                                SetCenterString(sb, table.Rows[i][t].ToString(), "", "", str => str == "" ? "0" : str);
                            }
                            break;
                            case "float":
                            {
                                SetCenterString(sb, table.Rows[i][t].ToString(), "", "f", str => str == "" ? "0" : str);
                            }
                            break;
                        }
                    }

                    // 是否不是最后
                    if (t != table.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }

                    sb.Append("\n");
                }

                sb.Append("\t\t\t)");

                // 是否不是最后
                if (i != table.Rows.Count - 1)
                {
                    sb.Append(",\n");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获得数组字符串
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="type">类型</param>
        /// <param name="content">内容</param>
        /// <returns>拼接后的数组字符串</returns>
        private void SetArrayString(StringBuilder sb, string type, string content)
        {
            sb.Append("new ");
            sb.Append(type);
            sb.Append(" {\n");
            
            // 取出括号
            content = content.Replace("[", "").Replace("]", "").Replace(" ", "");

            // 获得每一项数据
            string[] datas = content.Split(',');

            if (datas.Length > 0 && datas[0] != "")
            {
                switch (type)
                {
                    case "int[]":
                    {
                        SetArrayItemString(sb, datas, "", "", str => str);
                    }
                    break;
                    case "float[]":
                    {
                        SetArrayItemString(sb, datas, "", "f", str => str);
                    }
                    break;
                    case "string[]":
                    {
                        SetArrayItemString(sb, datas, "\"", "\"", str => str);
                    }
                    break;
                }
            }

            sb.Append("\t\t\t\t}");
        }
            
        /// <summary>
        /// 获得数组项字符串
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="datas">数据数组</param>
        /// <param name="beforeString">开始的字符串</param>
        /// <param name="afterString">结尾的字符串</param>
        /// <param name="handlerFunc">内容处理函数</param>
        /// <returns>数组项字符串</returns>
        private void SetArrayItemString<T>(StringBuilder sb, string[] datas, string beforeString, string afterString, Func<string, T>handlerFunc)
        {
            // 遍历每一项
            for (int i = 0; i < datas.Length; i++)
            {
                sb.Append("\t\t\t\t\t");
                SetCenterString(sb, datas[i], beforeString, afterString, handlerFunc);
                // 是否不是最后
                if (i != datas.Length - 1)
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
        }

        /// <summary>
        /// 获得中间字符串
        /// </summary>
        /// <param name="sb">字符串构造器</param>
        /// <param name="strSrc">原字符串</param>
        /// <param name="beforeString">之前字符串</param>
        /// <param name="afterString">之后字符串</param>
        /// <param name="handlerFunc">处理函数</param>
        /// <returns>处理后的字符串</returns>
        private void SetCenterString<T>(StringBuilder sb, string strSrc, string beforeString, string afterString, Func<string, T> handlerFunc)
        {
            sb.Append(beforeString);
            sb.Append(handlerFunc(strSrc));
            sb.Append(afterString);
        }

        /// <summary>
        /// 获得有效的名字
        /// 首字母大写，如果是数字首字母为下划线
        /// </summary>
        /// <param name="src">原字符串</param>
        /// <returns>有效的名字</returns>
        private string GetAvailableName(string src)
        {
            // 去掉首尾空格
            src = src.Trim();

            int ascii = src[0];

            // 如果是数字
            if (ascii >= 48 && ascii <= 57)
            {
                src = "_" + src;
            }
            else
            {
                src = (src[0] + "").ToUpper() + src.Substring(1);
            }

            // 查找空格
            int index = src.IndexOf(" ");
            while (index != -1)
            {
                // 将下一个字母大写 并 忽略掉空格
                src = src.Substring(0, index) + (src[index + 1] + "").ToUpper() + src.Substring(index + 2);

                // 查找空格
                index = src.IndexOf(" ");
            }

            return src;
        }
        #endregion
    }
}