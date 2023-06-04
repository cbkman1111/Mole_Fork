using System;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.TargetScripts.TargetEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SweetSugar.Scripts.TargetScripts.TargetSystem
{
    /// <summary>
    /// target container keeps the object should be collected, its count, sprite, color
    /// </summary>
    [Serializable]
    public class SubTargetContainer
    {
        ///using to keep count of targets
        public GameObject targetPrefab;

        public int count;
        int Savecount;
        public int preCount;
        public Object extraObject;
        public Object[] extraObjects;
        public int color;
        public TargetGUI TargetGui;
        public TargetContainer targetLevel;
        public CollectingTypes collectingAction;

        public SubTargetContainer(GameObject _target, int _count, Object _extraObject)
        {
            targetPrefab = _target;
            count = _count;
            preCount = count;
            extraObject = _extraObject;
        }

        public void changeCount(int i)
        {
            count += i;
            if (count < 0) count = 0;
            preCount = count;
        }

        public int GetCount()
        {
            return count;
        }

        public SubTargetContainer DeepCopy()
        {
            // SubTargetContainer other = (SubTargetContainer)this.MemberwiseClone();
            var other = new SubTargetContainer(targetPrefab, count, extraObject);
            return other;
        }

    }
}