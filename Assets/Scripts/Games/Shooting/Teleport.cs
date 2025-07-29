using UnityEngine;

namespace Giant.Shooting
{
    public class Telepoprt : MonoBehaviour
    {
        public int TargetID;

        // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌을 시작했을 때
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("충돌 시작!");
        }

        // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌중일 때
        private void OnCollisionStay(Collision collision)
        {
            Debug.Log("충돌 중!");
        }

        // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌이 끝났을 때
        private void OnCollisionExit(Collision collision)
        {
            Debug.Log("충돌 끝!");
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("트리거 시작!");
        }

        private void OnTriggerStay(Collider other)
        {
            Debug.Log("트리거 중!");
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("트리거 끝!");
        }
    }
}
