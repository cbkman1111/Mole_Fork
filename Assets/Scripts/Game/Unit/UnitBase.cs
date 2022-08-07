using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public SkeletonAnimation skel = null;
    public enum SkellAnimationState { attack = 0, die, hit, idle, run, run_shoot};
    public SkellAnimationState state = SkellAnimationState.idle;

    // Start is called before the first frame update
    void Start()
    {
        var scene = AppManager.Instance.GetCurrentScene();
        
        Vector3 v = scene.MainCamera.transform.position - transform.position; 
        v.z = 0;
        v.y = 0;

        skel.transform.LookAt(scene.MainCamera.transform.position - v);
    }

    public void SetState(SkellAnimationState state)
    {
        string name = SkellAnimationState.idle.ToString();
        switch(state)
        {
            case SkellAnimationState.idle:
            case SkellAnimationState.attack:
            case SkellAnimationState.die:
            case SkellAnimationState.hit:
            case SkellAnimationState.run:
            case SkellAnimationState.run_shoot:
                name = state.ToString();
                break;
        }

        skel.state.SetAnimation(0, name, true);
    }
}
