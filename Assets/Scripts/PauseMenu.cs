using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenu;
    public static UnityEvent FreezeCameraEvent = new UnityEvent();
    public static UnityEvent ResumeCameraEvent = new UnityEvent();

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isPaused) {
            Resume();
        }
    }

    public void Resume()
    {
        ResumeCameraEvent.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        FreezeCameraEvent.Invoke();
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Pause();
        }
    }
}
