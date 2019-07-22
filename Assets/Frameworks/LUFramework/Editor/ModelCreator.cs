/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 数据类创建类
 */

using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 数据类创建类
    /// </summary>
	public class ModelCreator
	{
        #region 右键菜单方法
        /// <summary>
        ///  创建脚本
        /// </summary>
        [MenuItem("Assets/Create/LUFramework/Model")]
        public static void CreateScript()
        {
            // 创建资源
            CreateAsset("NewModel.cs", Config.MODEL_TEMPLATE_PATH);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 创建资源
        /// </summary>
        /// <param name="defaultName">默认文件名</param>
        /// <param name="templatePath">模板路径</param>
        private static void CreateAsset(string defaultName, string templatePath)
        {
            // 获得路径
            string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            // 开始编辑名字
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<ScriptAsset>(),
                Path.Combine(path, defaultName),
                (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D),
                templatePath
            );
        }
        #endregion

        #region 资源类
        /// <summary>
        /// 脚本资源类
        /// </summary>
        public class ScriptAsset : EndNameEditAction
        {
            /// <summary>
            /// 名字编辑完后调用
            /// </summary>
            /// <param name="instanceId">实例Id</param>
            /// <param name="pathName">路径名</param>
            /// <param name="resourceFile">原资源文件</param>
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // 获得模板处理后的内容
                string content = Helper.GetTemplateContent(pathName, resourceFile, out string newPath, srcFileName => srcFileName);

                // 写入文件
                File.WriteAllText(newPath, content);
                AssetDatabase.ImportAsset(newPath);
            }
        } 
        #endregion
    }
}