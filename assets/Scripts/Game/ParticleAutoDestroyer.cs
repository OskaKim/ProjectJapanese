using UnityEngine;

/*particleを自動的に削除*/
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoDestroyer : MonoBehaviour {

    float lifetime = 0;

    void Start()
    {
        lifetime = GetComponent<ParticleSystem>().startLifetime;
    }    
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            Destroy(gameObject);
    }
}
