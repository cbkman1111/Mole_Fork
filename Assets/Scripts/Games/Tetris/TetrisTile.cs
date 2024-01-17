using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tetris
{
    public class TetrisTile : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer spriteRenderer = null;
        public Vector2 Coordnate { get; set; } = Vector2.zero;
        public int Value { get; private set; }

        public bool Init(int x, int y, int value)
        {
            Value = value;
            Coordnate = new Vector2(x, y);

            if (Value == -1)
            {
                SetColor(Color.green);
            }
            /*
            else if (Value == 0)
            {
                SetColor(Color.white);
            }
            else
            {
                SetColor(Color.black);
            }
            */
            return true;
        }

        public void SetValue(int value)
        {
            this.Value = value;
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
    }
}
