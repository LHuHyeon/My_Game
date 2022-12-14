using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// json을 코드로 변환할 곳
namespace Data
{
    #region Stat

    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int maxSp;
        public int attack;
        public int luk;
        public int statPoint;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            // List를 Dictionary로 변환
            foreach(Stat stat in stats){
                dict.Add(stat.level, stat);
            }

            return dict;
        }
    }

    #endregion
}
