using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol.model.fight;

namespace Server.tool.poker
{
    /// <summary>
    /// 赢三张扑克工具类
    /// </summary>
    public class TPokerUtil
    {
        /// <summary>
        /// 置随机数种子
        /// </summary>
        Random ran = new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// 获取一副新的扑克牌
        /// </summary>
        /// <returns></returns>
        public List<PokerModel> GetPokerList()
        {
            //声明一副新扑克
            List<PokerModel> newPoker = new List<PokerModel>();
            //依次将扑克装入列表中
            for(int i = 1;i < 14;i++)//i=牌值
            {
                for (int j = 1; j < 5; j++)//j=颜色值
                    newPoker.Add(new PokerModel(i, j));
            }
            //洗牌
            List<PokerModel> retPoker = new List<PokerModel>();
            while (newPoker.Count > 0)
            {
                //随机查找一个下标
                int idx = ran.Next(0, newPoker.Count - 1);
                retPoker.Add(newPoker[idx]);
                newPoker.RemoveAt(idx);
            }
            return retPoker;
        }
        /// <summary>
        /// 获取扑克类型
        /// </summary>
        /// <param name="poker">扑克列表</param>
        /// <returns>0 散牌</returns>
        /// <returns>1 对子</returns>
        /// <returns>2 顺子</returns>
        /// <returns>3 同花</returns>
        /// <returns>4 同花顺</returns>
        /// <returns>5 豹子</returns>
        public int GetType(List<PokerModel> poker)
        {
            if (poker.Count != 3) return 0;
            //扑克值
            List<int> Value = new List<int>();
            //扑克色
            List<int> Color = new List<int>();
            for (int i = 0; i < poker.Count; i++)
            {
                if(poker[i].Value == 1)
                    Value.Add(poker[i].Value + 13);
                else
                    Value.Add(poker[i].Value);
                Color.Add(poker[i].Color);
            }
            Value.Sort();
            Color.Sort();
            //豹子
            if (Value[0] == Value[1] && Value[1] == Value[2])
            {
                return 5;
            }
            //是否是顺子
            bool isStraight = false;
            //是否为同花
            bool isColorOnce = false;
            //顺子
            if (Value[0] == Value[1] - 1 && Value[1] - 1 == Value[2])
            {
                isStraight = true;
            }
            //同花
            if (Color[0] == Color[1] && Color[1] == Color[2])
            {
                isColorOnce = true;
            }
            //同花顺
            if (isColorOnce && isStraight)
            {
                return 4;
            }
            if (isColorOnce) return 3;
            if (isStraight) return 2;
            //对子
            if (Value[0] == Value[1] || Value[0] == Value[2] || Value[1] == Value[2])
                return 1;
            //单牌
            return 0;
        }
        /// <summary>
        /// 扑克比牌
        /// </summary>
        /// <param name="poker1"></param>
        /// <param name="poker2"></param>
        /// <returns>true 胜利  false 失败</returns>
        public bool GetComparePoker(List<PokerModel> poker1, List<PokerModel> poker2) {
            //获取比牌类型
            int getType1 = GetType(poker1);
            //获取被比牌类型
            int getType2 = GetType(poker2);
            //如果被比类型大于比牌类型，则直接失败
            if (getType2 > getType1)
                return false;
            if (getType1 > getType2)
                return true;
            //即将拿去比牌的值
            List<int> Value1 = new List<int>();
            //即将拿去被比牌的值
            List<int> Value2 = new List<int>();
            for (int i = 0; i < poker1.Count; i++)
            {
                if (poker1[i].Value == 1)
                    Value1.Add(poker1[i].Value + 13);
                else
                    Value1.Add(poker1[i].Value);
                if (poker2[i].Value == 1)
                    Value2.Add(poker2[i].Value + 13);
                else
                    Value2.Add(poker2[i].Value);
            }
            Value1.Sort();
            Value2.Sort();
            //如果是对子，则先比较对子，在比较单牌
            if (getType1 == 1)
            {
                //获取对牌 AA2
                int two1 = Value1[0] == Value1[1] ? Value1[0] : Value1[1];
                int two2 = Value2[0] == Value2[1] ? Value2[0] : Value2[1];
                if (two1 > two2)
                    return true;
                if (two2 > two1)
                    return false;
                //获取单牌 AA2
                int one1 = Value1[0] == Value1[1] ? Value1[2] : Value1[0];
                int one2 = Value2[0] == Value2[1] ? Value2[2] : Value2[0];
                if (one1 > one2)
                    return true;
                //不管是小于还是等于，先比先输
                return false;
            }
            else {
                //如果比牌的最大牌大于被比牌的最大牌，直接胜利
                if (Value1[2] > Value2[2])
                    return true;
                if (Value1[2] == Value2[2])
                {
                    if (Value1[1] > Value2[1])
                        return true;
                    if (Value1[1] == Value2[1])
                        if (Value1[0] > Value2[0])
                            return true;
                }
                //不管小于还是等于，先比先输
                return false;
            }
        }
    }
}
