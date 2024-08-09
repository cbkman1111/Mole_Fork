using DG.Tweening;
using System;

using System.Collections.Generic;
using System.Linq;
using Common.Global;
using UI.Menu;
using UI.Popup;
using UnityEngine;
using static UnityEditor.Rendering.InspectorCurveEditor;
using UnityEditor;


namespace Gostop
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class BoardPosition
    {
        public Transform Gwang = null;
        public Transform Mung = null;
        public Transform Thee = null;
        public Transform Pee = null;

        public Transform RecvieCard = null;
        public Transform Hand = null;
    }

    public class Score
    {
        public int shake; // 흔듬 숫자.
        public int go; // 고 숫자.
        public bool peebak; // 피박.
        public bool gwangbak; // 광박.
        public bool mungbak; // 멍박.
        public bool goback; // 고박.

        public bool chungdan;
        public bool hongdan;
        public bool chodan;
        public bool godori;

        public int gawng;
        public int mung;
        public int pee;
        public int thee;

        public int total;

        public void Init()
        {
            shake = 0; 
            go = 0; 
            peebak = false; 
            gwangbak = false; 
            mungbak = false; 
            goback = false;
            chungdan = false;
            hongdan = false;
            chodan = false;
            godori = false;
            gawng = 0;
            mung = 0;
            pee = 0;
            thee = 0;
            total = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Board : MonoBehaviour
    {
        public Score[] gameScore = null;
        public Action<Player, Score> updateScore = null;
    }
}
