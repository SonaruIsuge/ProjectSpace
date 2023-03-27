using SonaruUtilities;
using UnityEngine.InputSystem;
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


public class PairingData : SceneData
{
    public InputDevice[] PairingPlayers { get; }

    public PairingData(InputDevice[] pairingPlayers)
    {
        PairingPlayers = pairingPlayers;
    }
}


public class ResultData : SceneData
{
    public bool IsWin { get; }
    public float UseTime { get; }

    public ResultData(bool win, float time)
    {
        IsWin = win;
        UseTime = time;
    }
}
