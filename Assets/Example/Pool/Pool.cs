/*
 * 时间 : 2019/10/22
 * 作者 : Author
 * 项目 : ProjectName
 * 描述 : 
 */

using LUFramework;
using System.Collections.Generic;
using UnityEngine;

namespace LUF
{
    /// <summary>
    /// 
    /// </summary>
	public class Pool : MonoBehaviour 
	{
        #region 公共字段
        #endregion

        #region 私有字段
        GameObject obj;
		#endregion
		
		#region 默认回调
		/// <summary>
		/// 开始后调用
		/// </summary>
		void Start () 
		{
            obj = PoolManager.Instance.TryGet("Tag", GetObj);

            Invoke("Recover", 3);
		}
		
		/// <summary>
		/// 每帧调用
		/// </summary>
		void Update () 
		{
		    
		}
		#endregion
		
		#region 其他方法
        /// <summary>
        /// 生成方法
        /// </summary>
        /// <returns></returns>
        GameObject GetObj()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        /// <summary>
        /// 回收
        /// </summary>
        void Recover()
        {
            PoolManager.Instance.Recover("Tag", obj);
        }
		#endregion
	}
}