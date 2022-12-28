using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ant
{
    public class Monster : MonoBehaviour
    {
        private Rigidbody2D rigidBody = null;
        public SpriteRenderer hand = null;
        public float speed = 0.01f;

        public ObjectData Data { get; set; }

        public bool Init(ObjectData data)
        {
            this.Data = data;

            rigidBody = GetComponent<Rigidbody2D>();
            transform.position = data.position;

            return true;
        }

        public void Move(Vector3 direction)
        {
            if(rigidBody != null)
            {
                var position = transform.position + direction * speed;
                rigidBody.MovePosition(position);
                hand.transform.position = transform.position + (direction * 0.6f);
            }
        }

        public Vector3 GetHandPosition()
        {
            return hand.transform.position;
        }

        public void Save()
        {
            Data.position = transform.position;

            Data.Save();
        }
    }

}
