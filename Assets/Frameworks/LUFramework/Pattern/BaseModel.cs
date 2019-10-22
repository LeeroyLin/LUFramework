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
	public class BaseModel<T> : SingletonClass<T> where T : class, new()
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
        }
        #endregion
        
        #region 虚方法
        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual void InitData()
        {

        }

        /// <summary>
        /// 存储到本地
        /// </summary>
        public virtual void Save2Local()
        {

        }
        #endregion
    }
}