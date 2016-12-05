using UnityEngine;
using System.Collections;

public class SEManager : MonoBehaviour
{
    public enum SEs { Correct, InCorrect};

    /*サウンドエフェクトを指定し、再生する*/
    public void Play(SEs se)
    {
        AudioSource audio = transform.FindChild(se.ToString()).GetComponent<AudioSource>();
        audio.Play();
    }
}