using NPOI.Util.ArrayExtensions;
using System;
using UnityEngine;

namespace Giant.Shooting
{
    public class Map : MonoBehaviour
    {
        [SerializeField] public Teleport[] Teleports = null;
        [SerializeField] public GameObject[] CameraArea = null;

        public event Action<int> OnTeleport = null;

        public bool Init()
        {
            OnTeleport = null;

            foreach (var t in Teleports)
            {
                t.OnEnter = OnTeleportMap;
            }
            
            return true;
        }

        private void OnTeleportMap(int id)
        {
            if (OnTeleport != null)
            {
                OnTeleport(id);
            }
        }
    }
}

