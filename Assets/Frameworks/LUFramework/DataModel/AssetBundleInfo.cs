/*
 * 时间 : 2019/8/5
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 资源包信息类
 */

using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 资源包信息类
    /// </summary>
	public class AssetBundleInfo
    {
        #region 属性
        /// <summary>
        /// 资源包路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 资源包
        /// </summary>
        public AssetBundle AssetBundle { get; set; }
        
        /// <summary>
        /// 资源字典
        /// </summary>
        public Dictionary<string, object> AssetDic { get; set; }
		#endregion
		
		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
        /// <param name="path">资源包路径</param>
        /// <param name="assetBundle">资源包</param>
		public AssetBundleInfo(string path, AssetBundle assetBundle)
		{
            Path = path;
            AssetBundle = assetBundle;
            AssetDic = new Dictionary<string, object>();
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 比较方法
        /// </summary>
        /// <param name="obj">目标</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            return (obj as AssetBundleInfo).Path == Path;
        }
        #endregion
    }
}