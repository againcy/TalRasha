using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CameraBoundry
{
    public float xMin;
    public float xMax;
    public float zMin;
    public float zMax;
}
public class CameraMover : MonoBehaviour
{
    public bool flagEnable;
    public CameraBoundry boundry;
    public float speed;
    public float moveDist;

    private float curSpeed;
    private float cameraWidth;
    private float cameraHeight;
    private float originHeight = 5.0f;

    private Vector3 targetPosition;
    private bool focusing;
    private bool scrolling;

    private float startTime;

	void Start ()
	{
        curSpeed = speed;
        cameraWidth = Camera.main.pixelWidth;
        cameraHeight = Camera.main.pixelHeight;
        focusing = false;
        scrolling = false;
	}
    void Update()
    {
        if (flagEnable == false) return;
        var mouse = Input.mousePosition;
        var curPosition = Camera.main.transform.position;
        var curVelocity = Camera.main.velocity;
        float nextX = curPosition.x;
        float nextZ = curPosition.z;
        
        if (focusing == true)
        {
            //聚焦到指定位置
            //已移动到指定位置
            if (curPosition.x == targetPosition.x && curPosition.z == targetPosition.z && curVelocity == new Vector3())
            {
                focusing = false;
                curSpeed = speed;
            }
            nextX = Mathf.SmoothDamp(curPosition.x, targetPosition.x, ref curVelocity.x, moveDist / curSpeed);
            nextZ = Mathf.SmoothDamp(curPosition.z, targetPosition.z, ref curVelocity.z, moveDist / curSpeed);
        }
        else 
        {
            //检测是否有卷轴滚动操作
            if (mouse.x <= cameraWidth * 0.01 || mouse.x >= cameraWidth * 0.99 || mouse.y <= cameraHeight * 0.01 || mouse.y >= cameraHeight * 0.99)
            {
                if (scrolling == false) startTime = Time.time;
                scrolling = true;
                targetPosition = curPosition;
                
                if (mouse.x <= cameraWidth * 0.01) targetPosition.x = curPosition.x - moveDist/10;
                else if (mouse.x >= cameraWidth * 0.99) targetPosition.x = curPosition.x + moveDist / 10;

                if (mouse.y <= cameraHeight * 0.01) targetPosition.z = curPosition.z - moveDist / 10;
                else if (mouse.y >= cameraHeight * 0.99) targetPosition.z = curPosition.z + moveDist / 10;
            }
            //卷轴滚动
            if (targetPosition != curPosition)
            {
                float distCovered = speed/20 * (Time.time - startTime);
                float fracJourney = distCovered / Vector3.Distance(curPosition, targetPosition);
                var nextPosition = Vector3.Lerp(curPosition, targetPosition, fracJourney);
                 if (fracJourney >= 1) scrolling = false;
                nextX = nextPosition.x;
                nextZ = nextPosition.z;
            }
        }
        Camera.main.transform.position = new Vector3(
                Mathf.Clamp(nextX, boundry.xMin, boundry.xMax),
                curPosition.y,
                Mathf.Clamp(nextZ, boundry.zMin, boundry.zMax));


    }

    public void Focus(Vector3 target)
    {
        curSpeed = speed * 2;
        focusing = true;
        scrolling = false;
        targetPosition.x = target.x;
        targetPosition.z = target.z - originHeight;//camera夹角45度
    }
}