/*
 * 时间 : 2019/7/17
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 对象池对象类
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// 对象池对象类
    /// </summary>
    public class PoolItem
    {
        /// <summary>
        /// 对象列表
        /// </summary>
        public List<GameObject> ItemList { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PoolItem()
        {
            // 初始化
            ItemList = new List<GameObject>();
            Capacity = Config.DEFAULT_POOL_CAPACITY;
        }

        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="item">物体对象</param>
        public void AddItem(GameObject item)
        {
            ItemList.Add(item);
        }

        /// <summary>
        /// 设置容量
        /// </summary>
        /// <param name="capacity">容量</param>
        public void SetCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                return;
            }

            Capacity = capacity;
        }

        /// <summary>
        /// 是否满了
        /// </summary>
        /// <returns>是否满了</returns>
        public bool IsFull()
        {
            return ItemList.Count >= Capacity;
        }
    }
}