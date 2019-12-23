using UnityEngine;
using System.Collections;

namespace Model
{
    /// <summary>
    /// 加载资源类
    /// </summary>
    public class LoadResourceModel
    {
        /// <summary>
        /// 资源的类型
        /// </summary>
        public GameResource.ResourceType type = GameResource.ResourceType.PREFAB;
        /// <summary>
        /// 待加载的资源的路径
        /// </summary>
        public string path = "";

        public LoadResourceModel() { }
        public LoadResourceModel(GameResource.ResourceType type, string path)
        {
            this.type = type;
            this.path = path;
        }
    }
}

