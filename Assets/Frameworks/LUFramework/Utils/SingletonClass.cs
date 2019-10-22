/*
 * 时间 : 2019/7/5
 * 作者 : LeeroyLin
 * 描述 : 单例类基类
 */
using UnityEngine;

/// <summary>
/// 单例类基类
/// </summary>
public class SingletonClass<T> where T : class, new()
{
    #region 私有字段
    /// <summary>
    /// 单例对象
    /// </summary>
    private static T _instance;
    #endregion

    #region 属性
    /// <summary>
    /// 获得单例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
	#endregion
}