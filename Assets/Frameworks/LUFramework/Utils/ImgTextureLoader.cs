/*
 * 时间 : 2019/7/10
 * 作者 : LeeroyLin
 * 项目 : LU框架
 * 描述 : 图片贴图加载类
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LUFramework
{
    /// <summary>
    /// 图片贴图加载类
    /// </summary>
	public class ImgTextureLoader : SingletonScript<ImgTextureLoader> 
	{
        #region 公共字段
        #endregion

        #region 私有字段
        /// <summary>
        /// 存储已经加载过的精灵图片
        /// </summary>
        private Dictionary<string, Sprite> _spritesDic = new Dictionary<string, Sprite>();
		#endregion
		
		#region 属性
		#endregion
		
		#region 默认回调
		/// <summary>
		/// 开始后调用
		/// </summary>
		void Start () 
		{

		}
		
		/// <summary>
		/// 每帧调用
		/// </summary>
		void Update () 
		{
		
		}
        #endregion

        #region 加载纹理到图片
        /// <summary>
        /// 加载纹理到图片对象
        /// </summary>
        /// <param name="imgArray">图片对象数组</param>
        /// <param name="urlArray">纹理url数组</param>
        public void LoadTexture2Image(Image[] imgArray, string[] urlArray)
        {
            // 开启协程加载
            StartCoroutine(LoadTextures2ImagesCoroutine(imgArray, urlArray));
        }

        /// <summary>
        /// 加载纹理到图片对象
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="url">纹理url</param>
        public void LoadTexture2Image(Image img, string url)
        {
            // 开启协程加载
            StartCoroutine(LoadTextures2ImagesCoroutine(new Image[] { img }, new string[] { url }));
        }

        /// <summary>
        /// 加载纹理到图片协程 数组
        /// </summary>
        /// <param name="imgArray"></param>
        /// <param name="urlArray"></param>
        /// <returns></returns>
        private IEnumerator LoadTextures2ImagesCoroutine(Image[] imgArray, string[] urlArray)
        {
            // 判断大循环次数
            int count = Mathf.CeilToInt(imgArray.Length / (float)Config.loadTextureNumPerTime);

            // 下标
            int index = 0;

            Coroutine coroutine = default(Coroutine);
            Sprite sprite;

            // 循环执行
            for (int i = 0; i < count; i++)
            {
                coroutine = null;

                for (int t = 0; t < Config.loadTextureNumPerTime; t++)
                {
                    // 获得下标
                    index = i * Config.loadTextureNumPerTime + t;

                    // 是否下标合法
                    if (index < imgArray.Length)
                    {
                        // 是否字典里有
                        if (_spritesDic.ContainsKey(urlArray[index]))
                        {
                            // 获得字典里的精灵图片
                            sprite = _spritesDic[urlArray[index]];

                            // 直接设置
                            imgArray[index].sprite = sprite;
                        }
                        else
                        {
                            // 通过协程加载
                            coroutine = StartCoroutine(LoadTexture2ImageCoroutine(imgArray[index], urlArray[index]));
                        }
                    }
                }
                
                if(coroutine != default(Coroutine))
                    yield return coroutine;
            }
        }

        /// <summary>
        /// 加载纹理到图片对象 协程
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="url">纹理url</param>
        private IEnumerator LoadTexture2ImageCoroutine(Image img, string url)
        {
            // 加载
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();
                
            // 是否没有错误
            if (!req.isNetworkError && !req.isHttpError)
            {
                // 获得纹理
                Texture2D texture = ((DownloadHandlerTexture)req.downloadHandler).texture;

                // 设置纹理到图片对象
                img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 1));

                // 是否字典里没有
                if (!_spritesDic.ContainsKey(url))
                {
                    // 记录纹理
                    _spritesDic.Add(url, img.sprite);
                }
            }
            else
            {
                Debug.LogError(string.Format("[LoadTexture2ImageCoroutine] 从{0}加载图片失败 ：{1}", url, req.error));
            }
        }
        #endregion
    }
}