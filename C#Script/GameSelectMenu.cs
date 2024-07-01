using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSelectMenu : MonoBehaviour
{
    public GameObject EQBTN;
    public GameObject CQBTN;

    public AudioClip clickse;
    AudioSource audiosource1;


    void Start()
    {
        audiosource1 = GetComponent<AudioSource>();
    }
    public void EQClick(){
        audiosource1.PlayOneShot(clickse);
        SceneManager.LoadScene("EQgame");
    }
    public void CQClick(){
        audiosource1.PlayOneShot(clickse);
        SceneManager.LoadScene("CQgame");
    }
}
