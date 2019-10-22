/*
 * 时间 : 2019/8/5
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 资源管理类
 */

using LUFramework;
using LUFramework.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 资源管理类
    /// </summary>
	public class AssetManager : SingletonScript<AssetManager>
	{
        #region 公共字段
        #endregion

        #region 私有字段
        /// <summary>
        /// 缓存列表
        /// </summary>
        private List<AssetBundleInfo> _cacheList = new List<AssetBundleInfo>();
        #endregion

        #region 默认回调
        /// <summary>
        /// 唤醒后回调
        /// </summary>
        void Awake()
        {
            // 注册处理方法
            RegisterHandlers();
        }
        #endregion

        #region 通知处理方法
        /// <summary>
        /// 注册处理方法
        /// </summary>
        void RegisterHandlers()
        {
            //NotificationManager.Instance.RegisgerHandler(NotificationConfig.CTRL_ASSET_LOAD, LoadAssetFromAB);
            //NotificationManager.Instance.RegisgerHandler(NotificationConfig.CTRL_ASSET_PRE_LOAD, PreLoadAssetBundle);
        }

        /// <summary>
        /// 加载资源的中转方法
        /// </summary>
        /// <param name="args">参数</param>
        void LoadAssetFromAB(object args)
        {
            // 转为数据类
            AssetData data = args as AssetData;

            // 加载资源
            LoadAssetFromAB(data.ABPath, data.AssetName, data.IsCache, data.FinishedHandler);
        }
        #endregion

        #region 资源包方法
        /// <summary>
        /// 预加载资源包
        /// </summary>
        /// <param name="abPath">资源包路径</param>
        public void PreLoadAssetBundle(object abPath)
        {
            StartCoroutine("PreLoadAssetBundleCoroutine", abPath);
        }
        /// <summary>
        /// 预加载资源包
        /// </summary>
        /// <param name="abPath">资源包路径</param>
        IEnumerator PreLoadAssetBundleCoroutine(string abPath)
        {
            // 加载ab包
            AssetBundleCreateRequest assetBundleReq = AssetBundle.LoadFromFileAsync(abPath);
            yield return assetBundleReq;

            // 是否完成了且有资源包
            if (assetBundleReq.isDone && assetBundleReq.assetBundle != null)
            {
                // 记录
                if (_cacheList.Find(p=>p.Path == abPath) == null)
                {
                    _cacheList.Add(new AssetBundleInfo(abPath, assetBundleReq.assetBundle));
                }
            }
        }

        /// <summary>
        /// 从资源包加载资源
        /// </summary>
        /// <param name="abPath">资源包路径</param>
        /// <param name="assetName">资源名</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="finishedHandler">完成处理方法</param>
        public void LoadAssetFromAB<T>(string abPath, string assetName, bool isCache, Action<T> finishedHandler) where T : class
        {
            // 尝试获得缓存中的资源包信息
            AssetBundleInfo assetBundleInfo = null;
            assetBundleInfo = _cacheList.Find(p => p.Path == abPath);
            
            // 是否有该资源包信息
            if (assetBundleInfo != null)
            {
                object assetObj;
                // 尝试获得资源
                if (assetBundleInfo.AssetDic.TryGetValue(assetName, out assetObj))
                {
                    // 直接回调
                    finishedHandler(assetObj as T);
                }
                else
                {
                    // 从资源包加载资源
                    StartCoroutine(LoadAssetFromABCoroutine(abPath, assetBundleInfo.AssetBundle, assetName, isCache, finishedHandler));
                }
            }
            else
            {
                // 加载资源包并加载资源
                StartCoroutine(LoadAssetBundleCoroutine(abPath, assetName, isCache, finishedHandler));
            }
        }

        /// <summary>
        /// 加载资源包的协程
        /// </summary>
        /// <param name="abPath">资源包路径</param>
        /// <param name="assetName">资源名</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="finishedHandler">完成处理方法</param>
        IEnumerator LoadAssetBundleCoroutine<T>(string abPath, string assetName, bool isCache, Action<T> finishedHandler) where T : class
        {
            // 加载ab包
            AssetBundleCreateRequest assetBundleReq = AssetBundle.LoadFromFileAsync(abPath);
            yield return assetBundleReq;

            // 是否完成了且有资源包
            if (assetBundleReq.isDone && assetBundleReq.assetBundle != null)
            {
                // 新建数据
                AssetBundleInfo assetBundleInfo = new AssetBundleInfo(abPath, assetBundleReq.assetBundle);

                // 记录
                _cacheList.Add(assetBundleInfo);

                // 有资源名
                if (!string.IsNullOrEmpty(assetName))
                {
                    // 从资源包加载资源
                    StartCoroutine(LoadAssetFromABCoroutine(abPath, assetBundleReq.assetBundle, assetName, isCache, finishedHandler));
                }
            }
        }

        /// <summary>
        /// 从资源包加载资源的协程
        /// </summary>
        /// <param name="abPath">资源包路径</param>
        /// <param name="assetBundle">资源包</param>
        /// <param name="assetName">资源名</param>
        /// <param name="isCache">是否缓存</param>
        /// <param name="finishedHandler">完成处理方法</param>
        IEnumerator LoadAssetFromABCoroutine<T>(string abPath, AssetBundle assetBundle, string assetName, bool isCache, Action<T> finishedHandler) where T : class
        {
            // 获得资源
            AssetBundleRequest assetReq = assetBundle.LoadAssetAsync(assetName);
            yield return assetReq;

            // 完成了
            if (assetReq.isDone)
            {
                // 回调
                if (finishedHandler != null)
                {
                    finishedHandler.Invoke(assetReq.asset as T);
                }

                // 是否缓存
                if (isCache)
                {
                    // 尝试获得缓存中的资源包信息
                    AssetBundleInfo assetBundleInfo = null;
                    assetBundleInfo = _cacheList.Find((e) => e.Path == abPath);

                    // 如果有该资源包
                    if (assetBundleInfo != null)
                    {
                        object assetObj;
                        // 是否没有该资源
                        if (!assetBundleInfo.AssetDic.TryGetValue(assetName, out assetObj))
                        {
                            // 添加到字典
                            assetBundleInfo.AssetDic.Add(assetName, assetReq.asset);
                        }
                    }
                    else
                    {
                        // 新建数据
                        assetBundleInfo = new AssetBundleInfo(abPath, assetBundle);

                        // 添加到字典
                        assetBundleInfo.AssetDic.Add(assetName, assetReq.asset);
                    }
                }
            }
        }

        /// <summary>
        /// 移除资源包缓存
        /// </summary>
        /// <param name="abPath">资源包</param>
        public void RemoveAssetBundleCache(string abPath)
        {
            // 尝试获得缓存中的资源包信息
            AssetBundleInfo assetBundleInfo = null;
            assetBundleInfo = _cacheList.Find((e) => e.Path == abPath);

            // 如果有该资源信息
            if (assetBundleInfo != null)
            {
                // 移除
                _cacheList.Remove(assetBundleInfo);
            }
        }

        /// <summary>
        /// 移除资源缓存
        /// </summary>
        /// <param name="abPath"></param>
        /// <param name="assetName"></param>
        public void RemoveAssetCache(string abPath, string assetName)
        {
            // 尝试获得缓存中的资源包信息
            AssetBundleInfo assetBundleInfo = null;
            assetBundleInfo = _cacheList.Find((e) => e.Path == abPath);

            // 如果有该资源信息
            if (assetBundleInfo != null)
            {
                // 是否有该资源
                if (assetBundleInfo.AssetDic.ContainsKey(assetName))
                {
                    // 移除
                    assetBundleInfo.AssetDic.Remove(assetName);
                }
            }
        }
        #endregion
    }
}