using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public VideoClip endingVideo;   

    void Start()
    {
        videoPlayer.clip = endingVideo;
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoEnd;

        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(ReturnToTitle());
    }

    IEnumerator ReturnToTitle()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Sauvegardes supprimées !");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }
}