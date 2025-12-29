using UnityEngine;
using Zombie3D;

public class NetworkTransformInterpolation : MonoBehaviour
{
    public enum InterpolationMode
    {
        INTERPOLATION = 0,
        EXTRAPOLATION = 1
    }

    public InterpolationMode mode;

    private double interpolationBackTime = 200.0;

    private float extrapolationForwardTime = 1000f;

    private bool running;

    private NetworkTransform[] bufferedStates = new NetworkTransform[20];

    private NetworkTransform receivedTransform;

    private int statesCount;

    public void StartReceiving()
    {
        running = true;
    }

    public void StopReceiving()
    {
        running = false;
    }

    public void ReceivedTransform(NetworkTransform ntransform)
    {
        if (running && TNetConnection.Connection != null && (receivedTransform == null || !(receivedTransform.TimeStamp >= ntransform.TimeStamp)))
        {
            receivedTransform = ntransform;
            (GetComponent<PlayerShell>().m_player as Multiplayer).UpdateMultiTransform(ntransform.Direction, ntransform.AngleRotation, ntransform.Position, (float)(TNetConnection.Connection.TimeManager.AveragePing / 1000.0 / 2.0));
        }
    }

    public void _ReceivedTransform(NetworkTransform ntransform)
    {
        if (!running || (bufferedStates[0] != null && ntransform.TimeStamp < bufferedStates[0].TimeStamp))
        {
            return;
        }
        for (int num = bufferedStates.Length - 1; num >= 1; num--)
        {
            bufferedStates[num] = bufferedStates[num - 1];
        }
        bufferedStates[0] = ntransform;
        statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);
        for (int i = 0; i < statesCount - 1; i++)
        {
            if (bufferedStates[i].TimeStamp < bufferedStates[i + 1].TimeStamp)
            {
                Debug.Log("State inconsistent");
            }
        }
    }

    private void UpdateValues()
    {
        if (TNetConnection.Connection != null)
        {
            double averagePing = TNetConnection.Connection.TimeManager.AveragePing;
            if (averagePing < 50.0)
            {
                interpolationBackTime = 50.0;
            }
            else if (averagePing < 100.0)
            {
                interpolationBackTime = 100.0;
            }
            else if (averagePing < 200.0)
            {
                interpolationBackTime = 200.0;
            }
            else if (averagePing < 400.0)
            {
                interpolationBackTime = 400.0;
            }
            else if (averagePing < 600.0)
            {
                interpolationBackTime = 600.0;
            }
            else
            {
                interpolationBackTime = 1000.0;
            }
            interpolationBackTime += 300.0;
        }
    }
}