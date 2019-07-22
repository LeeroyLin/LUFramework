/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 脚本创建类
 */

using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 脚本创建类
    /// </summary>
	public class ScriptCreator
	{
        #region 私有字段
        /// <summary>
        /// 是否是接口
        /// </summary>
        private static bool _isInterface = false;

        /// <summary>
        /// 是否是枚举
        /// </summary>
        private static bool _isEnum = false;
        #endregion

        #region 右键菜单方法
        /// <summary>
        ///  创建脚本
        /// </summary>
        [MenuItem("Assets/Create/LUFramework/Script")]
        public static void CreateScript()
        {
            // 创建资源
            CreateAsset("NewLUFScript.cs", Config.SCRIPT_TEMPLATE_PATH);

            // 标记
            _isInterface = false;
            _isEnum = false;
        }

        /// <summary>
        ///  创建类
        /// </summary>
        [MenuItem("Assets/Create/LUFramework/Class")]
        public static void CreateClass()
        {
            // 创建资源
            CreateAsset("NewLUFClass.cs", Config.CLASS_TEMPLATE_PATH);

            // 标记
            _isInterface = false;
            _isEnum = false;
        }

        /// <summary>
        ///  创建枚举
        /// </summary>
        [MenuItem("Assets/Create/LUFramework/Enum")]
        public static void CreateEnum()
        {
            // 创建资源
            CreateAsset("NewLUFEnum.cs", Config.ENUM_TEMPLATE_PATH);

            // 标记
            _isInterface = false;
            _isEnum = true;
        }

        /// <summary>
        ///  创建接口
        /// </summary>
        [MenuItem("Assets/Create/LUFramework/Interface")]
        public static void CreateInterface()
        {
            // 创建资源
            CreateAsset("NewLUFInterface.cs", Config.ENUM_TEMPLATE_PATH);

            // 标记
            _isInterface = true;
            _isEnum = false;
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
                string content = Helper.GetTemplateContent(pathName, resourceFile, out string newPath, srcFileName => {
                    // 如果是接口
                    if (_isInterface)
                    {
                        return Helper.SetStartWith(srcFileName, "I");
                    }
                    // 如果是枚举
                    if (_isEnum)
                    {
                        return Helper.SetStartWith(srcFileName, "E");
                    }

                    return srcFileName;
                });

                // 写入文件
                File.WriteAllText(newPath, content);
                AssetDatabase.ImportAsset(newPath);
            }
        } 
        #endregion
    }
}