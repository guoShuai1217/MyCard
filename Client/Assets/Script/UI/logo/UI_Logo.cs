using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Logo : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ////测试代码
        //Debug.Log("UI_Logo Start");
        //GameApp.Instance.TimeManagerScript.AddSchedule(delegate () { Debug.Log("测试成功"); }, 5000);
        //transform.FindChild("Button").GetComponent<Button>().onClick.AddListener(delegate () {
        //    Debug.Log("Button On Click");
        //    GameApp.Instance.GameLevelManagerScript.LoadSystemUI(GameResource.SystemUIType.UIHINTLOGPANEL,
        //        delegate() {
        //            GameObject go;
        //            if (GameApp.Instance.GameLevelManagerScript.SystemUICache.TryGetValue(GameResource.SystemUIType.UIHINTLOGPANEL, out go))
        //            {
        //                if (!go.GetComponent<test>())
        //                    go.AddComponent<test>();
        //            }
        //        });
        //});
        //给LOGO场景添加一个时间上的缓冲，1秒后执行场景加载
        GameApp.Instance.TimeManagerScript.AddSchedule(delegate ()
        {
            GameApp.Instance.GameLevelManagerScript.LoadScene(GameResource.SceneName.LOGIN);
        }, 1000);
    }
	


	// Update is called once per frame
	void Update () {
	
	}
}
