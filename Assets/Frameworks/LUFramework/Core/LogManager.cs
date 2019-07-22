/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 日志管理类
 */

using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 日志管理类
    /// </summary>
	public class LogManager : SingletonScript<LogManager>
	{
        #region 公共字段
        /// <summary>
        /// 日志消息类型
        /// </summary>
        public ELogType logType = ELogType.Console;
        #endregion

        #region 私有字段
        /// <summary>
        /// 日志文件名
        /// </summary>
        string _logFileName;
		#endregion
		
		#region 默认回调
		/// <summary>
		/// 唤醒后调用
		/// </summary>
		void Awake () 
		{
            _logFileName = "";
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="logSeverity">日志严重程度</param>
        private void ShowLog(string msg, ELogSeverity logSeverity)
        {
            // 根据不同严重程度打印
            switch (logSeverity)
            {
                case ELogSeverity.Log:
                    Debug.Log(msg);
                    break;
                case ELogSeverity.Warning:
                    Debug.LogWarning(msg);
                    break;
                case ELogSeverity.Error:
                    Debug.LogError(msg);
                    break;
            }
        }

        /// <summary>
        /// 存储日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="logSeverity">日志严重程度</param>
        private void SaveLog(string msg, ELogSeverity logSeverity)
        {
            // 拼接字符串
            StringBuilder sb = new StringBuilder();
            sb.Append(Config.LOG_PATH);
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd"));

            // 存储的路径
            string path = sb.ToString();

            // 是否该路径不存在
            if (!Directory.Exists(path))
            {
                // 创建路径
                Directory.CreateDirectory(path);
            }

            // 当前是否没有日志
            if (string.IsNullOrEmpty(_logFileName))
            {
                _logFileName = DateTime.Now.ToString("HH-mm-ss");
            }

            // 文件路径
            sb.Append("/");
            sb.Append(_logFileName);
            sb.Append(".txt");
            path = sb.ToString();

            // 写入该文件
            WriteToFile(string.Format("{0} : #{1}# {2}", DateTime.Now.ToString("HH:mm:ss"), logSeverity.ToString(), msg), path);
        }

        /// <summary>
        /// 写到文件
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="path">路径</param>
        private void WriteToFile(string msg, string path)
        {
            // 打开文件流 写入
            using (FileStream fsWriter = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                // 获得字节数组
                byte[] byteArray = Encoding.UTF8.GetBytes(msg);

                // 写入
                fsWriter.Write(byteArray, 0, byteArray.Length);
            }

        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 打日子
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="msg">消息</param>
        /// <param name="logSeverity">日志严重程度</param>
        /// <param name="isDebug">是否调试</param>
        /// <param name="args">参数</param>
        public void Log(string title, string msg, ELogSeverity logSeverity, bool isDebug, params object[] args)
        {
            // 是否是调试信息 且 当前不在编辑器
            if (isDebug && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.LinuxEditor && Application.platform != RuntimePlatform.OSXEditor)
            {
                return;
            }

            // 拼接日志内容
            string logMsg = string.Format(Config.LOG_PATTERN, title, string.Format(msg, args));

            // 是否打印
            if (logType != ELogType.File)
            {
                ShowLog(logMsg, logSeverity);
            }

            // 是否存储到文件
            if (logType != ELogType.Console)
            {
                SaveLog(logMsg, logSeverity);
            }
        }
        #endregion
    }
}