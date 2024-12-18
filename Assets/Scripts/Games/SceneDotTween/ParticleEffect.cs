using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    private ParticleSystem[] particleSystems = null;
    private float duration = 0;

    void Awake()
    {
        particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        duration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool alive = false;
        duration += Time.deltaTime;
        string nameAlived = string.Empty;
        float durationAlived = 0;

        for (int num = 0; num < particleSystems?.Length; ++num)
        {
            var particle = particleSystems[num];
            if (particle == null)
                continue;

            //if (particle.gameObject.activeInHierarchy && particle.IsAlive())
            if (particle.gameObject.activeInHierarchy && particle.isPlaying == true)
            {
                alive = true;
                nameAlived = particle.name;
                durationAlived = particle.main.duration;
                break;
            }
        }

        if (!alive)
        {
            Debug.Log($"life time = {duration} - {name}/{nameAlived}/{durationAlived}");
            Destroy(gameObject);
        }
    }
}
