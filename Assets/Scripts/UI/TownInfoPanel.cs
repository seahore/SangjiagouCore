using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class TownInfoPanel : MonoBehaviour
{
    public MapRenderer MapRenderer;

    public void Set(Town town)
    {
        transform.Find("Town Name").GetComponent<Text>().text = town.Name;
        if(town.IsCapital) {
            transform.Find("Town Name").GetComponent<Text>().text += "бя";
        }
        transform.Find("Position").GetComponent<Text>().text = town.Position.ToString();
        transform.Find("Controller Text").GetComponent<Text>().text = town.Controller.Name;
        transform.Find("Development Text").GetComponent<Text>().text = town.Development.ToString();
    }

    public void Drawback()
    {
        GetComponent<Animator>().SetBool("Open", false);
    }
    public void Show()
    {
        GetComponent<Animator>().SetBool("Open", true);
    }

    public void OnCloseButtonClick()
    {
        Drawback();
        MapRenderer.DeselectTile();
    }
}
