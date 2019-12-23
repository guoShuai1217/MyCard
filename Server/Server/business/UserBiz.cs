using GameProtocol.model.login;
using NetFrame;
using Server.dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.business
{
    public class UserBiz
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserModel GetModel(UserToken token) {
            //判断连接是否为空
            if (token == null) return null;
            RoleInfo info = cache.CacheFactory.user.Get(token);
            if (info == null)
                return null;
            UserModel model = new UserModel();
            model.id = info.id;
            model.head = info.head;
            model.username = info.username;
            model.nickname = info.nickname;
            model.phone = info.phone;
            model.sex = info.sex;
            model.coin = info.coin;
            model.cash = info.cash;
            return model;
        }
    }
}
