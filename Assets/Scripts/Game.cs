using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    public static AudioSource audioSource;
    public static AudioClip[] audioClips
    {
        get
        {
            clips ??= Resources.LoadAll<AudioClip>("AudioClips");
            return clips;
        }
    }
    public static float alertRotation;

    private static AudioClip[] clips;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        alertRotation = 0f;
    }

    void Update()
    {
        alertRotation += 5f * Time.deltaTime;
    }

    public static bool ClickDetected(bool isRightClick = true)
    {
        bool detected = false;

        bool mouseClicked = Input.GetMouseButtonDown(isRightClick ? 0 : 1);
        bool buttonClicked = Input.GetKeyDown(isRightClick ? KeyCode.J : KeyCode.K);
        bool uiClicked = EventSystem.current.IsPointerOverGameObject();

        if ((mouseClicked && !uiClicked) || buttonClicked)
            detected = true;

        return detected;
    }

    public static IEnumerator PlayAudio(string clipName, float volumeScale = 1f, float delay = 0f)
    {
        AudioClip clip = Array.Find(audioClips, (e) => e.name == clipName);

        if (clip == null)
            Debug.LogError(clipName + " does not exist in Resources/AudioClips");

        yield return new WaitForSeconds((float)delay);

        audioSource.PlayOneShot(clip, (float)volumeScale);
    }
}
