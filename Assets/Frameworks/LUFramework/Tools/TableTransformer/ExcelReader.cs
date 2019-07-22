/*
 * 时间 : 2019/7/16
 * 作者 : LeeroyLin
 * 项目 : 表转换器
 * 描述 : Excel读取器
 */

using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

namespace LUFramework
{
    /// <summary>
    /// Excel读取器
    /// </summary>
	public class ExcelReader 
	{
        #region 公共字段
        #endregion

        #region 私有字段
        #endregion

        #region 其他方法
        /// <summary>
        /// 读取Excel
        /// </summary>
        /// <param name="path">Excel文件路径</param>
        /// <returns>数据集</returns>
        public static DataTable ReadExcel(string path)
        {
            // 读取文件
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            excelReader.Close();

            // 返回第一个表信息
            return result.Tables[0];

        }


        #endregion
    }
}