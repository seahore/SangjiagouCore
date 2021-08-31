using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class CheckStatesPanel : MonoBehaviour
{
    public GameObject MaskPrefab;
    public GameObject StateSelection;

    void Start()
    {
        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");

        var list = transform.Find("States Scroll View/Viewport/States List");

        var unitHeight = StateSelection.GetComponent<RectTransform>().rect.height;
        var unitWidth = StateSelection.GetComponent<RectTransform>().rect.width;

        list.GetComponent<RectTransform>().offsetMax = new Vector2(Game.CurrentEntities.States.Count * (unitWidth + 8) - 8, 0);

        float tw = 8;
        for (int i = 0; i < Game.CurrentEntities.States.Count; ++i) {
            var s = Game.CurrentEntities.States[i];
            var o = Instantiate(StateSelection, list).transform;
            o.GetComponent<Toggle>().group = list.GetComponent<ToggleGroup>();
            o.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => {
                if (isOn) {
                    transform.Find("State And Monarch Name").GetComponent<Text>().text = $"{s.Name} <size=18>{s.Monarch.Name}</size>";
                    transform.Find("Territory Text").GetComponent<Text>().text = s.Territory.Count.ToString();
                    transform.Find("Capital Text").GetComponent<Text>().text = s.Capital.Name;
                    transform.Find("Population Text").GetComponent<Text>().text = s.Population.ToString();
                    transform.Find("Army Text").GetComponent<Text>().text = s.Army.ToString();
                    transform.Find("Food Text").GetComponent<Text>().text = s.Food.ToString();
                    transform.Find("Satisfaction Text").GetComponent<Text>().text = s.Satisfaction.ToString();
                    transform.Find("Ceremony Text").GetComponent<Text>().text = s.Ceremony.ToString();
                    transform.Find("Politech Text").GetComponent<Text>().text = s.Politech.ToString();
                    transform.Find("Militech Text").GetComponent<Text>().text = s.Militech.ToString();
                    var l = new List<StackedBar.StackedBarItem>();
                    foreach (var kv in s.InfluenceOfSchools) {
                        l.Add(new StackedBar.StackedBarItem(kv.Key.Name, kv.Key.Color, kv.Value));
                    }
                    transform.Find("Influence Table").GetComponent<StackedBar>().Items = l;
                    transform.Find("Influence Table").GetComponent<StackedBar>().Reset();
                    transform.Find("School Name Text").GetComponent<Text>().text = "";
                    transform.Find("Influence Value Text").GetComponent<Text>().text = "";
                    transform.Find("Influence Ratio Text").GetComponent<Text>().text = "";
                    var reltab = transform.Find("Relationships Scroll View/Viewport/Relationships Table").GetComponent<Table>();
                    reltab.Reset();
                    foreach (var kv in s.AttitudeTowards) {
                        reltab.AddRow(kv.Key, kv.Value);
                    }
                }
            });
            o.GetComponent<Toggle>().group = list.GetComponent<ToggleGroup>();
            o.GetComponent<RectTransform>().offsetMin = new Vector2(tw , -10 - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(tw + unitWidth, -10);
            o.Find("Flag").GetComponent<Image>().sprite = s.Flag;
            o.Find("Button Face").GetComponent<Image>().color = Color.Lerp(s.PrimaryColor, Color.white, 0.5f);
            o.Find("Name").GetComponent<Text>().text = s.Name;
            o.Find("Monarch Name").GetComponent<Text>().text = s.Monarch.Name;
            tw += unitWidth + 4;
        }
    }

    public void OnEnterInfluenceTable(StackedBar.StackedBarItem item, float ratio)
    {
        transform.Find("School Name Text").GetComponent<Text>().text = item.Name;
        transform.Find("Influence Value Text").GetComponent<Text>().text = item.Value.ToString();
        transform.Find("Influence Ratio Text").GetComponent<Text>().text = ((int)(ratio * 100)).ToString() + '%';
    }

    public void OnCloseButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
        }
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在关闭动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.parent.gameObject);
    }
}
