/*
 * 时间 : 2019/10/22
 * 作者 : Author
 * 项目 : ProjectName
 * 描述 : 窗体类 - 视图层
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LUFramework;

namespace LUF
{
    /// <summary>
    /// 窗体类 - 视图层
    /// </summary>
	public class TestForm : BaseForm 
	{
		#region UI对象
		/// <summary>
		/// 按钮
		/// </summary>
		private Button _btnWillExport;
		/// <summary>
		/// 文本
		/// </summary>
		private Text _textMsg;
		#endregion
	
		#region 私有字段
		#endregion
				
		#region 默认回调
		/// <summary>
		/// 开始后调用
		/// </summary>
		new void Start () 
		{
            base.Start();
		}
		#endregion
		
		#region 重写方法
        /// <summary>
        /// 获得UI控件
        /// </summary>
        protected override void GetUI()
        {
			_btnWillExport = Helper.GetComponentByRecursion<Button>(transform, "_btnWillExport");
			_textMsg = Helper.GetComponentByRecursion<Text>(transform, "_TextMsg");
		}

        /// <summary>
        /// 绑定事件
        /// </summary>
        protected override void BindEvent()
        {
			// 绑定_btnWillExport事件
			_btnWillExport.onClick.AddListener(OnClickBtnWillExport);
		}

        /// <summary>
        /// 注册处理方法
        /// </summary>
        protected override void RegisterHandlers()
        {
			
		}
		#endregion
		
		#region UI事件方法
		/// <summary>
		/// 按钮_btnWillExport点击事件处理方法
		/// </summary>
		public void OnClickBtnWillExport()
		{
			// 发送通知
			SendNotification(NotificationConfig.UI_TESTFORM_BTNWILLEXPORT);
		}
		#endregion
	}
}