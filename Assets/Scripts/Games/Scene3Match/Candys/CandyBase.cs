using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    /// <summary>
     /// �⺻ Ÿ��.
     /// </summary>
    public class CandyBase : MonoBehaviour
    {
        protected int X = 0;
        protected int Y = 0;

        public virtual bool Init(int x, int y)
        {
            X = x;
            Y = y;

            return true;
        }
    }
}
