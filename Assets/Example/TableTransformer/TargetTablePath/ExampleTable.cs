/*
 * 时间 : 2019/10/22
 * 作者 : Author
 * 项目 : ProjectName
 * 描述 : LUF表数据 - 转表自动生成
 */

using System.Collections.Generic;

namespace LUF
{
    /// <summary>
    /// ExampleTable表数据 - 转表自动生成
    /// </summary>
	public class ExampleTable
	{
        /// <summary>
        /// 数据
        /// </summary>
        public static List<ExampleTableItem> data = new List<ExampleTableItem>() {
			// 0
			new ExampleTableItem()
			{
				Id = 1,
				Price = 90f,
				Name = "Starfish",
				QualityList = new int[] {
					1,
					2
				},
				ProbabilityList = new float[] {
					0.6f,
					0.4f
				},
				ListFriends = new string[] {
					"'icon_starfish_white'",
					"'icon_starfish_green'"
				}
			},
			// 1
			new ExampleTableItem()
			{
				Id = 2,
				Price = 100f,
				Name = "Sponge",
				QualityList = new int[] {
					2,
					3,
					4
				},
				ProbabilityList = new float[] {
					0.3f,
					0.5f,
					0.2f
				},
				ListFriends = new string[] {
					"'icon_sponge_green'",
					"'icon_sponge_blue'",
					"'icon_sponge_purple'"
				}
			}
        };
	}

    /// <summary>
    /// ExampleTable表数据项
    /// </summary>
    public struct ExampleTableItem
    {
		/// <summary>
		/// ItemId
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// ItemPrice
		/// </summary>
		public float Price { get; set; }
		/// <summary>
		/// ItemName
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// QualityList
		/// </summary>
		public int[] QualityList { get; set; }
		/// <summary>
		/// ProbabilityList
		/// </summary>
		public float[] ProbabilityList { get; set; }
		/// <summary>
		/// IconList
		/// </summary>
		public string[] ListFriends { get; set; }
    }
}