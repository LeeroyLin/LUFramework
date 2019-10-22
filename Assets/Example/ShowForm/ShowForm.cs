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
	public class ShowForm : MonoBehaviour 
	{
		#region 公共字段
		#endregion
		
		#region 私有字段
		#endregion
		
		#region 默认回调
		/// <summary>
		/// 开始后调用
		/// </summary>
		void Start () 
		{
            UIManager.Instance.ShowForm("TestForm", EFormType.Normal, EFormDisplayType.Single, EFormModalType.Lucency);
		}
		
		/// <summary>
		/// 每帧调用
		/// </summary>
		void Update () 
		{
		
		}
		#endregion
		
		#region 其他方法
		#endregion
	}
}