/*
 * 时间 : 2019/10/29
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
        public readonly static IReadOnlyList<ExampleTableItem> data = new List<ExampleTableItem>() {
			// 0
			new ExampleTableItem(
				1,
				90f,
				"Starfish",
				new int[] {
					1,
					2
				},
				new float[] {
					0.6f,
					0.4f
				},
				new string[] {
					"'icon_starfish_white'",
					"'icon_starfish_green'"
				}
			),
			// 1
			new ExampleTableItem(
				2,
				100f,
				"Sponge",
				new int[] {
					2,
					3,
					4
				},
				new float[] {
					0.3f,
					0.5f,
					0.2f
				},
				new string[] {
					"'icon_sponge_green'",
					"'icon_sponge_blue'",
					"'icon_sponge_purple'"
				}
			)
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
		public readonly int Id;
		/// <summary>
		/// ItemPrice
		/// </summary>
		public readonly float Price;
		/// <summary>
		/// ItemName
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// QualityList
		/// </summary>
		public readonly IReadOnlyList<int> QualityList;
		/// <summary>
		/// ProbabilityList
		/// </summary>
		public readonly IReadOnlyList<float> ProbabilityList;
		/// <summary>
		/// IconList
		/// </summary>
		public readonly IReadOnlyList<string> ListFriends;

		public ExampleTableItem(int Id, float Price, string Name, int[] QualityList, float[] ProbabilityList, string[] ListFriends)
		{
			this.Id = Id;
			this.Price = Price;
			this.Name = Name;
			this.QualityList = QualityList;
			this.ProbabilityList = ProbabilityList;
			this.ListFriends = ListFriends;
		}

    }
}