using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {
    [SerializeField]
    ParticleSystem DamagedEffectPrefab;
    
    /*攻撃された*/
    public void Damaged()
    {
        Debug.Log("BossDamaged");
        //particle
        Instantiate(DamagedEffectPrefab, transform.position, transform.rotation);
    }
}
