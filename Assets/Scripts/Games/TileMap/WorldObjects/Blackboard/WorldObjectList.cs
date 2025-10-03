using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    public class WorldObjectList : List<WorldObject> {
        public List<GameObject> List()
        {
            var list = new List<GameObject>(this.Count);
            foreach (var obj in this)
            {
                if (obj != null)
                    list.Add(obj.gameObject);
            }

            return list;
        }
    }
}