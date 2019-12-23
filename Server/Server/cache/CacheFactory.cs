using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.cache
{
    /// <summary>
    /// 缓存代理
    /// </summary>
    public class CacheFactory
    {
        /// <summary>
        /// 建立一个用户数据缓存
        /// </summary>
        public readonly static UserCache user;
        /// <summary>
        /// 建立一个匹配队列缓存
        /// </summary>
        public readonly static MatchCache match;

        public readonly static FightCache fight;
        static CacheFactory()
        {
            user = new UserCache();
            match = new MatchCache();
            fight = new FightCache();
        }
    }
}
