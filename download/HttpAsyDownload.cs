 using System;
 using System.Net;
 using System.Threading;
 using UnityEngine;

/// <summary>
///   Http下载类 
/// </summary>
public class HttpAsyDownload
{
  public enum emErrorCode
  {
      /// <summary>
      /// 未知错误
      /// </summary>
      None,
      /// <summary>
      /// 取消下载
      /// </summary>
      Cancel,
      /// <summary>
      /// 没有响应
      /// </summary>
      NoResponse,
      /// <summary>
      /// 下载出错
      /// </summary>
      DownLoadError,
      /// <summary>
      /// 请求超时
      /// </summary>
      TimeOut,
      /// <summary>
      /// 强制关闭
      /// </summary>
      Abort,
  }

    public const int TIMEOUT_TIME = 20000;
    public string URL { get; private set; }
    public string Root { get; private set; }
    public string LocalName { get; private set; }

    public string FullName
    {
        get
        {
            if (string.IsNullOrEmpty(Root) && string.IsNullOrEmpty(LocalName))
            {
                return Root + "/" + LocalName;
            }
            return null;
        }
    }

    public bool IsDone { get; private set; }
    public emErrorCode ErrorCode;
    /// <summary>
    /// 下载文件的总大小 总长度
    /// </summary>
    public long Length { get; private set; }

    public long CompletedLength { get; private set; }
    /// <summary>
    /// 下载通知回调函数
    /// </summary>
    private Action<HttpAsyDownload, long> notify_callback;
    /// <summary>
    /// 下载出错的回调函数
    /// </summary>
    private Action<HttpAsyDownload> error_callback;
    /// <summary>
    /// 下载内容的实例 用来获得文件的最后修改时间等信息
    /// </summary>
    private DownloadContent Content = null;
    /// <summary>
    /// 保证线程安全的锁对象
    /// </summary>
    private object lock_object = null;
    /// <summary>
    /// wen请求对象实例
    /// </summary>
    private HttpWebRequest _httpWebRequest = null;

    public HttpAsyDownload(string url)
    {
        URL = url;
    }
    /// <summary>
    /// 提供外部调用下载开始接口函数
    /// </summary>
    public void Start(string root,string localname,Action<HttpAsyDownload,long> notify = null,Action<HttpAsyDownload> error = null)
    {
        lock (lock_object)
        {
            //TODO 不知道为什么要先终止下载
            //下载之前先终止
            Root = root;
            LocalName = localname;
            if (notify != null)
            {
                notify_callback = notify;
            }
            if (error != null)
            {
                error_callback = error;
            }
            //设置各种属性的初始值
            IsDone = false;
            ErrorCode = emErrorCode.None;
            Content = new DownloadContent(FullName);
            CompletedLength = 0;
            Length = 0;
            //开始下载
        }
  }
    
}