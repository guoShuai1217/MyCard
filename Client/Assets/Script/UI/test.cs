using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class test : MonoBehaviour {
    int idx = 0;
	// Use this for initialization
	void Start () {
        //测试代码
        transform.Find("ButtonAdd").GetComponent<Button>().onClick.AddListener(delegate () {
            idx++;
            transform.Find("Text").GetComponent<Text>().text = idx + "了";
        });
        transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(delegate () {
            GameApp.Instance.GameLevelManagerScript.CloseSystemUI(GameResource.SystemUIType.UIHINTLOGPANEL,delegate() {
                Debug.Log("关闭");
            });
        });
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
