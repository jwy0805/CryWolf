using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Portrait : MonoBehaviour
{
    private bool _isActive;
    
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            func(_isActive);
        }
    }

    private void func(bool active)
    {
        string monsterName = gameObject.name.Replace("Button", "");
        string num = GameData.TowerList.Contains(monsterName)
            ? GameData.Tower.FirstOrDefault(item => item.Value == monsterName).Key
            : GameData.Monster.FirstOrDefault(item => item.Value == monsterName).Key;
        string level = num.Substring(1, 1);

        gameObject.SetActive(false);

        switch (level)
        {
            case "0":
                gameObject.SetActive(true);
                if (active == false)
                {
                    Image img = gameObject.GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 0.6f);
                }
                else
                {
                    Image img = gameObject.GetComponent<Image>();
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1.0f);
                }
                break;
            
            default:
                if (active)
                {
                    gameObject.SetActive(true);
                }
                break;
        }
    }
}
