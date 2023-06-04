using System;
using System.Linq;
using Malee;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEditor;
using UnityEngine;
namespace SweetSugar.Scripts.TargetScripts.TargetEditor
{
    [CreateAssetMenu(fileName = "TargetLevel", menuName = "TargetLevel", order = 1)]
    public class TargetLevel : ScriptableObject
    {
        [Reorderable(null,"Target",null)]
        public TargetList targets;
        private TargetEditorScriptable targetsEditor;

        private void OnEnable()
        {
            var t = Resources.Load("Levels/Targets/" + this.name);
        }

#if UNITY_EDITOR
        public void LoadFromLevel(LevelData levelData, TargetEditorScriptable _targetsEditor, bool checkCount = true)
        {
            targetsEditor = _targetsEditor;
            SpriteList sprites = new SpriteList();
            this.targets.Clear();

            for (var index = 0; index < levelData.subTargetsContainers.Count; index++)
            {  
                var container = levelData.subTargetsContainers[index];
                if (container.count > 0 || !checkCount)
                {
                    Sprite spr = null;
                    if (container.extraObject != null) spr = (Sprite) container.extraObject;
                    var targetContainer = levelData.GetTargetEditor();
                    var sprites1 = SpriteList(targetContainer, spr);
                    ResetSprites(levelData, targetContainer, sprites1, index, container.count);
                }
            }
            sprites = new SpriteList();
            sprites.Add(new SpriteObject {icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/SweetSugar/Textures_png/Win Scene/Star_02.png")});
            this.targets.Add(CreateTarget(levelData.GetTargetByNameEditor("Stars"), sprites, 1, levelData));
            saveData();
        }

        private void ResetSprites(LevelData levelData, TargetContainer targetContainer, SpriteList sprites1, int index, int containerCount)
        {
            if (levelData.target.name == "Ingredients")
            {
                if (index == 0)
                    targets.Add(CreateTarget(targetContainer, sprites1, containerCount, levelData));
                else
                {
                    sprites1 = new SpriteList();
                    sprites1.Add(new SpriteObject {icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/SweetSugar/Textures_png/Items/ingredient_02.png")});
                    targets.Add(CreateTarget(targetContainer, sprites1, containerCount, levelData));
                }
            }
            else if (targetContainer.name != "Stars")
            {
                targets.Insert(0,CreateTarget(targetContainer, sprites1, containerCount, levelData));
            }
        }

        private SpriteList SpriteList(TargetContainer type, Sprite spr, bool GroupTarget=false)
        {
            SpriteList sprites;
            var targetContainer = targetsEditor.targets.First(i => i.name == type.name);
            if (spr != null && targetContainer.defaultSprites.Any(i => i.sprites.Any(x => x.icon.name == spr.name)))
            {
                sprites = targetContainer.defaultSprites.First(i => i
                    .sprites.Any(x => x.icon.name == spr.name)).sprites.Copy();
            }
            else sprites = targetContainer.defaultSprites[0].sprites.Copy();

            return sprites;
        }
        TargetObject CreateTarget(TargetContainer type, SpriteList sprites, int count, LevelData levelData)
        {
            var targetObject = new TargetObject();
            targetObject.sprites = new SpriteList();
            targetObject.targetType = new TargetType();
            targetObject.targetType.type = levelData.GetTargetIndex(type.name);
            targetObject.sprites = sprites;
            targetObject.CountDrawer = new Count();
            targetObject.CountDrawer.count = count;
            return targetObject;
        }
        
        public void saveData(){
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ChangeTarget(TargetContainer targetContainer, int newselectedTarget, LevelData levelData, TargetEditorScriptable targetEditorScriptable)
        {
            var targetDelete = targets.Where(i => i.targetType.GetTarget().name != "Stars").ToList();
            foreach (var targetObject in targetDelete) targets.Remove(targetObject);
            var newTarget = targetEditorScriptable.targets[newselectedTarget];
            var list = targetEditorScriptable.targets.Where(i => i.name == newTarget.name).SelectMany(i => i.defaultSprites).OrderByDescending(i=>i.sprites[0].icon.name);
            foreach (var sprArray in list)
            {
                ResetSprites(levelData, newTarget, sprArray.sprites, 0, 0);
            }
            saveData();
        }
#endif
    }

    [Serializable]
    public class TargetList : ReorderableArray<TargetObject>
    {
    }

    [Serializable]
    public class SpriteList : ReorderableArray<SpriteObject>
    {
        public SpriteList Copy()
        {
            SpriteList list = new SpriteList();
            foreach (var item in this)
            {
                list.Add(item.Copy());
            }

            return list;
        }
    }
    [Serializable]
    public class SpriteObject
    {
        public Sprite icon;
        public bool uiSprite;
        public SpriteObject Copy()
        {
            return (SpriteObject)MemberwiseClone();
        }
    }

    [Serializable]
    public class TargetObject
    {
        public TargetType targetType;
        [Reorderable]
        public SpriteList sprites;
        public Count CountDrawer;
        [Tooltip("Not finish the level even the target is complete until move/time is out")]
        public bool NotFinishUntilMoveOut;
        public BoolScoreShow ShowTheScoreForStar;
    }
    
    [Serializable]
    public class BoolScoreShow
    {
        public bool ShowTheScore;
    }

    [Serializable]
    public class Count
    {
        public int count;
    }
    [Serializable]
    public class TargetType
    {
        public int type;
        private TargetEditorScriptable _targetEditorScriptable;

        public TargetEditorScriptable EditorScriptable
        {
            get
            {
                if (_targetEditorScriptable == null) _targetEditorScriptable = Resources.Load<TargetEditorScriptable>("Levels/TargetEditorScriptable");
                return _targetEditorScriptable;
            }
        }

        public TargetContainer GetTarget()
        {
            return EditorScriptable.targets[type];
        }
    }


}
