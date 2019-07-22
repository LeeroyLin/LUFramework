/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 数据模型基类
 */

using LUFramework.Interface;
using System;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 数据模型基类
    /// </summary>
	public abstract class BaseModel : INotifier 
	{
        #region 保护字段
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseModel()
        {
            // 初始化数据
            InitData();

            // 注册处理函数
            RegisterHandlers();
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 初始化数据
        /// </summary>
        public abstract void InitData();
        #endregion

        #region 虚方法
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
    }
}