using UnityEngine;
using System.Collections;

public class SEManager : MonoBehaviour
{
    public enum SEs {None, Correct, InCorrect, Button1, Button1Close, Button2};

    /*サウンドエフェクトを指定し、再生する*/
    public void Play(SEs se)
    {
        AudioSource audio = transform.FindChild(se.ToString()).GetComponent<AudioSource>();
        audio.Play();
    }
}