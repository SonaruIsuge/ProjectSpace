using SonaruUtilities;
using UnityEngine.SceneManagement;


public class GameFlowManager : TSingletonMonoBehaviour<GameFlowManager>
{
    public SceneData SceneData { get; private set; }


    public void LoadScene(int sceneIndex, SceneData data)
    {
        SceneData = data;
        SceneManager.LoadScene(sceneIndex);
    }
}


public abstract class SceneData {}


public class ResultData : SceneData
{
    public bool IsWin { get; private set; }
    public float UseTime { get; private set; }

    public ResultData(bool win, float time)
    {
        IsWin = win;
        UseTime = time;
    }
}
