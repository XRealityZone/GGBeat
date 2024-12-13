using UnityEngine;

namespace GGBeat
{
  public enum SceneType
  {
    Cyberpunk = 0,
    VanGogh = 1,
    Cartoon = 2
  }

  public enum SongType
  {
    Cyberpunk2077 = 0,
    MissingU = 1,
    SaintPerros = 2
  }

  /// <summary>
  /// 控制整个应用的界面切换、native 交互等
  /// </summary>
  public class AppManager
  {
    [HideInInspector]
    public bool isMainMenuOpen = false;

    [HideInInspector]
    public bool isImmersiveSpaceOpen = false;

    [HideInInspector]
    public bool isSplashOpened = false;

    public SongType songType = SongType.Cyberpunk2077;

    public SceneType sceneType = SceneType.Cyberpunk;

    private static AppManager _instance;

    public static AppManager Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new AppManager();
          SwiftUIPlugin.SetSwiftNativeCallback(SwiftUIPlugin.OnSwiftNativeCallback);
        }
        return _instance;
      }
    }

    private void OpenNativeWindow(string name)
    {
      SwiftUIPlugin.OpenNativeWindow(name);
    }

    private void CloseNativeWindow(string name)
    {
      SwiftUIPlugin.CloseNativeWindow(name);
    }

    public void OpenMainMenu()
    {
      isMainMenuOpen = true;
      OpenNativeWindow("Main");
    }

    public void CloseMainMenu()
    {
      isMainMenuOpen = false;
      CloseNativeWindow("Main");
    }

    public void OpenPlayground(OpenPlaygroundArgs args)
    {
      switch (args.scene)
      {
        case "Cyberpunk":
          sceneType = SceneType.Cyberpunk;
          break;
        case "VanGogh":
          sceneType = SceneType.VanGogh;
          break;
        case "Cartoon":
          sceneType = SceneType.Cartoon;
          break;
        default:
          sceneType = SceneType.Cyberpunk;
          break;
      }
      switch (args.song)
      {
        case "2077":
          songType = SongType.Cyberpunk2077;
          break;
        case "Missing U":
          songType = SongType.MissingU;
          break;
        case "SaintPerros":
          songType = SongType.SaintPerros;
          break;
        default:
          songType = SongType.Cyberpunk2077;
          break;
      }
      Instance.isImmersiveSpaceOpen = true;
      UnityEngine.SceneManagement.SceneManager.LoadScene("1_RK_Playground");
    }

    public void ClosePlayground()
    {
      Instance.isImmersiveSpaceOpen = false;
      UnityEngine.SceneManagement.SceneManager.LoadScene("0_Root");
    }
  }

  public class OpenPlaygroundArgs
  {
    public string song;
    public string scene;
  }
}
