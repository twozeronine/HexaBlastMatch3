using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;

    private static Managers Instance
    {
        get
        {
            init();
            return instance;
        }
    }

    #region Contents

    private readonly GameManager game = new GameManager();
    private readonly StageManager stage = new StageManager();
    

    public static GameManager Game => Instance.game;
    public static StageManager Stage => Instance.stage;
    
    #endregion

    #region Core

    private readonly DataManager data = new DataManager();
    private readonly InputManager input = new InputManager();
    private readonly PoolManager pool = new PoolManager();
    private readonly ResourceManager resource = new ResourceManager();
    private readonly SceneManagerEx scene = new SceneManagerEx();
    private readonly UIManager ui = new UIManager();

    public static DataManager Data => Instance.data;
    public static InputManager Input => Instance.input;
    public static PoolManager Pool => Instance.pool;
    public static ResourceManager Resource => Instance.resource;
    public static SceneManagerEx Scene => Instance.scene;
    public static UIManager UI => Instance.ui;
    #endregion

    void Start()
    {
        init();
    }

    void Update()
    {
        input.OnUpdate();
        game.OnUpdate();
    }

    private static void init()
    {
        if (instance == null)
        {
            var go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();
            instance.gameObject.SetActive(true);

            instance.data.Init();
            instance.input.Init();
            instance.pool.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
        UI.Clear();
    }
}