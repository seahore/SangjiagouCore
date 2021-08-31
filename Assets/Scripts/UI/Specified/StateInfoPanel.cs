using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class StateInfoPanel : MonoBehaviour
{
    public MapRenderer MapRenderer;

    public void Set(State state)
    {
        GetComponent<Image>().color = Color.Lerp(state.PrimaryColor, Color.white, 0.75f);
        transform.Find("Flag").GetComponent<Image>().sprite = state.Flag;
        transform.Find("State Name").GetComponent<Text>().text = state.Name;
        transform.Find("Monarch Name").GetComponent<Text>().text = state.Monarch.Name;
        transform.Find("Territory Text").GetComponent<Text>().text = state.Territory.Count.ToString();
        transform.Find("Capital Text").GetComponent<Text>().text = state.Capital.Name;
        transform.Find("Population Text").GetComponent<Text>().text = state.Population.ToString();
        transform.Find("Army Text").GetComponent<Text>().text = state.Army.ToString();
        transform.Find("Food Text").GetComponent<Text>().text = state.Food.ToString();
        transform.Find("Satisfaction Text").GetComponent<Text>().text = state.Satisfaction.ToString();
        transform.Find("Ceremony Text").GetComponent<Text>().text = state.Ceremony.ToString();
        transform.Find("Politech Text").GetComponent<Text>().text = state.Politech.ToString();
        transform.Find("Militech Text").GetComponent<Text>().text = state.Militech.ToString();
        var l = new List<StackedBar.StackedBarItem>();
        foreach (var kv in state.InfluenceOfSchools) {
            l.Add(new StackedBar.StackedBarItem(kv.Key.Name, kv.Key.Color, kv.Value));
        }
        transform.Find("Influence Table").GetComponent<StackedBar>().Items = l;
        transform.Find("Influence Table").GetComponent<StackedBar>().Reset();
        transform.Find("School Name Text").GetComponent<Text>().text = "";
        transform.Find("Influence Value Text").GetComponent<Text>().text = "";
        transform.Find("Influence Ratio Text").GetComponent<Text>().text = "";
    }

    public void OnEnterInfluenceTable(StackedBar.StackedBarItem item, float ratio)
    {
        transform.Find("School Name Text").GetComponent<Text>().text = item.Name;
        transform.Find("Influence Value Text").GetComponent<Text>().text = item.Value.ToString();
        transform.Find("Influence Ratio Text").GetComponent<Text>().text = ((int)(ratio * 100)).ToString() + '%';
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
