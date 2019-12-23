using NetFrame;
using Server.dao;
using ServerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.cache
{
    /// <summary>
    /// 用户数据缓存
    /// </summary>
    public class UserCache
    {
        /// <summary>
        /// 用户账号与用户数据的映射
        /// </summary>
        Dictionary<string, dao.RoleInfo> AccountMap = new Dictionary<string, dao.RoleInfo>();
        /// <summary>
        /// 在线连接和用户账号的映射
        /// </summary>
        Dictionary<UserToken, string> OnlineAccount = new Dictionary<UserToken, string>();
        /// <summary>
        /// 玩家ID与用户连接的映射
        /// </summary>
        Dictionary<int, UserToken> IdToToken = new Dictionary<int, UserToken>();

        int index = 0;
        /// <summary>
        /// 注册账号
        /// </summary>
        public string RegiserAccount(UserToken token) {
            //创建一个新的角色账号
            dao.RoleInfo role = new dao.RoleInfo();
            index++;
            role.id = index;
            role.username = "lin" + (index + 10000);
            role.password = "password";
            role.nickname = "游" + (index + 10000);
            //账号：lin10001
            //密码：password
            //昵称：游10001
            //头像：default
            //金币：10000
            //钻石：10000
            //性别：0-男
            //手机号：“"
            DebugUtil.Instance.LogToTime("快速注册成功:账号=" + role.username + " 昵称=" + role.nickname);
            AccountMap.Add(role.username, role);
            CacheFactory.user.Online(token, role.username);
            return role.password;
        }

        /// <summary>
        /// 读取账号
        /// </summary>
        public void LoadAccount()
        {

        }

        /// <summary>
        /// 是否含有此账号
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IshasAccount(string username)
        {
            //如果账号列表包含此账号
            if(AccountMap.ContainsKey(username))
                return true;
            return false;
        }

        /// <summary>
        /// 账号密码是否匹配
        /// </summary>
        /// <returns></returns>
        public bool IsPassword(string username,string password)
        {
            if (!IshasAccount(username)) return false;
            if (AccountMap[username].password.Equals(password))
                return true;
            return false;
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsOnline(UserToken token) {
            if (OnlineAccount.ContainsKey(token))
                return true;
            return false;
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsOnline(int id)
        {
            if (IdToToken.ContainsKey(id) && OnlineAccount.ContainsKey(IdToToken[id]))
                return true;
            return false;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void Online(UserToken token, string username)
        {
            if (IsOnline(token))
            {
                DebugUtil.Instance.LogToTime(username + "已在线", LogType.WARRING);
                return;
            }
            if (!IshasAccount(username))
            {
                DebugUtil.Instance.LogToTime(username + "不存在", LogType.WARRING);
                return;
            }
            if (IdToToken.ContainsKey(AccountMap[username].id))
            {
                DebugUtil.Instance.LogToTime(username + "移除账号连接", LogType.WARRING); 
                IdToToken.Remove(AccountMap[username].id);
            }
            DebugUtil.Instance.LogToTime(username + "上线成功", LogType.WARRING);
            IdToToken.Add(AccountMap[username].id, token);
            OnlineAccount.Add(token, username);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void offline(UserToken token)
        {
            //如果在线并且含有此账号
            if (IsOnline(token) && IshasAccount(OnlineAccount[token]))
            {
                int id = AccountMap[OnlineAccount[token]].id;
                if (IdToToken.ContainsKey(id))
                    IdToToken.Remove(id);
                OnlineAccount.Remove(token);
                DebugUtil.Instance.LogToTime(id + "玩家下线了");
            }
        }

        /// <summary>
        /// 保存账户信息
        /// </summary>
        public void Save(UserToken token) {

        }

        public RoleInfo Get(UserToken token)
        {
            //判断是否在线
            if (!IsOnline(token)) return null;
            //判断是否含有账号
            if (!IshasAccount(OnlineAccount[token])) return null;
            return AccountMap[OnlineAccount[token]];
        }

        public RoleInfo Get(int id)
        {
            //判断是否在线
            if (!IsOnline(id)) return null;
            //判断是否含有账号
            if (!IshasAccount(OnlineAccount[IdToToken[id]])) return null;
            return AccountMap[OnlineAccount[IdToToken[id]]];
        }

        /// <summary>
        /// 通过连接获取用户ID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int GetIdToToken(UserToken token)
        {
            //如果当前没有在线，或没有此账号 
            if (!IsOnline(token) || !IshasAccount(OnlineAccount[token])) return -1;
            return AccountMap[OnlineAccount[token]].id;
        }

        /// <summary>
        /// 通过用户ID获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserToken GetToken(int id)
        {
            //如果当前没有该ID的映射
            if (!IdToToken.ContainsKey(id)) return null;
            return IdToToken[id];
        }
    }
}
