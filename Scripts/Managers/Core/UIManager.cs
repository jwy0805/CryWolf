using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private int _order = 10;
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    private UI_Scene _sceneUI = null;
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject gameObject, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        
        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instanciate($"UI/WorldSpace/{name}");
        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }

        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(gameObject);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instanciate($"UI/SubItem/{name}");
        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }

        return Util.GetOrAddComponent<T>(gameObject);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instanciate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(gameObject);
        _sceneUI = sceneUI;
        
        gameObject.transform.SetParent(Root.transform);

        return sceneUI;
    }
    
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        GameObject gameObject = Managers.Resource.Instanciate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(gameObject);
        _popupStack.Push(popup);
        
        gameObject.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }
        
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
