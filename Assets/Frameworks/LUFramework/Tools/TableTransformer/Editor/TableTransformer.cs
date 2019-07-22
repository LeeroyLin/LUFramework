/*
 * 时间 : 2019/7/16
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 表转换器
 */

using LUFramework;
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
            StringBuilder sb = new StringBuilder();

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
            // 单元格内容
            string cellValue = "";
            // 项类型
            string cellType = "";
            
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

                // 获得文件名
                fileName = GetAvailableName(Path.GetFileNameWithoutExtension(filePath));

                // 获得表名
                tableName = table.Rows[0][0].ToString();

                // 初始化列表
                titleList = new List<string>();

                // 获得标题
                sb = new StringBuilder();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    // 获得单元格内容
                    cellValue = GetAvailableName(table.Rows[3][i].ToString());

                    // 添加到标题列表
                    titleList.Add(cellValue);

                    // 添加到结构体内容中
                    sb.Append(Helper.GetSummaryString(table.Rows[1][i].ToString()));
                    sb.Append("\t\tpublic ");
                    sb.Append(table.Rows[2][i].ToString());
                    sb.Append(" ");
                    sb.Append(cellValue);
                    sb.Append(" { get; set; }");

                    // 是否不是最后
                    if (i != table.Columns.Count - 1)
                    {
                        sb.Append("\n");
                    }
                }

                structContent = sb.ToString();

                // 遍历项
                sb = new StringBuilder();
                for (int i = 4; i < table.Rows.Count; i++)
                {
                    sb.Append("\t\t\t// ");
                    sb.Append(i - 4);
                    sb.Append("\n\t\t\tnew ");
                    sb.Append(fileName);
                    sb.Append("Item()\n\t\t\t{\n");

                    for (int t = 0; t < table.Columns.Count; t++)
                    {
                        // 获得项类型
                        cellType = table.Rows[2][t].ToString().ToLower();

                        sb.Append("\t\t\t\t");
                        sb.Append(titleList[t]);
                        sb.Append(" = ");

                        // 判断类型
                        switch (cellType)
                        {
                            case "string":
                            {
                                sb.Append("\"");
                                sb.Append(table.Rows[i][t].ToString());
                                sb.Append("\"");
                            }
                            break;
                            case "int":
                            {
                                sb.Append(table.Rows[i][t].ToString());
                            }
                            break;
                            case "float":
                            {
                                sb.Append(table.Rows[i][t].ToString());
                                sb.Append("f");
                            }
                            break;
                        }

                        // 是否不是最后
                        if (t != table.Columns.Count - 1)
                        {
                            sb.Append(",\n");
                        }
                        else
                        {
                            sb.Append("\n");
                        }
                    }

                    // 是否不是最后
                    if (i != table.Rows.Count - 1)
                    {
                        sb.Append("\t\t\t},\n");
                    }
                    else
                    {
                        sb.Append("\t\t\t}");
                    }
                }

                listContent = sb.ToString();

                // 获得路径
                string path = Path.Combine(_targetPath.Substring(_targetPath.IndexOf("Assets")), fileName + ".cs");

                // 拼接模版
                dataContent = Helper.GetTemplateContent(path, Config.TABLE_TEMPLATE_PATH, out string newPath, null);

                // 替换内容
                dataContent = dataContent.Replace("#Item_Data#", listContent).Replace("#Item_Struct#", structContent);

                // 写入文件
                File.WriteAllText(newPath, dataContent);
                AssetDatabase.ImportAsset(newPath);
            }
        }

        /// <summary>
        /// 获得有效的名字
        /// 首字母大写，如果是数字首字母为下划线
        /// </summary>
        /// <param name="src">原字符串</param>
        /// <returns>有效的名字</returns>
        private string GetAvailableName(string src)
        {
            int ascii = src[0];

            // 如果是数字
            if (ascii >= 48 && ascii <= 57)
            {
                return "_" + src;
            }
            else
            {
                return (src[0] + "").ToUpper() + src.Substring(1);
            }
        }
        #endregion
    }
}