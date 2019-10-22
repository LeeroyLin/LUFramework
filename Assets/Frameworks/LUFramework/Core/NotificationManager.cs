/*
 * 时间 : 2019/7/19
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 通知管理类
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 通知管理类
    /// </summary>
    public class NotificationManager : SingletonScript<NotificationManager> 
    {
        #region 公共字段
        #endregion

        #region 私有字段
        /// <summary>
        /// 观察者字典
        /// </summary>
        private Dictionary<string, List<Action<object>>> _observerDic = new Dictionary<string, List<Action<object>>>();
        #endregion

        #region 默认回调
        /// <summary>
        /// 唤醒后调用
        /// </summary>
        void Awake () 
	    {

	    }
        #endregion

        #region 其他方法
        /// <summary>
        /// 注册处理方法
        /// </summary>
        /// <param name="notificationTag">通知标签</param>
        /// <param name="handler">处理方法</param>
        public void RegisgerHandler(string notificationTag, Action<object> handler)
        {
            List<Action<object>> list;
            // 尝试获得对应的处理方法列表
            if (_observerDic.TryGetValue(notificationTag, out list))
            {
                // 获得该方法下标
                int index = GetHandlerIndex(list, handler);

                // 是否有该方法
                if (index != -1)
                {
                    // 替换
                    list[index] = handler;
                }
                else
                {
                    // 添加新的处理方法
                    list.Add(handler);
                }

                // 设置列表
                _observerDic[notificationTag] = list;
            }
            else
            {
                // 新建列表
                list = new List<Action<object>>
                {
                    handler
                };

                // 新建标签和列表
                _observerDic.Add(notificationTag, list);
            }
        }

        /// <summary>
        /// 移除方法
        /// </summary>
        /// <param name="notificationTag">通知名</param>
        /// <param name="handler">处理方法</param>
        public void RemoveHandler(string notificationTag, Action<object> handler)
        {
            List<Action<object>> list;
            // 尝试获得对应的处理方法列表
            if (_observerDic.TryGetValue(notificationTag, out list))
            {
                // 获得该方法下标
                int index = GetHandlerIndex(list, handler);

                // 是否有该处理方法
                if (index != -1)
                {
                    // 移除
                    list.Remove(list[index]);

                    // 是否列表为空
                    if (list.Count == 0)
                    {
                        // 移除该字典项
                        _observerDic.Remove(notificationTag);
                    }
                }
            }
        }

        /// <summary>
        /// 移除所有的处理方法
        /// </summary>
        /// <param name="notificationTag">通知名</param>
        public void RemoveAllHandlers(string notificationTag)
        {
            // 字典中是否有该项
            if (_observerDic.ContainsKey(notificationTag))
            {
                // 移除
                _observerDic.Remove(notificationTag);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="notificationTag">通知标签</param>
        /// <param name="args">参数</param>
        public void Execute(string notificationTag, object args = null)
        {
            List<Action<object>> list;
            // 尝试获得对应的处理方法列表
            if (_observerDic.TryGetValue(notificationTag, out list))
            {
                // 遍历所有的处理方法
                foreach (Action<object> item in list)
                {
                    // 回调
                    item(args);
                }
            }
        }

        /// <summary>
        /// 获取处理方法的下标
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="handler">处理方法</param>
        /// <returns>下标 没有则返回-1</returns>
        int GetHandlerIndex(List<Action<object>> list, Action<object> handler)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Method == handler.Method)
                {
                    return i;
                }
            }

            return -1;
        }
	    #endregion
    }
}
