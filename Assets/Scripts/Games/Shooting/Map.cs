using NPOI.Util.ArrayExtensions;
using System;
using UnityEngine;

namespace Giant.Shooting
{
    public class Map : MonoBehaviour
    {
        [SerializeField] Teleport[] Teleports = null;
        public Action<int> OnTeleport = null;

        public bool Init()
        {
            foreach(var t in Teleports)
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

