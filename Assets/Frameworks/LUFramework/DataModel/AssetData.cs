/*
 * 时间 : 2019/8/6
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 资源数据
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 资源数据
    /// </summary>
	public class AssetData
    {
        /// <summary>
        /// 资源包路径
        /// </summary>
        public string ABPath { get; set; }

        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; }

        /// <summary>
        /// 加载完毕回调
        /// </summary>
        public Action<UnityEngine.Object> FinishedHandler { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetData(string abPath, string assetName, bool isCache, Action<UnityEngine.Object> finishedHandler)
        {
            ABPath = abPath;
            AssetName = assetName;
            IsCache = isCache;
            FinishedHandler = finishedHandler;
        }
    }
}