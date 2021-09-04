using UnityEngine;
using UnityEngine.UI;

public class FileSelected : MonoBehaviour
{

    // 토글 상태에 따른 (카테고리txt) color 변화
    #region Methods
    private void Update()
    {
        if (transform.parent.GetComponent<Toggle>().isOn)
        {
            GetComponent<Text>().color = new Color(1f,1f,1f,1f);
        }
        else
        {
            GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.6f);
        }
    }
    #endregion
}