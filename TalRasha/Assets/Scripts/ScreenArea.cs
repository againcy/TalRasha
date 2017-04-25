using UnityEngine;
using System.Collections;

public class ScreenArea : MonoBehaviour
{
    public Canvas canvas;

    private RectTransform rt;
    private float left;
    private float right;
    private float top;
    private float bottom;
	void Start ()
	{
        rt = this.gameObject.GetComponent<RectTransform>();
        left = rt.localPosition.x - rt.rect.width / 2;
        right = rt.localPosition.x + rt.rect.width / 2;
        top = rt.localPosition.y + rt.rect.height / 2;
        bottom = rt.localPosition.y - rt.rect.height / 2;

    }
    /// <summary>
    /// 检测是否包含在area
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
	public bool Contains(Vector2 position)
    {
        var posX = position.x - canvas.pixelRect.width/2;
        var posY = position.y - canvas.pixelRect.height/2;
        if (posX >= left && posX <= right && posY >= bottom && posY <= top) return true;
        else return false;
    }
}