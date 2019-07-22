/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 日志枚举
 */

namespace LUFramework
{
    /// <summary>
    /// 日志类型枚举
    /// </summary>
	public enum ELogType
	{
        /// <summary>
        /// 控制台
        /// </summary>
        Console,
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 控制台和文件
        /// </summary>
        ConsoleFile
	}

    /// <summary>
    /// 日志严重性枚举
    /// </summary>
    public enum ELogSeverity
    {
        /// <summary>
        /// 普通日志
        /// </summary>
        Log,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}