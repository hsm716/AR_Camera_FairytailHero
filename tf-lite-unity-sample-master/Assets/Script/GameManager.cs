using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VoxelBusters.ReplayKit;

public class GameManager : MonoBehaviour
{


    // 현재 생성중인 오브젝트의 scale값을 저장.
    [HideInInspector]
    public float scaleX;
    [HideInInspector]
    public float scaleY;
    //

    int initString=0;

    string initStr="";

    // 스티커를 클릭할 수 있는 상태 전환 변수
    [HideInInspector]
    public bool isClick;
    // 혹여나 오류로 인해 값이 바뀌지 않더라고, 일정시간 지나면 선택이 가능하게 바꿔주는 역활. 
    bool isTimer;

    // 앱을 처음 구동했을 때, 각종 값들에 대해 초기화 해주는 역활.
    [HideInInspector]
    public int key;

    [Header("현재 AR 오브젝트")]
    public GameObject curARobj;

    [Header("현재 BTN 오브젝트")]
    public GameObject curBtnobj;

    private void Awake()
    {
        isClick = false;
        Application.targetFrameRate = 60;

        // WebCam (DeviceCam) 접근권한 설정
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }


        // 앱 처음 구동시, 
        key = initString + PlayerPrefs.GetInt("init");

        
        for (int i = 0; i < 354; i++)
        {
            initStr += "0";
        }

        // 다운로드 리스트, 즐겨찾기 리스트 모두 0값을 초기화.
        if (key == 0) // Key가 0일 때만 즉, 앱을 다운로드하고 처음 구동했을 때만 값을 초기화 해줌.
        {
            PlayerPrefs.SetInt("init", key + 1);
            PlayerPrefs.SetString("List", initStr);
            PlayerPrefs.SetString("MY", initStr);
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void Update()
    {
        if (isClick == true)
        {
            if (isTimer == false) {
                isTimer = true;
                StartCoroutine(TicTac_timer());
            }
        }
    }
    IEnumerator TicTac_timer()
    {
        yield return new WaitForSeconds(4f);
        isTimer = false;
        isClick = false;
    }
}
