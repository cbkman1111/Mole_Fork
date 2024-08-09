using System;
using UnityEngine;

namespace Gostop
{
    [Serializable]
    public class BoardSetting
    {
        public float DeckCardTime = 0.1f;
        public float HitCardTime = 0.1f;
        public float TakeCardTime = 0.1f;
        public float SuffleCardTime = 0.1f;
        public float SuffleCardInterval = 0.1f;
        public float FlipTime = 0.1f;
        public float HandUpTime = 0.1f;
        public float HandUpDelay = 0.025f;
        public float HandOpenTime = 0.1f;

        public float HitUpTime = 0.2f;
        public float HitDownTime = 0.1f;
    }

    public class BoardSettingContainer : ScriptableObject
    {
        public BoardSetting setting;

        public void SetData(BoardSetting data)
        {
            this.setting = data.DeepClone<BoardSetting>();
        }
    }
}
