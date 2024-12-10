using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TitleController : MonoBehaviour
{
    private enum TitleType
    {
        Enter,
        Title,
        Selection,
        Loader,
        GamePlay
    }

    private const string GAME_SCENE_NAME = "SampleScene";
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Enter = Animator.StringToHash("Enter");
    private static readonly int Exit = Animator.StringToHash("Exit");

    [Space]
    public GameObject index1;
    
    [Space]
    public GameObject title;
    public AudioSource bgm;
    public GameObject buttonRoot;
    public Button startButton;
    public Button collectionButton;
    public Button thanksButton;
    public Button exitButton;
    private readonly List<Button> m_titleButtons = new();
    private int m_titleSelectIndex = -1;

    [Space]
    public GameObject selection;
    public Button selectPistol;
    public Button selectSniperRifle;
    public Button selectSubmachine;
    public WeaponConfig pistolConfig;
    public WeaponConfig sniperRifleConfig;
    public WeaponConfig submachineConfig;
    private readonly List<Button> m_selectionButtons = new();
    private int m_weaponSelectIndex;
    
    [Space]
    public GameObject loader;
    public List<GameObject> m_tips = new();
    
    private TitleType m_titleType = TitleType.Enter;
    
    // Start is called before the first frame update
    void Start()
    {
        InitTitle();
        InitSelection();

        StartEnter();
    }

    #region Init

    private void InitTitle()
    {
        m_titleButtons.Clear();
        m_titleButtons.Add(startButton);
        m_titleButtons.Add(collectionButton);
        m_titleButtons.Add(thanksButton);
        m_titleButtons.Add(exitButton);
        ChangeTitleSelect(0);
        for (var i = 0; i < m_titleButtons.Count; ++i)
        {
            var index = i;
            var buttonGo = m_titleButtons[i];
            var button = buttonGo.GetComponent<Button>();
            var controller = buttonGo.GetComponent<TitleButtonController>();
            controller.pointerEnterEvent = data =>
            {
                if (button.interactable == false) return;
                ChangeTitleSelect(index);
            };
            
            controller.pointerClickEvent = data =>
            {
                if (button.interactable == false) return;
                OnTitlePress(buttonGo);
            };
        }
    }

    private void InitSelection()
    {
        m_selectionButtons.Clear();
        m_selectionButtons.Add(selectPistol);
        m_selectionButtons.Add(selectSniperRifle);
        m_selectionButtons.Add(selectSubmachine);
        for (var i = 0; i < m_selectionButtons.Count; ++i)
        {
            var index = i;
            var button = m_selectionButtons[i];
            var controller = button.GetComponent<TitleButtonController>();
            controller.pointerEnterEvent = data =>
            {
                if (button.interactable == false) return;
                ChangeSelectionSelect(index);
            };
            
            controller.pointerClickEvent = data =>
            {
                if (button.interactable == false) return;
                OnSelectionPress(button);
            };
        }
    }

    #endregion

    #region Switch Title Type

    private void StartEnter()
    {
        index1.SetActive(true);
        var animator = index1.GetComponent<Animator>();
        animator.SetTrigger(Run);
        StartCoroutine(Enter2Title(8));
    }

    private IEnumerator Enter2Title(float delay)
    {
        yield return new WaitForSeconds(delay);
        title.SetActive(true);
        buttonRoot.SetActive(true);
        index1.SetActive(false);
        bgm.gameObject.SetActive(true);
        m_titleType = TitleType.Title;
    }
    
    private void Title2Selection()
    {
        buttonRoot.SetActive(false);
        selection.SetActive(true);
        ChangeSelectionSelect(0);
        m_titleType = TitleType.Selection;
    }

    private void Selection2Title()
    {
        buttonRoot.SetActive(true);
        selection.SetActive(false);
        m_titleType = TitleType.Title;
    }

    private const float SWITCH_TIP_TIME = 5;
    private const float MIN_LOAD_TIME = 8f;
    private float m_loaderTipTimer;
    private float m_loaderTimer;
    private int m_lastTip = -1;
    private void Selection2Loader()
    {
        loader.SetActive(true);
        loader.GetComponent<Animator>().SetTrigger(Enter);
        StartCoroutine(AudioManager.AudioFadesOut(bgm, 2));
        m_loaderTipTimer = SWITCH_TIP_TIME;
        m_lastTip = -1;
        m_titleType = TitleType.Loader;
        m_loaderTimer = 0;
        //异步加载
        StartCoroutine(Loader2Game());
    }

    // 终于要进游戏了
    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
    private IEnumerator Loader2Game()
    {
        var async = SceneManager.LoadSceneAsync(GAME_SCENE_NAME, LoadSceneMode.Additive);
        if (async == null)
        {
            Debug.LogError($"加载游戏场景失败, 请联系管理员 {GAME_SCENE_NAME}");
            yield break;
        }
        
        //异步加载场景
        while (!async.isDone)
        {
            m_loaderTimer += Time.deltaTime;
            yield return null;
        }

        if (m_loaderTimer < MIN_LOAD_TIME)
        {
            var remainTime = MIN_LOAD_TIME - m_loaderTimer;
            yield return new WaitForSeconds(remainTime);
        }
        
        m_titleType = TitleType.GamePlay;
        var scene = SceneManager.GetSceneByName(GAME_SCENE_NAME);
        Debug.Log($"异步加载游戏场景完成 {scene}");
        loader.GetComponent<Animator>().SetTrigger(Exit);
        if (m_lastTip != -1)
        {
            var lastText = m_tips[m_lastTip].GetComponent<TextMeshProUGUI>();
            if (m_showTipCoroutine != null) StopCoroutine(m_showTipCoroutine);
            if (m_hideTipCoroutine != null) StopCoroutine(m_hideTipCoroutine);
            m_hideTipCoroutine = StartCoroutine(HideTips(lastText, 0.5f));
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.SetActiveScene(scene);
        var gameManager = ManagerLocator.Instance.Get<GameManager>();
        gameManager.StartInitFromTitle();
    }

    #endregion
    
    // Update is called once per frame
    void Update()
    {
        switch (m_titleType)
        {
            case TitleType.Enter:
                break;
            case TitleType.Title:
                OnTitleUpdate();
                break;
            case TitleType.Selection:
                OnSelectionUpdate();
                break;
            case TitleType.Loader:
                OnLoaderUpdate();
                break;
            case TitleType.GamePlay:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #region Input Handles

    private int ChangeSelect(int index, int move, List<Button> buttons)
    {
        bool enable;
        var select = index;
        do
        {
            select += move;
            if (select == buttons.Count)
            {
                select = 0;
            }
            enable = buttons[select].GetComponent<Button>().interactable;
        } while (!enable);

        return select;
    }

    private void OnTitleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var next = ChangeSelect(m_titleSelectIndex, 1, m_titleButtons);
            ChangeTitleSelect(next);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            var last = ChangeSelect(m_titleSelectIndex, -1, m_titleButtons);
            ChangeTitleSelect(last);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnTitlePress(m_titleButtons[m_titleSelectIndex]);
        }
    }

    private void OnSelectionUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Selection2Title();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var next = ChangeSelect(m_weaponSelectIndex, 1, m_selectionButtons);
            ChangeSelectionSelect(next);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var last = ChangeSelect(m_weaponSelectIndex, -1, m_selectionButtons);
            ChangeSelectionSelect(last);
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSelectionPress(m_selectionButtons[m_weaponSelectIndex]);
        }
    }
    
    private void OnLoaderUpdate()
    {
        m_loaderTipTimer += Time.deltaTime;
        if (m_loaderTipTimer < SWITCH_TIP_TIME) return;
        
        m_loaderTipTimer = 0;
        var tip = m_lastTip;
        while (tip == m_lastTip)
        {
            tip = Random.Range(0, m_tips.Count);
        }

        if (m_lastTip == -1)
        {
            SwitchTips(null, m_tips[tip], offset: 1f);
        }
        else
        {
            SwitchTips(m_tips[m_lastTip], m_tips[tip]);
        }
        m_lastTip = tip;
    }

    #endregion

    #region Button Events

    private void OnTitlePress(Button pressedButton)
    {
        if (pressedButton == startButton)
        {
            Title2Selection();
        }
        else if (pressedButton == collectionButton)
        {
            //TODO future
        }
        else if (pressedButton == thanksButton)
        {
            //TODO 最后做
        }
        else if (pressedButton == exitButton)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void OnSelectionPress(Button button)
    {
        if (button == selectPistol)
        {
            PlayerController.defaultWeaponConfig = pistolConfig;
        }
        else if (button == selectSniperRifle)
        {
            PlayerController.defaultWeaponConfig = sniperRifleConfig;
        }
        else if (button == selectSubmachine)
        {
            PlayerController.defaultWeaponConfig = submachineConfig;
        }
        else
        {
            return;
        }
        
        Selection2Loader();
    }

    private void ChangeTitleSelect(int buttonIndex)
    {
        if (m_titleSelectIndex != -1)
        {
            var selectedButton = m_titleButtons[m_titleSelectIndex];
            SetTitleButtonSelect(selectedButton, false);
        }
        var button = m_titleButtons[buttonIndex];
        SetTitleButtonSelect(button, true);
        m_titleSelectIndex = buttonIndex;
    }

    private void SetTitleButtonSelect(Button button, bool value)
    {
        var root = button.transform.Find("SelectRoot");
        root.gameObject.SetActive(value);
    }

    private readonly Color m_selectColor = new(1, 0.9f, 0.5f, 1);
    private readonly Color m_deselectColor = Color.white;
    private void ChangeSelectionSelect(int buttonIndex)
    {
        if (m_weaponSelectIndex != -1)
        {
            var oldButton = m_selectionButtons[m_weaponSelectIndex];
            oldButton.GetComponent<Image>().color = m_deselectColor;
        }
        
        var newButton = m_selectionButtons[buttonIndex];
        newButton.GetComponent<Image>().color = m_selectColor;
        m_weaponSelectIndex = buttonIndex;
    }

    #endregion

    #region Utils

    private Coroutine m_hideTipCoroutine;
    private Coroutine m_showTipCoroutine;
    private void SwitchTips(GameObject last, GameObject now, float delay = 0.5f, float offset = 0f)
    {
        if (last == null)
        {
            var nowText = now.GetComponent<TextMeshProUGUI>();
            if (m_showTipCoroutine != null) StopCoroutine(m_showTipCoroutine);
            m_showTipCoroutine = StartCoroutine(ShowTips(nowText, delay, offset));
        }
        else
        {
            var lastText = last.GetComponent<TextMeshProUGUI>();
            var nowText = now.GetComponent<TextMeshProUGUI>();
            if (m_showTipCoroutine != null) StopCoroutine(m_showTipCoroutine);
            if (m_hideTipCoroutine != null) StopCoroutine(m_hideTipCoroutine);
            m_hideTipCoroutine = StartCoroutine(HideTips(lastText, delay));
            m_showTipCoroutine = StartCoroutine(ShowTips(nowText, delay, delay + offset));
        }
    }

    private IEnumerator HideTips(TextMeshProUGUI tip, float delay)
    {
        tip.color = new Color(tip.color.r, tip.color.g, tip.color.b, 1);
        var alphaDelta = 1 / delay;
        while (tip.color.a > 0)
        {
            var alpha = Mathf.Clamp01(tip.color.a - alphaDelta * Time.deltaTime);
            tip.color = new Color(tip.color.r, tip.color.g, tip.color.b, alpha);
            yield return null;
        }
        
        tip.gameObject.SetActive(false);
        m_hideTipCoroutine = null;
    }

    private IEnumerator ShowTips(TextMeshProUGUI tip, float delay, float offset)
    {
        tip.gameObject.SetActive(true);
        tip.color = new Color(tip.color.r, tip.color.g, tip.color.b, 0);
        yield return new WaitForSeconds(offset);
        
        var alphaDelta = 1 / delay;
        while (tip.color.a < 1)
        {
            var alpha = Mathf.Clamp01(tip.color.a + alphaDelta * Time.deltaTime);
            tip.color = new Color(tip.color.r, tip.color.g, tip.color.b, alpha);
            yield return null;
        }

        m_showTipCoroutine = null;
    }

    #endregion
}
