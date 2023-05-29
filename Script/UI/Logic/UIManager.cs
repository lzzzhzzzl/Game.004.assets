using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    [Header("加载进度条的选项")]
    public Slider slider;
    public TMP_Text text;
    public Image image;
    [Header("UI组件")]
    public Transform mainUI;
    public Transform otherUI;
    private float loadable;
    private bool canLoad;
    private string currentTask;
    [Header("UI菜单组件")]
    public Button settingsButton;
    public GameObject menuPrefab;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    private GameObject menuCanvas;
    private void Awake()
    {
        settingsButton.onClick.AddListener(TogglePausePanel);
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.IslandMapClickEvent += OnIslandMapClickEvent;
        EventHandler.StartSliderEvent += OnStartSliderEvent;
        EventHandler.SetSliderEvent += OnSetSliderEvent;
        EventHandler.EndSliderEvent += OnEndSliderEvent;
        EventHandler.GameOverEvent += OnGameOverEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.IslandMapClickEvent -= OnIslandMapClickEvent;
        EventHandler.StartSliderEvent -= OnStartSliderEvent;
        EventHandler.SetSliderEvent -= OnSetSliderEvent;
        EventHandler.EndSliderEvent -= OnEndSliderEvent;
        EventHandler.GameOverEvent -= OnGameOverEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        if (menuCanvas.transform.childCount > 0)
        {
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
        }

        string sceneName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }
    public void OnstartGameEvent()
    {
        EventHandler.CallIslandMapClickEvent(false);
        EventHandler.CallstartGameEvent();
    }
    private void OnStartSliderEvent(Sprite sprite)
    {
        loadable = 0;
        slider.value = 0;
        text.text = "加载地图中...";
        canLoad = true;
        image.sprite = sprite;
        StartCoroutine(StartSlider());
    }
    private void OnIslandMapClickEvent(bool isClick)
    {
        mainUI.gameObject.SetActive(!isClick);
        otherUI.gameObject.SetActive(isClick);
    }
    private void OnSetSliderEvent(float newloadable, string newTask)
    {
        loadable = newloadable;
        currentTask = newTask;
    }
    private void OnEndSliderEvent()
    {
        canLoad = false;
    }

    private IEnumerator StartSlider()
    {
        float currentLoadable = 0;
        while (canLoad)
        {
            if (currentLoadable != loadable)
            {
                slider.value = currentLoadable;
                text.text = "正在" + currentTask + "   总体进度" + slider.value * 100 + "%";
                currentLoadable = loadable;
            }

            if (slider.value < loadable)
            {
                slider.value += 0.005f;
                text.text = "正在" + currentTask + "   总体进度" + slider.value * 100 + "%";
            }
            yield return null;
        }
    }
    public void CloseIslandmapUI()
    {
        EventHandler.CallIslandMapClickEvent(false);
    }

    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            System.GC.Collect();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ReturnMenuCancas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }

    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        EventHandler.CallEndGameEvent();
        yield return new WaitForSeconds(1f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }
    public void ReLoadGameButtom()
    {
        Time.timeScale = 1;
        gameOverPanel.SetActive(false);
        EventHandler.CallCloseCurrentScene();
        EventHandler.CallReLoadGameEvent();
    }
    private void OnGameOverEvent()
    {
        System.GC.Collect();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
