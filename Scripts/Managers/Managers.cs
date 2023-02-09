using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance;
    private static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    private readonly GameManager _game = new GameManager();
    
    public static GameManager Game => Instance._game;
    // = public static GameManager Game { get { return Instance._game; } }

    #endregion
    
    #region Core
    private InputManager _input = new InputManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private SoundManager _sound = new SoundManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private UIManager _ui = new UIManager();
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SoundManager Sound => Instance._sound;
    public static SceneManagerEx Scene => Instance._scene;
    public static UIManager UI => Instance._ui;

    #endregion
    
    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
        _sound.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            
            s_instance._pool.Init();
            s_instance._sound.Init();
        }
    }
    
    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        Pool.Clear();
    }
}
