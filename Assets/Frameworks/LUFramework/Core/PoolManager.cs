/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 对象池管理类
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 对象池管理类
    /// </summary>
	public class PoolManager : SingletonScript<PoolManager> 
	{
        #region 公共字段
        #endregion

        #region 私有字段
        /// <summary>
        /// 对象池节点
        /// </summary>
        Transform _poolNode;
        
        /// <summary>
        /// 对象池字典
        /// </summary>
        Dictionary<string, PoolItem> _poolDic;
        #endregion

        #region 默认回调
        /// <summary>
        /// 唤醒后调用
        /// </summary>
        void Awake()
        {
            // 初始化
            Init();
        }
		#endregion
		
		#region 私有方法
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 初始化
            _poolDic = new Dictionary<string, PoolItem>();

            // 生成一个新的物体作为对象池节点
            _poolNode = new GameObject("Pool").transform;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">物体变换对象</param>
        /// <param name="capacity">最大容量</param>
        public void Recover(string tag, GameObject obj, int capacity = 0)
        {
            PoolItem item;
            // 是否有该标签的对象池
            if (_poolDic.TryGetValue(tag, out item))
            {
                // 是否满了
                if (item.IsFull())
                {
                    // 直接销毁
                    Destroy(obj);

                    return;
                }
            }
            else
            {
                // 新建游戏物体
                GameObject newObj = new GameObject(tag);

                // 设置父节点
                newObj.transform.SetParent(_poolNode);

                // 新对象池对象
                item = new PoolItem();

                // 设置容量
                item.SetCapacity(capacity);

                // 添加新的对象池对象
                _poolDic.Add(tag, item);
            }

            // 隐藏显示
            obj.SetActive(false);

            // 添加到对应节点下
            obj.transform.SetParent(_poolNode.Find(tag));

            // 添加到列表中
            item.AddItem(obj);
        }

        /// <summary>
        /// 尝试获得物体
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="createFun">创造函数，如果对象池没有则通过该方法创造</param>
        /// <param name="initFun">初始化函数，对物体进行处理</param>
        public GameObject TryGet(string tag, Func<GameObject> createFun, Func<GameObject, GameObject> initFun = null)
        {
            GameObject obj = null;
            PoolItem item;
            // 是否有该标签的对象池 且 列表中还有物品
            if (_poolDic.TryGetValue(tag, out item) && item.ItemList.Count > 0)
            {
                // 获得对应物体
                obj = item.ItemList[item.ItemList.Count - 1];

                // 从列表移除
                item.ItemList.RemoveAt(item.ItemList.Count - 1);

            }
            else
            {
                // 返回新建的
                obj = createFun();
            }

            // 初始化操作
            if (initFun != null)
            {
                obj = initFun(obj);
            }

            // 显示
            obj.SetActive(true);

            // 返回该对象
            return obj;
        }
        #endregion
    }
}