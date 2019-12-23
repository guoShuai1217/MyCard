using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class DebugManager : MonoBehaviour {
    private void Awake()
    {
        //为单例组件赋值
        GameApp.Instance.DebugManagerScript = this;
        //为Unity的Log事件添加一个监听
        if(GameData.Instance.IsDebugWrite)
            Application.logMessageReceived += UnityLogMessage;
    }
	
    #region 对Unity打印函数的封装
    public void Log(object obj) { Debug.Log(obj); }

    public void LogWarning(object obj) { Debug.LogWarning(obj); }

    public void LogError(object obj) { Debug.LogError(obj); }
    #endregion
    //为Unity输出添加一个监听事件
    void UnityLogMessage(string LogMessage,string stack,LogType type) {
        switch (type)
        {
            //如果是普通日志和警告日志，则只添加打印日志时间和日志消息
            case LogType.Log:
            case LogType.Warning:
                LogMessage = DateTime.Now.ToString("hh:mm:ss ") + LogMessage;
                break;
            //如果是错误日志和异常日志，则添加打印时间和异常错误脚本组件的发生位置及调用信息
            default:
                LogMessage = DateTime.Now.ToString("hh:mm:ss ") + LogMessage + " <" + stack + ">";
                break;
        }
        Write(LogMessage, type);
    }
    //声明一个数据流对象
    StreamWriter streamWriter;
    //将输出保存至本地
    void Write(string LogMessage, LogType type) {
        //PC平台上的沙盒路径为C:/Users/Administrator/AppData/LocalLow/NZQP/牛仔棋牌
        //Android的沙盒路径为 /storage/emulated/0/Android/data/com.nzqp.game/files
        //IOS的沙盒路径为     /var/mobile/Containers/Data/Application/app sandbox/Documents
        //获取本机缓存沙盒路径
        string path = Application.persistentDataPath + "/OutLog/" + type;
        //获取程序运行路径
        //Application.dataPath
        //获取资源包资源路径
        //Application.streamingAssetsPath
        //获取资源
        //Resources.Load
        try
        {
            //如果路径不存在，则创建路径  
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            //如果文件存在则加载，如果不存在则创建
            if (streamWriter == null)
                streamWriter = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path);
            //写入本地
            streamWriter.WriteLine(LogMessage);
        }
        //不管上面执行成功失败，均执行下面的代码
        finally
        {
            if (streamWriter != null)
            {
                //释放数据流对象
                streamWriter.Flush();
                streamWriter.Dispose();
                streamWriter = null;
            }
        }
    }


}
