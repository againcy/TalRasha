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

	void Start ()
	{
        curSpeed = speed;
        cameraWidth = Camera.main.pixelWidth;
        cameraHeight = Camera.main.pixelHeight;
        focusing = false;
	}
    void Update()
    {
        if (flagEnable == false) return;
        var mouse = Input.mousePosition;
        var curPosition = Camera.main.transform.position;
        var curVelocity = Camera.main.velocity;
        float nextX = curPosition.x;
        float nextZ = curPosition.z;

        
        if (focusing == false)
        {
            //随鼠标移动
            targetPosition = curPosition;
            if (mouse.x < cameraWidth * 0.02) targetPosition.x = curPosition.x - moveDist;
            else if (mouse.x > cameraWidth * 0.98) targetPosition.x = curPosition.x + moveDist;

            if (mouse.y < cameraHeight * 0.02) targetPosition.z = curPosition.z - moveDist;
            else if (mouse.y > cameraHeight * 0.98) targetPosition.z = curPosition.z + moveDist;
        }
        else
        {
            //聚焦到指定位置
            //已移动到指定位置
            if (curPosition.x == targetPosition.x && curPosition.z == targetPosition.z && curVelocity == new Vector3())
            {
                focusing = false;
                curSpeed = speed;
            }
        }

        nextX = Mathf.SmoothDamp(curPosition.x, targetPosition.x, ref curVelocity.x, moveDist / curSpeed);
        nextZ = Mathf.SmoothDamp(curPosition.z, targetPosition.z, ref curVelocity.z, moveDist / curSpeed);
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(nextX, boundry.xMin, boundry.xMax),
            curPosition.y,
            Mathf.Clamp(nextZ, boundry.zMin, boundry.zMax));
    }

    public void Focus(Vector3 target)
    {
        curSpeed = speed * 2;
        focusing = true;
        targetPosition.x = target.x;
        targetPosition.z = target.z - originHeight;//camera夹角45度
    }
}