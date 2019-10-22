/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 窗体基类
 */

using LUFramework.Interface;
using System;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 窗体基类
    /// </summary>
	public abstract class BaseController : MonoBehaviour, INotifier 
	{
		#region 保护字段
		#endregion
		
		#region 默认回调
        /// <summary>
        /// 唤醒后调用
        /// </summary>
        void Awake()
        {
            // 唤醒后调用
            OnAwake();
        }

        /// <summary>
        /// 开始后调用
        /// </summary>
        void Start()
        {
            // 注册处理函数
            RegisterHandlers();

            // 开始后调用
            OnStart();
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 唤醒后调用
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// 开始后调用
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// 注册处理函数
        /// </summary>
        protected virtual void RegisterHandlers() { }
        #endregion

        #region 接口实现方法
        /// <summary>
        /// 注册处理函数
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="handler">处理方法</param>
        public void RegisterHandler(string tag, Action<object> handler)
        {
            NotificationManager.Instance.RegisgerHandler(tag, handler);
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="args"></param>
        public void SendNotification(string tag, object args = null)
        {
            NotificationManager.Instance.Execute(tag, args);
        }
        #endregion

        #region 其他方法
        #endregion
    }
}