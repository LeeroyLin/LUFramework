/*
 * 时间 : 2019/10/22
 * 作者 : Author
 * 项目 : ProjectName
 * 描述 : 控制类 - 控制层
 */

using System.Collections.Generic;
using UnityEngine;
using LUFramework;

namespace LUF
{
    /// <summary>
    /// 控制类 - 控制层
    /// </summary>
	public class TestFormController : BaseController 
	{
		#region 私有字段
		#endregion
				
		#region 重写方法
        /// <summary>
        /// 开始后回调
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
        }

        /// <summary>
        /// 注册处理方法
        /// </summary>
        protected override void RegisterHandlers()
        {
			// 注册BtnWillExport处理方法
			RegisterHandler(NotificationConfig.UI_TESTFORM_BTNWILLEXPORT, OnClickBtnWillExport);

		}
		#endregion
		
		#region 处理方法
		/// <summary>
		/// BtnWillExport处理方法
		/// </summary>
		/// <param name="args"> 参数</param>
		public void OnClickBtnWillExport(object args)
		{
			
		}
		#endregion
	}
}