using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class StateInfoPanel : MonoBehaviour
{
    public void Set(State state)
    {
        transform.Find("State Name").GetComponent<Text>().text = state.Name;
        transform.Find("Monarch Name").GetComponent<Text>().text = state.Monarch.Name;
        transform.Find("Territory Text").GetComponent<Text>().text = state.Territory.Count.ToString();
        transform.Find("Capital Text").GetComponent<Text>().text = state.Capital.Name;
        transform.Find("Population Text").GetComponent<Text>().text = state.Population.ToString();
        transform.Find("Army Text").GetComponent<Text>().text = state.Army.ToString();
        transform.Find("Food Text").GetComponent<Text>().text = state.Food.ToString();
        transform.Find("Satisfaction Text").GetComponent<Text>().text = state.Satisfaction.ToString();
        transform.Find("Ceremony Text").GetComponent<Text>().text = state.Ceremony.ToString();
        transform.Find("Politech Text").GetComponent<Text>().text = state.PoliTech.ToString();
        transform.Find("Militech Text").GetComponent<Text>().text = state.MiliTech.ToString();
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
    }
}
