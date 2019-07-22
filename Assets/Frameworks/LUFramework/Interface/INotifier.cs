/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 通知接口
 */

using System;

namespace LUFramework.Interface
{
    /// <summary>
    /// 通知接口
    /// </summary>
	public interface INotifier 
	{
        /// <summary>
        /// 注册处理函数
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="handler">处理方法</param>
        void RegisterHandler(string tag, Action<object> handler);

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="tag">通知标签</param>
        /// <param name="args">参数</param>
        void SendNotification(string tag, object args = null);
	}
}