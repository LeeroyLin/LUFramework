/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : UI控制类
 */


using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// UI控制类
    /// </summary>
	public class UIManager : SingletonScript<UIManager>
    {
        #region 公共字段
        /// <summary>
        /// UI根节点
        /// </summary>
        public Transform canvas;
        #endregion

        #region 私有字段
        /// <summary>
        /// 存储三个节点的数组
        /// </summary>
        private Transform[] _nodeArray;

        /// <summary>
        /// 栈数组
        /// </summary>
        private Stack<BaseForm>[] _stackArray;

        /// <summary>
        /// 窗体缓存字典
        /// 存储所有还存在的窗体
        /// </summary>
        private Dictionary<string, BaseForm> _cacheFormDic;

        /// <summary>
        /// 窗体显示字典
        /// 存储已显示的窗体
        /// </summary>
        private Dictionary<string, BaseForm> _shownFormDic;

        /// <summary>
        /// 模态字典
        /// 存储窗体对应的模态对象
        /// </summary>
        private Dictionary<BaseForm, GameObject> _modalDic;
        #endregion

        #region 默认回调
        /// <summary>
        /// 唤醒后调用
        /// </summary>
        void Awake()
        {
            // 初始化
            Init();

            // 获得节点
            GetUI();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            _nodeArray = new Transform[3];
            _stackArray = new Stack<BaseForm>[] {
                new Stack<BaseForm>(),
                new Stack<BaseForm>(),
                new Stack<BaseForm>()
            };
            _cacheFormDic = new Dictionary<string, BaseForm>();
            _shownFormDic = new Dictionary<string, BaseForm>();
            _modalDic = new Dictionary<BaseForm, GameObject>();
        }

        /// <summary>
        /// 获得UI节点
        /// </summary>
        private void GetUI()
        {
            // 获得Canvas
            if (GetCanvas())
            {
                // 获得三个节点
                _nodeArray[0] = canvas.Find("Normal");
                _nodeArray[1] = canvas.Find("Fixed");
                _nodeArray[2] = canvas.Find("Popup");
            }
        }

        /// <summary>
        /// 获得Canvas
        /// </summary>
        /// <returns>是否成功获得</returns>
        private bool GetCanvas()
        {
            // canvas名
            string canvasName = Path.GetFileName(Config.CANVAS_PATH);

            // 获得场景中的节点
            GameObject canvasObj = GameObject.Find(canvasName);

            // 是否节点为空
            if (canvasObj == null)
            { 
                try
                {
                    // 生成新的canvas
                    canvasObj = Instantiate(Resources.Load(Config.CANVAS_PATH)) as GameObject;

                }
                catch (System.Exception e)
                {
                    LogManager.Instance.Log("LU框架|UI", "从{0}生成canvas失败, 错误信息: {1}", ELogSeverity.Error, false, Config.CANVAS_PATH, e.Message);

                    return false;
                }

                // 设置名字
                canvasObj.name = canvasName;
            }

            // 设置根节点
            canvas = canvasObj.transform;

            // 将该脚本附加的物体作为canvas子节点
            transform.SetParent(canvas);

            // 设置不销毁
            DontDestroyOnLoad(canvas);

            return true;
        }

        /// <summary>
        /// 加载或从缓存获取窗体对象
        /// </summary>
        /// <param name="name">窗体名</param>
        /// <returns>窗体对象</returns>
        private BaseForm LoadFormOrFromCache(string name)
        {
            // 是否缓存字典有该窗体
            if (_cacheFormDic.TryGetValue(name, out BaseForm form))
            {
                // 显示
                form.Show();

                // 返回
                return form;

            }
            else
            {
                // 加载窗体
                return LoadForm(name);
            }
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="name">窗体名</param>
        /// <returns>窗体对象</returns>
        private BaseForm LoadForm(string name)
        {
            try
            {
                // 生成对象
                GameObject formObj = Instantiate(Resources.Load(Config.FORM_PATH + name), _nodeArray[0]) as GameObject;

                // 设置名字
                formObj.name = name;

                // 获得脚本
                BaseForm form = formObj.GetComponent<BaseForm>();

                // 如果没有该脚本
                if (form == null)
                {
                    LogManager.Instance.Log("LU框架|UI", "窗体{0}没有BaseForm脚本", ELogSeverity.Error, false, name);
                }
                else
                {
                    // 添加到缓存字典
                    _cacheFormDic.Add(name, form);
                }

                return form;

            }
            catch (System.Exception e)
            {
                LogManager.Instance.Log("LU框架|UI", "生成窗体{0}失败, 错误信息: {1}", ELogSeverity.Error, false, name, e.Message);

                return null;
            }
        }

        /// <summary>
        /// 尝试将窗体添加到已显示的字典
        /// </summary>
        /// <param name="name">窗体名</param>
        /// <param name="form">窗体对象</param>
        private void TryAddFormToShownDic(string name, BaseForm form)
        {
            // 如果字典中没有该项
            if (!_shownFormDic.ContainsKey(name))
            {
                // 添加
                _shownFormDic.Add(name, form);
            }
        }

        /// <summary>
        /// 设置窗体父节点
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="formType">窗体类型</param>
        private void SetFormParent(BaseForm form, EFormType formType)
        {
            // 设置父节点
            form.transform.SetParent(_nodeArray[(int)formType]);
        }

        /// <summary>
        /// 设置对应现实
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="formType">窗体类型</param>
        /// <param name="displayType">窗体显示类型</param>
        private void SetDisplay(BaseForm form, EFormType formType, EFormDisplayType displayType)
        {
            // 获得对应栈对象
            Stack<BaseForm> stack = _stackArray[(int)formType];

            // 临时窗体对象
            BaseForm tempForm;

            switch (displayType)
            {
                // 单独显示
                case EFormDisplayType.Single:
                {
                    // 将所有栈内的窗体弹出并销毁
                    for (int i = 0; i < stack.Count; i++)
                    {
                        tempForm = stack.Pop();
                        tempForm.Destroy();
                    }

                }
                break;
                // 栈显示 不隐藏其他
                case EFormDisplayType.StackAdditive:
                {
                    // 添加到栈中
                    stack.Push(form);

                }
                break;
                // 栈显示 隐藏其他
                case EFormDisplayType.StackSingle:
                {
                    // 将栈里原来所有的窗体隐藏
                    foreach (var f in stack)
                    {
                        f.Hide();
                    }

                    // 添加到栈中
                    stack.Push(form);
                }
                break;
            }
        }

        /// <summary>
        /// 设置模态
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="modalType">模态类型</param>
        private void SetModal(BaseForm form, EFormModalType modalType)
        {
            // 尝试从模态字典里获得该窗体的模态
            _modalDic.TryGetValue(form, out GameObject modal);

            // 是否不要模态
            if (modalType == EFormModalType.None)
            {
                // 是否字典中有模态
                if (modal != null)
                {
                    // 回收该模态
                    PoolManager.Instance.Recover(Config.TAG_MODAL, modal);

                    // 从字典中清除
                    _modalDic.Remove(form);
                }

                return;
            }
            else
            {
                // 是否字典没有模态
                if (modal == null)
                {
                    // 从对象池中获得模态
                    modal = PoolManager.Instance.TryGet(Config.TAG_MODAL, CreateModal);

                    // 判空
                    if (modal == null)
                    {
                        return;
                    }
                }
            }

            // 设置父节点
            modal.transform.SetParent(form.transform.parent);

            // 放到窗体之下
            modal.transform.SetSiblingIndex(form.transform.GetSiblingIndex());

            // 根据不同的模态类型设置颜色
            Color modalColor = Color.white;
            switch (modalType)
            {
                // 全透明
                case EFormModalType.Lucency:
                {
                    modalColor = new Color(0, 0, 0, 0);
                }
                break;
                // 半透明
                case EFormModalType.Translucency:
                {
                    modalColor = new Color(0, 0, 0, 0.5f);
                }
                break;
                // 不透明
                case EFormModalType.Opacity:
                {
                    modalColor = new Color(0, 0, 0, 1);
                }
                break;
            }
        }

        /// <summary>
        /// 创建模态窗体
        /// </summary>
        /// <returns>模态窗体</returns>
        private GameObject CreateModal()
        {
            try
            {
                // 生成
                GameObject modalObj = Instantiate(Resources.Load(Config.MODAL_PATH)) as GameObject;

                // 修改名字
                modalObj.name = Path.GetFileName(Config.MODAL_PATH);

                return modalObj;

            }
            catch (System.Exception e)
            {
                LogManager.Instance.Log("LU框架|UI", "创建模态窗体失败, 错误信息: {0}", ELogSeverity.Error, false, e.Message);

                return null;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="name">窗体名</param>
        /// <param name="formType">窗体类型</param>
        /// <param name="displayType">显示类型</param>
        /// <param name="modalType">模态类型</param>
        /// <returns>是否成功显示</returns>
        public bool ShowForm(string name, EFormType formType, EFormDisplayType displayType, EFormModalType modalType = EFormModalType.None)
        {
            // 加载或从缓存获取窗体对象
            BaseForm form = LoadFormOrFromCache(name);

            // 如果窗体对象为空
            if (form == null)
            {
                return false;
            }

            // 添加到显示的窗体字典
            TryAddFormToShownDic(name, form);

            // 设置窗体父节点
            SetFormParent(form, formType);

            // 显示处理
            SetDisplay(form, formType, displayType);

            // 模态处理
            SetModal(form, modalType);

            return true;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="name">窗体名</param>
        /// <param name="isDestroyHiddenForms">是否销毁其他隐藏的窗体</param>
        /// <returns>是否成功关闭</returns>
        public bool CloseForm(string name, bool isDestroyHiddenForms)
        {
            return true;
        }
		#endregion
	}
}