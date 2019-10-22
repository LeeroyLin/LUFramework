/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LUFramwork
 * 描述 : Config
 */

using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 配置类
    /// </summary>
	public class Config
	{
        #region Init
        /// <summary>
        /// Namespace
        /// "Auto" means to use the project name.
        /// You can change it to what you want.
        /// </summary>
        public static readonly string NAME_SPACE = "Auto";

        /// <summary>
        /// ProjectName
        /// </summary>
        public static readonly string PROJ_NAME = "ProjectName";

        /// <summary>
        /// Author
        /// </summary>
        public static readonly string DEVELOPER = "Author";

        #endregion

        #region Template
        /// <summary>
        /// 模版路径
        /// </summary>
        public static readonly string TEMPLATE_PATH = Application.dataPath + "/Frameworks/LUFramework/Template/";

        /// <summary>
        /// 脚本模板路径
        /// </summary>
        public static readonly string SCRIPT_TEMPLATE_PATH = TEMPLATE_PATH + "ScriptTemplate.txt";

        /// <summary>
        /// 类模板路径
        /// </summary>
        public static readonly string CLASS_TEMPLATE_PATH = TEMPLATE_PATH + "ClassTemplate.txt";

        /// <summary>
        /// 枚举模板路径
        /// </summary>
        public static readonly string ENUM_TEMPLATE_PATH = TEMPLATE_PATH + "EnumTemplate.txt";

        /// <summary>
        /// 接口模板路径
        /// </summary>
        public static readonly string INTERFACE_TEMPLATE_PATH = TEMPLATE_PATH + "InterfaceTemplate.txt";

        /// <summary>
        /// 视图模板路径
        /// </summary>
        public static readonly string VIEW_TEMPLATE_PATH = TEMPLATE_PATH + "ViewTemplate.txt";

        /// <summary>
        /// 模型模板路径
        /// </summary>
        public static readonly string MODEL_TEMPLATE_PATH = TEMPLATE_PATH + "ModelTemplate.txt";

        /// <summary>
        /// 控制器模板路径
        /// </summary>
        public static readonly string CONTROLLER_TEMPLATE_PATH = TEMPLATE_PATH + "ControllerTemplate.txt";

        /// <summary>
        /// 通知配置类模板路径
        /// </summary>
        public static readonly string NOTIFICATION_CONFIG_TEMPLATE_PATH = TEMPLATE_PATH + "NotificationConfigTemplate.txt";

        /// <summary>
        /// 转表模板路径
        /// </summary>
        public static readonly string TABLE_TEMPLATE_PATH = TEMPLATE_PATH + "TableTemplate.txt";
        #endregion

        #region MVC
        /// <summary>
        /// 视图脚本路径
        /// </summary>
        public static readonly string VIEW_SCRIPT_PATH = "Assets/Scripts/MVC/Views";
        /// <summary>
        /// 数据模型脚本路径
        /// </summary>
        public static readonly string MODEL_SCRIPT_PATH = "Assets/Scripts/MVC/Models";
        /// <summary>
        /// 控制脚本路径
        /// </summary>
        public static readonly string CONTROLLER_SCRIPT_PATH = "Assets/Scripts/MVC/Controllers";
        #endregion

        #region MessageCenter
        /// <summary>
        /// 消息中心配置路径
        /// </summary>
        public static readonly string NOTIFICATION_CONFIG_PATH = "Assets/Scripts/Configs";
        #endregion

        #region UI
        /// <summary>
        /// canvas路径
        /// Resources下的相对路径
        /// </summary>
        public static readonly string CANVAS_PATH = "LUFramework/LUFCanvas";

        /// <summary>
        /// 窗体路径
        /// Resources下的相对路径
        /// </summary>
        public static readonly string FORM_PATH = "LUFramework/Forms/";

        /// <summary>
        /// 模态路径
        /// Resources下的相对路径
        /// </summary>
        public static readonly string MODAL_PATH = "LUFramework/ModalForm";

        /// <summary>
        /// 模态半透明的透明度
        /// 0-1
        /// </summary>
        public static readonly float MODAL_ALPHA = 0.67f;
        #endregion

        #region Pool
        /// <summary>
        /// 默认对象池容量
        /// </summary>
        public const int DEFAULT_POOL_CAPACITY = 20;

        /// <summary>
        /// 标签 模态
        /// </summary>
        public const string TAG_MODAL = "Modal";
        #endregion

        #region Log
        /// <summary>
        /// 日志路径
        /// </summary>
        public static readonly string LOG_PATH = Application.streamingAssetsPath + "/Log/";

        /// <summary>
        /// 日志模版
        /// </summary>
        public static readonly string LOG_PATTERN = "[{0}] {1}\r\n";
        #endregion

        #region TextureLoader
        /// <summary>
        /// 同时加载的最大数量
        /// </summary>
        public static readonly int loadTextureNumPerTime = 10;
        #endregion
    }
}