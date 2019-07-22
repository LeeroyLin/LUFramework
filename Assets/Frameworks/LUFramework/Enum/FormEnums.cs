/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 窗体的枚举
 */
 
namespace LUFramework
{
    /// <summary>
    /// 窗体类型枚举
    /// </summary>
    public enum EFormType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal,

        /// <summary>
        /// 固定
        /// </summary>
        Fixed,

        /// <summary>
        /// 弹窗
        /// </summary>
        PopUp
    }

    /// <summary>
    /// 窗体显示类型枚举
    /// </summary>
    public enum EFormDisplayType
    {
        /// <summary>
        /// 单独显示 其他销毁
        /// </summary>
        Single,

        /// <summary>
        /// 栈 叠加在其他之上
        /// 不隐藏其他
        /// </summary>
        StackAdditive,

        /// <summary>
        /// 栈 隐藏其他
        /// </summary>
        StackSingle
    }

    /// <summary>
    /// 窗体模态枚举
    /// </summary>
    public enum EFormModalType
    {
        /// <summary>
        /// 没有模态
        /// </summary>
        None,

        /// <summary>
        /// 全透明
        /// </summary>
        Lucency,

        /// <summary>
        /// 半透明
        /// </summary>
        Translucency,

        /// <summary>
        /// 不透明
        /// </summary>
        Opacity
    }
}