using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ZoomTest : MonoBehaviour
{
    [HideInInspector]
    public float zoomOutMin;  //줌 최소값 설정, by 황승민, 21/03/24
    [HideInInspector]
    public float zoomOutMax = 4;  //줌 최대값 설정, by 황승민, 21/03/24
    [Header("카메라뷰 이미지오브젝트")]
    public GameObject CameraView; //scale 변화를 통해 줌을 구현하는 방법을 채택했으므로 RawImage 오브젝트를 가져옴, by 황승민, 21/03/24

    public static string path_str_tmp; //경로 + 파일명을 저장하기위한 문자열
    public static string path_str_save; //경로 + 파일명을 저장하기위한 문자열

    [Header("FaceDetectSample 클래스")]
    public FaceDetectionSample fds;
    // Update is called once per frame193
    float ratio;

    void Update()
    {
        if (fds.webcamTexture != null)
        {
            ratio = (float)fds.webcamTexture.width / (float)fds.webcamTexture.height;

            if (fds.cur_ratio == FaceDetectionSample.Aspect_ratio.full_screen)
            {
                zoomOutMin = ratio * 1.47f;
                zoomOutMax = 5.5f;
            }
            else if (fds.cur_ratio == FaceDetectionSample.Aspect_ratio.nine_by_sixteen)
            {
                zoomOutMin = ratio;
                zoomOutMax = 4.5f;
            }
            else
            {
                zoomOutMin = 1f;
                zoomOutMax = 4;
            }
        }
        

        //터치를 통한 줌인 줌아웃 동작인식, by 황승민, 21/03/24
        if (Input.touchCount == 2) //두 개의 입력을 받을 시 작동함, by 황승민, 21/03/24
        {
            Touch touchZero = Input.GetTouch(0);  //첫번째 터치, by 황승민, 21/03/24
            Touch touchOne = Input.GetTouch(1);   //두번째 터치, by 황승민, 21/03/24


            //1,2 터치의 거리값을 통해서 줌인 줌아웃 정도를 결정, by 황승민, 21/03/24
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            //거리값을 매개변수로 Zoom함수를 호출, by 황승민, 21/03/24
           Zoom(difference * 0.01f);
        }
    }

    void Zoom(float increment)
    {
        // 현재 카메라뷰의 Scale x,y 값을 변화시켜줌, by 황승민, 21/03/24
        float x = Mathf.Clamp(Mathf.Abs(CameraView.transform.localScale.x + increment), zoomOutMin, zoomOutMax);
        float y = Mathf.Clamp(Mathf.Abs(CameraView.transform.localScale.y + increment), zoomOutMin, zoomOutMax);
        CameraView.transform.localScale = new Vector3(x, y, 1f);
    }

}