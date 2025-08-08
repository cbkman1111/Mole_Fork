using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class Stat
    {
        public enum StatType
        {
            Health, // 생명력
            Energy, // 에너지
            Strength, // 힘
            Dexterity, // 민첩성
            Intelligence, // 지능
            Speed, // 속도

            // - 재미로..
            Weight // 무게
        }

        private Dictionary<StatType, int> Info = new();

        public int GetStat(StatType type)
        {
            if (Info.TryGetValue(type, out int value))
            {
                return value;
            }
            else
            {
                Debug.LogWarning($"Stat type {type} not found.");
                return 1; // 기본값 반환
            }
        }

        public void SetStat(StatType type, int value)
        {
            if (Info.ContainsKey(type))
            {
                Info[type] = value;
            }
            else
            {
                Info.Add(type, value);
            }
        }
    }
}