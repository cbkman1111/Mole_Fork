using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBase : MonoBehaviour
{
    public NavMeshAgent agent = null;
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

    private void Update()
    {
        var r = transform.rotation;
        transform.rotation = Quaternion.Euler(r.x,0,r.z);

        if(agent != null)
        {
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                SetState(SkellAnimationState.run);
            }
            else
            {
                SetState(SkellAnimationState.idle);
            }
        }
    }

    public void SetState(SkellAnimationState s)
    {
        if (s == state)
            return;

        string name = SkellAnimationState.idle.ToString();
        switch(state)
        {
            case SkellAnimationState.idle:
            case SkellAnimationState.attack:
            case SkellAnimationState.die:
            case SkellAnimationState.hit:
            case SkellAnimationState.run:
            case SkellAnimationState.run_shoot:
                name = s.ToString();
                break;
        }

        state = s;
        skel.state.SetAnimation(0, name, true);
    }
}
