/*
 * 时间 : 2019/7/5
 * 作者 : LeeroyLin
 * 描述 : 单例脚本基类
 */
using UnityEngine;

/// <summary>
/// 单例脚本基类
/// </summary>
public class SingletonScript<T> : MonoBehaviour where T : class 
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
            if(_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
            }
            return _instance;
        }
    }
    #endregion

    #region 默认回调
    /// <summary>
    /// 当被销毁
    /// </summary>
    public void OnDestroy()
    {
        _instance = null;
    }
    #endregion
}