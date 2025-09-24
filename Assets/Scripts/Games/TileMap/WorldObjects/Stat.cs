using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class Stat
    {
        // 육체적 스탯
        // ㄴ 힘
        // ㄴ 민첩
        // ㄴ 지능
        // ㄴ 속도
        // ㄴ 에너지 스탯
        // ㄴ 체력
        // ㄴ 면역력
        public int Strength; // 힘
        public int Agility; // 민첩
        public int Intelligence; // 지능
        public int Speed; // 속도
        public int Energy; // 에너지 스탯
        public int Stamina; // 체력
        public int Immunity; // 면역력
        public int Weight; // 몸무게

        // 정신적 스탯
        // ㄴ 우울감
        // ㄴ 스트레스
        // ㄴ 불안감
        // ㄴ 집중력
        // ㄴ 행복감
        // ㄴ 피로도
        // ㄴ 창의성
        // ㄴ 적계심
        // ㄴ 동기부여
        // ㄴ 기억력
        public int Depression; // 우울감
        public int Stress; // 스트레스
        public int Anxiety; // 불안감
        public int Concentration; // 집중력
        public int Happiness; // 행복감
        public int Fatigue; // 피로도
        public int Creativity; // 창의성
        public int Hostility; // 적계심
        public int Motivation; // 동기부여
        public int Memory; // 기억력

        // 사회적 스탯
        // ㄴ 외로움
        // ㄴ 소속감
        // ㄴ 자존감
        // ㄴ 사랑받음
        // ㄴ 인정받음
        public int Loneliness; // 외로움
        public int Belonging; // 소속감
        public int SelfEsteem; // 자존감
        public int BeingLoved; // 사랑받음
        public int BeingRecognized; // 인정받음

        // 환경적 스탯
        // ㄴ 안전함
        // ㄴ 편안함
        // ㄴ 청결함
        // ㄴ 자연스러움
        // ㄴ 자원 풍부함
        public int Safety; // 안전함
        public int Comfort; // 편안함
        public int Cleanliness; // 청결함
        public int Naturalness; // 자연스러움
        public int ResourceAbundance; // 자원 풍부함

        public enum StatType
        {
            // 육체적 스탯
            Strength = 0, // 힘
            Agility, // 민첩
            Intelligence, // 지능
            Speed, // 속도
            Energy, // 에너지 스탯
            Stamina, // 체력
            Immunity, // 면역력
            Weight, // 몸무게

            // 정신적 스탯
            Depression, // 우울감
            Stress, // 스트레스
            Anxiety, // 불안감
            Concentration, // 집중력
            Happiness, // 행복감
            Fatigue, // 피로도
            Creativity, // 창의성
            Hostility, // 적계심
            Motivation, // 동기부여
            Memory, // 기억력

            // 사회적 스탯
            Loneliness, // 외로움
            Belonging, // 소속감
            SelfEsteem, // 자존감
            BeingLoved, // 사랑받음
            BeingRecognized, // 인정받음

            // 환경적 스탯
            Safety, // 안전함
            Comfort, // 편안함
            Cleanliness, // 청결함
            Naturalness, // 자연스러움
            ResourceAbundance, // 자원 풍부함
        }

        private Dictionary<StatType, int> Info = new();
        
        public long Health;

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