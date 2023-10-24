using Common.Global;
using Common.Scene;
using Common.UIObject;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Menu
{
    public class UIMenuDemo : MenuBase
    {
        private KnightControl _knightControl = null;
         
        [SerializeField]
        private AnimationCurve _timeScale = null;
        
        public bool InitMenu()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var objects = scene.GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].name == "Knight")
                {
                    _knightControl = objects[i].GetComponent<KnightControl>(); 
                    break;
                }
            }

            return true;
        }

        public override void OnValueChanged(Slider slider, float f)
        {
            if (slider.name == "Slider - TimeScale")
            {
                var scale= _timeScale.Evaluate(f);
                _knightControl.SetTimeScale(scale);
            }
        }

        protected override void OnClick(Button btn)
        {
            string name = btn.name;
            if(name == "Button - Back")
            {
                AppManager.Instance.ChangeScene(SceneBase.Scenes.SceneMenu);
            }
            else if(name == "Run")
            {
                _knightControl.running();
            }
            else if(name == "Jump")
            {
                _knightControl.jump();
            }
            else if(name == "Stun")
            {
                _knightControl.stun();
            }
            else if(name == "Attack_1")
            {
                _knightControl.attack_1();
            }
            else if(name == "Hit")
            {
                _knightControl.getHit();
            }
            else if(name == "Death")
            {
                _knightControl.death();
            }
            else if(name == "Attack_2")
            {
                _knightControl.attack_2();
            }
            else if(name == "Skill_2")
            {
                _knightControl.skill_2();
            }
            else if(name == "Skill_1")
            {
                _knightControl.skill_1();
            }
            else if(name == "Skill_3")
            {
                _knightControl.skill_3();
            }
            else if(name == "Idle")
            {
                _knightControl.idle();
            }
            else if(name == "Walk")
            {
                _knightControl.walking();
            }
        }
    }
}
