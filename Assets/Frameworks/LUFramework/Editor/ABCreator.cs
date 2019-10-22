/*
 * 时间 : 2019/8/5
 * 作者 : LeeroyLin
 * 项目 : 传奇地牢
 * 描述 : 资源包创建类
 */

using LUFramework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LD
{
    /// <summary>
    /// 资源包创建类
    /// </summary>
	public class ABCreator : EditorWindow
    {
        #region 私有方法
        /// <summary>
        /// 原路径
        /// </summary>
        private string _sourcePath;

        /// <summary>
        /// 目标路径
        /// </summary>
        private string _targetPath;

        /// <summary>
        /// 打包配置
        /// </summary>
        private BuildAssetBundleOptions _buildABOptions = BuildAssetBundleOptions.ChunkBasedCompression;

        /// <summary>
        /// 打包目标平台
        /// </summary>
        private BuildTarget _buildTarget = BuildTarget.StandaloneWindows64;

        /// <summary>
        /// 打包列表
        /// </summary>
        private List<AssetBundleBuild> _buildList = new List<AssetBundleBuild>();
        #endregion

        #region 菜单栏方法
        /// <summary>
        /// 打包
        /// </summary>
        [MenuItem("LUFramework/AssetBundle/Build")]
        public static void CreateAssetBundle()
        {
            GetWindow(typeof(ABCreator));
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
            GUILayout.Label("AssetBundle Creator");

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            // 原路径
            _sourcePath = EditorGUILayout.TextField("Assets source path : ", _sourcePath);

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
            _targetPath = EditorGUILayout.TextField("Assetbundle target path : ", _targetPath);

            // 原路径选择按钮
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                // 选择路径
                _targetPath = FileSelector.GetSelectFileName();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            // 压缩方式
            _buildABOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Compression", _buildABOptions);

            GUILayout.Space(10);

            // 目标平台
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Target platform", _buildTarget);

            GUILayout.Space(20);

            // 打包
            if (GUILayout.Button("Build", GUILayout.Height(20)))
            {
                CreateBuildList();
                Build();
            }

            GUILayout.EndVertical();
        }
        #endregion

        #region 其他方法
        /// <summary>
        /// 创建打包列表
        /// </summary>
        public void CreateBuildList()
        {
            // 是否原路径不存在
            if (!Directory.Exists(_sourcePath))
            {
                LogManager.Instance.Log("[LU框架|自动设置AB标签]", "没有目录{0}", ELogSeverity.Log, true, _sourcePath);
                return;
            }

            // 重置列表
            _buildList.Clear();

            // 遍历路径下的文件
            string[] filePathArray;

            // 遍历顶层文件夹
            string[] topDirectoriesArray = Directory.GetDirectories(_sourcePath);

            // 资源标签列表
            List<string> abTagList = new List<string>();
            
            // 打包对象
            AssetBundleBuild build;

            // 遍历顶层文件夹
            foreach (var topDirectory in topDirectoriesArray)
            {
                // 获得该文件夹下的文件
                filePathArray = Directory.GetFiles(topDirectory, "*.*", SearchOption.AllDirectories);

                // 新打包对象
                build = new AssetBundleBuild
                {
                    assetBundleName = topDirectory.Replace(_sourcePath, "").Substring(1)
                };

                // 遍历文件路径
                foreach (var path in filePathArray)
                {
                    // 忽略.meta
                    if (Path.GetExtension(path) == ".meta")
                    {
                        continue;
                    }

                    // 添加到列表
                    abTagList.Add(path.Substring(path.IndexOf("Assets")));
                }

                // 设置打包类中
                build.assetNames = abTagList.ToArray();
                abTagList.Clear();

                // 添加到列表
                _buildList.Add(build);
            }

        }

        /// <summary>
        /// 创建包
        /// </summary>
        public void Build()
        {
            // 目标目录为空
            if (string.IsNullOrEmpty(_targetPath))
            {
                LogManager.Instance.Log("[LU框架|打AB包]", "目标目录不能为空", ELogSeverity.Log, true);
                return;
            }

            // 是否没有目标目录
            if (!Directory.Exists(_targetPath))
            {
                // 创建目录
                Directory.CreateDirectory(_targetPath);
            }

            // 打包
            BuildPipeline.BuildAssetBundles(_targetPath, _buildList.ToArray(), _buildABOptions, _buildTarget);

            LogManager.Instance.Log("[LU框架|打AB包]", "打包完毕", ELogSeverity.Log, true);

            // 刷新资源
            AssetDatabase.Refresh();
        }
        #endregion
    }
}