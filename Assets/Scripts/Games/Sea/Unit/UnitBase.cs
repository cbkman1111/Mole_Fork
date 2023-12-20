using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using Common.Global;
using UnityEngine;
using UnityEngine.AI;

public class UnitBase : MonoBehaviour
{
    public NavMeshAgent agent = null;
    public SkeletonAnimation skel = null;
    protected enum SkellAnimationState { attack = 0, die, hit, idle, run, run_shoot};
    protected SkellAnimationState state = SkellAnimationState.idle;

    // Start is called before the first frame update
    void Start()
    {
        SetAngle();

        StartCoroutine("Proc", gameObject);
    }

    public virtual void SetAngle()
    {
        var scene = AppManager.Instance.CurrScene;
        Vector3 v = scene.MainCamera.transform.position - transform.position;
        v.z = 0;
        v.y = 0;

        skel.transform.LookAt(scene.MainCamera.transform.position - v);
    }
    
    
    private System.Collections.IEnumerator Proc()
    {
        while (true)
        {
            var r = transform.rotation;
            transform.rotation = Quaternion.Euler(r.x, 0, r.z);

            if (agent != null)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    SetState(SkellAnimationState.run);
                }
                else
                {
                    SetState(SkellAnimationState.idle);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }


    protected void SetState(SkellAnimationState s)
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
