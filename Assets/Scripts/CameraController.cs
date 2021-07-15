using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SangjiagouCore;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float ScrollSpeed = 10;
    public int ScrollTriggerEdgeWidth = 20;
    public float ScrollEasingTime = 0.1f;
    public float ZoomSpeed = 15;
    public float ZoomEasingTime = 0.1f;
    public float MaxZoomDistance = 10;
    public float MinZoomDistance = 2;
    public int PixelsPerUnit = 100;
    Vector2 scrollEasingFactor;

    Rect upArea;
    Rect downArea;
    Rect leftArea;
    Rect rightArea;
    Vector3 velocity;
    Vector3 border;
    float targetSize;

    const float CAMERA_STANDARD_SIZE = 5;

    void Start()
    {
        upArea = transform.position.y + (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) <= Game.CurrentEntities.MapSize.y
            ? new Rect(0, Screen.height - ScrollTriggerEdgeWidth, Screen.width, ScrollTriggerEdgeWidth)
            : Rect.zero;
        downArea = (transform.position.y * PixelsPerUnit) - (0.5 * GetComponent<Camera>().pixelHeight) >= 0
            ? new Rect(0, 0, Screen.width, ScrollTriggerEdgeWidth)
            : Rect.zero;
        rightArea = transform.position.x + (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) <= Game.CurrentEntities.MapSize.x
            ? new Rect(Screen.width - ScrollTriggerEdgeWidth, 1f, ScrollTriggerEdgeWidth, Screen.height)
            : Rect.zero;
        leftArea = transform.position.x - (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) >= 0
            ? new Rect(0, 0, ScrollTriggerEdgeWidth, Screen.height)
            : Rect.zero;

        scrollEasingFactor = Vector2.zero;
        targetSize = CAMERA_STANDARD_SIZE;
    }

    void Update()
    {
        #region 摄像机移动

        border = new Vector3 {
            x = Game.CurrentEntities.MapSize.x,
            y = Game.CurrentEntities.MapSize.y,
            z = 0
        };
        // 如果摄像机位置出某一边的边界，那么那个方向的运动被禁止
        bool up = upArea.Contains(Input.mousePosition) && transform.position.y <= border.y;
        bool down = downArea.Contains(Input.mousePosition) && transform.position.y >= 0;
        bool left = leftArea.Contains(Input.mousePosition) && transform.position.x >= 0;
        bool right = rightArea.Contains(Input.mousePosition) && transform.position.x <= border.x;

        // 更新移动的缓动系数
        if (right) {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, 1, Time.deltaTime / ScrollEasingTime);
        } else if (left) {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, -1, Time.deltaTime / ScrollEasingTime);
        } else {
            scrollEasingFactor.x = Mathf.Lerp(scrollEasingFactor.x, 0, Time.deltaTime / ScrollEasingTime);
        }
        if (up) {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, 1, Time.deltaTime / ScrollEasingTime);
        } else if (down) {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, -1, Time.deltaTime / ScrollEasingTime);
        } else {
            scrollEasingFactor.y = Mathf.Lerp(scrollEasingFactor.y, 0, Time.deltaTime / ScrollEasingTime);
        }

        velocity = new Vector3 {
            x = ScrollSpeed * scrollEasingFactor.x,
            y = ScrollSpeed * scrollEasingFactor.y,
            z = 0
        };

        // 因为摄像机的缩放是缩小视口，所以当缩放较近时移动速度可能会太快，故乘一个相机当前size和标准size的比例
        velocity *= Time.deltaTime * (GetComponent<Camera>().orthographicSize / CAMERA_STANDARD_SIZE);

        transform.Translate(velocity);
        #endregion

        #region 摄像机缩放
        // 更新缩放的缓动系数
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && targetSize <= MaxZoomDistance) {
            targetSize += ZoomSpeed * Time.deltaTime;
        } else if (Input.GetAxis("Mouse ScrollWheel") > 0 && targetSize >= MinZoomDistance) {
            targetSize -= ZoomSpeed * Time.deltaTime;
        }

        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, targetSize, Time.deltaTime / ZoomEasingTime);
        #endregion
    }
}
