using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SangjiagouCore;
using static SangjiagouCore.Utilities.ChineseNumerals;

public class ScenariosPanel : MonoBehaviour
{
    public GameObject MaskPrefab;
    public GameObject ScenarioSelection;
    public GameObject SelectSchoolPanelPrefab;

    UIHandler handler;

    List<History> histories;

    void Start()
    {
        handler = GameObject.FindWithTag("UIHandler").GetComponent<UIHandler>();

        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");

        histories = Game.LoadHistories();

        var l = transform.Find("Scroll View/Viewport/Scenarios List");
        var unitHeight = ScenarioSelection.GetComponent<RectTransform>().rect.height;
        int scenariosCount = 0;
        foreach (var h in histories) {
            scenariosCount += h.Scenarios.Count;
        }
        l.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((unitHeight + 8) * scenariosCount) + 8);
        float t = -8;
        foreach (var h in histories) {
            foreach (var s in h.Scenarios) {
                var o = Instantiate(ScenarioSelection, l.transform);
                o.GetComponent<Toggle>().group = l.GetComponent<ToggleGroup>();
                o.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn) {
                    if (isOn) {
                        transform.Find("Filename Input").GetComponent<InputField>().text = s.GameFilePath;
                        transform.Find("Introduction Text").GetComponent<Text>().text = s.Introduction;
                    }
                });
                o.GetComponent<RectTransform>().offsetMin = new Vector2(8, t - unitHeight);
                o.GetComponent<RectTransform>().offsetMax = new Vector2(-8, t);
                string fullImagePath = Game.HistoriesPath + Path.DirectorySeparatorChar + s.ImagePath;
                if (File.Exists(fullImagePath)) {
                    var t2d = new Texture2D(100, 100);
                    t2d.LoadImage(File.ReadAllBytes(fullImagePath));
                    Sprite sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0));
                    o.transform.Find("Image").GetComponent<Image>().sprite = sprite;
                }
                o.transform.Find("History Name").GetComponent<Text>().text = h.Name;
                o.transform.Find("Start Date").GetComponent<Text>().text = s.Name;
                t -= unitHeight + 4;
            }
        }
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

    public void OnLoadButtonClick(bool switchScene)
    {
        string filename = transform.Find("Filename Input").GetComponent<InputField>().text.Trim();
        if (filename == "") return;
        try {
            Game.LoadGame(filename, true, false);
        } catch(FileNotFoundException) {
            handler.ShowWarningBox($"\'{filename}\'无效。");
        } catch (IOException) {
            handler.ShowWarningBox($"存档文件正在被占用");
        }

        if (switchScene) {
            handler.SetLoadingMask(() => {
                SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
                void f(Scene scene, LoadSceneMode mode) {
                    if (scene.name == "GameScene")
                        OnLoaded();
                    SceneManager.sceneLoaded -= f;
                };
                SceneManager.sceneLoaded += f;
            });
        } else {
            OnLoaded();
            GetComponent<Animator>().SetTrigger("Close");   // 后面还有一个选择学派的面板要显示，所以不解除模糊
        }
    }

    public void OnLoaded()
    {
        GameObject.FindGameObjectWithTag("Tilemap").GetComponent<MapRenderer>().RefreshMap();

        GameObject.Find("Player Panel").transform.Find("Next Turn Button/Text").GetComponent<Text>().text = $"昭公{Int2Chinese(Game.CurrentEntities.Year)}年{Int2Chinese(Game.CurrentEntities.Month)}月";

        Instantiate(SelectSchoolPanelPrefab, GameObject.FindGameObjectWithTag("UIHandler").GetComponent<UIHandler>().UpperUICanvas.transform);
    }
}
