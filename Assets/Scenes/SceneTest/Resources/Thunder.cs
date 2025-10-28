using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Thunder : MonoBehaviour
{
    [SerializeField] private ParticleSystem ParticleSystem;
    [SerializeField] private ParticleSystem ParticleSystemRotation;

    //private MaterialPropertyBlock mpb;
    //private float percentage = 0f;
    //private MEC.CoroutineHandle CoroutineHandle;

    public void SetAngle(Transform from, Transform trans)
    {
        // from 에서 trans 각도 구하기.
        Vector3 direction = (trans.position - from.position).normalized;
        
        // 각도(라디안) 구하기
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = trans.position;
        // Thunder 오브젝트를 해당 방향으로 회전
        var shape = ParticleSystemRotation.shape;
        var q = Quaternion.LookRotation(direction);
        shape.rotation = q.eulerAngles;

        ParticleSystem.Stop();
        ParticleSystem.Play();

        Debug.DrawLine(transform.position, shape.rotation, Color.red, 4f);
    }

    public bool IsExpire()
    {
        if (ParticleSystem != null && ParticleSystem.isStopped == true)
        {
            return true;
        }

        return false;
    }
    
    public void StartEffect()
    {
        //percentage = 0;
        //mpb = new MaterialPropertyBlock();
        //mpb.SetFloat("_Slider", percentage);
        //lineRenderer.material.SetFloat("_Slider", percentage);
        //lineRenderer.SetPropertyBlock(mpb);
        
        //CoroutineHandle.IsRunning = false;
        //CoroutineHandle = MEC.Timing.RunCoroutine(UpdatePercent());
    }

    private IEnumerator<float> UpdatePercent()
    {
        yield return MEC.Timing.WaitForOneFrame;
        /*
        yield return MEC.Timing.WaitForOneFrame;
        while (percentage < 1.0f)
        {
            //percentage += 0.01f;
            percentage += 0.1f;
            if (percentage > 1.0f)
                percentage = 1.0f;

            //mpb.SetFloat("_Slider", percentage);
            //lineRenderer.SetPropertyBlock(mpb);
            //lineRenderer.material.SetFloat("_Slider", percentage);

            //yield return MEC.Timing.WaitForSeconds(0.25f);
            yield return MEC.Timing.WaitForOneFrame;
        }
        */
    }
}
