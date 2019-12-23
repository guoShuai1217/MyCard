using UnityEngine;
using System.Collections;
using GameProtocol;

public class GameOther : MonoBehaviour {
    GameObject OtherPanel;
    GameObject UIPanel;
    private void Awake()
    {
        GameApp.Instance.GameOtherScript = this;   
    }
    void Start () {
        //获取父物体
        Transform cardParent = transform.Find("CardPanel");
        Transform uiParent = transform.Find("UIPanel/gameinfo");
        //根据游戏类型选择待生成的桌面和UI
        switch (GameSession.Instance.RoomType)
        {
            case GameProtocol.SConst.GameType.WINTHREEPOKER:
                //生成桌面
                string path = GameResource.UIResourcePath + GameData.Instance.SystemUI[GameResource.SystemUIType.CARDOTHER_TP];
                OtherPanel = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, cardParent, Vector3.zero);
                //生成UI
                path = GameResource.UIResourcePath + GameData.Instance.SystemUI[GameResource.SystemUIType.UIFIGHT_TP];
                UIPanel = GameApp.Instance.ResourcesManagerScript.LoadInstantiateGameObject(path, uiParent, Vector3.zero);
                //添加脚本
                OtherPanel.gameObject.AddComponent<TPCardOther>();
                uiParent.gameObject.AddComponent<TPUI_Fight>().SetGameInfoPanel(UIPanel);
                UIPanel.AddComponent<UI_Head>();
                break;
            case GameProtocol.SConst.GameType.XZDD:

                break;
        }
    }
    /// <summary>
    /// 游戏结束，将UI删除
    /// </summary>
    public void GameOver() {
        Destroy(OtherPanel);
        Destroy(UIPanel);
    }
}
