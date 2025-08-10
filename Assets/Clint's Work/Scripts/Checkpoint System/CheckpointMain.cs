
using System.Collections.Generic;
using UnityEngine;

public class CheckpointMain : MonoBehaviour
{
    [SerializeField] private Transform checkpointsParent;
    private List<CheckpointSingle> checkpointList;
    private Dictionary<Transform, int> playerCheckpointIndex;
    public CarController player;
    public CarDriverAI AI1;
    public CarDriverAI AI2;
    public CarDriverAI AI3;

    private void Start()
    {
        checkpointList = new List<CheckpointSingle>();
        playerCheckpointIndex = new Dictionary<Transform, int>();

        //init checkpoints
        foreach(Transform checkpointTransform in checkpointsParent)
        {
            CheckpointSingle checkpoint = checkpointTransform.GetComponent<CheckpointSingle>();
            checkpoint.SetTrack(this);
            checkpointList.Add(checkpoint);
        }

        foreach(CarController player in FindObjectsByType<CarController>(sortMode:FindObjectsSortMode.None))
        {
            playerCheckpointIndex[player.transform] = 0;
        }
        foreach(CarDriverAI AI in FindObjectsByType<CarDriverAI>(sortMode: FindObjectsSortMode.None))
        {
            playerCheckpointIndex[AI.transform] = 0;
        }
    }
    
    public void PlayerThroughCheckpoint(CheckpointSingle checkpoint,CarController player)
    {
        int currentIndex = checkpointList.IndexOf(checkpoint);
        if(player.TryGetComponent<CarController>(out CarController controller))
        {
            int count = checkpointList.IndexOf(checkpoint);
            if(count == 0 && player.checkpointCount != 0)
            {
                player.lapCount++;
            }
            player.checkpointCount = count;
        }
    }
    public void PlayerThroughCheckpoint(CheckpointSingle checkpoint,CarDriverAI ai)
    {
        if (ai.TryGetComponent<CarDriverAI>(out CarDriverAI controllerAI))
        {
            int count = checkpointList.IndexOf(checkpoint);
            if(count == 0 && ai.checkpointCount != 0)
            {
                ai.lapCount++;
            }
            ai.checkpointCount = count;
        }
    }

    public int GetPlayerPosition()
    {
        int ahead = 0;
        if(player.lapCount < AI1.lapCount)
        {
            ahead++;
        }
        else
        {
            if(player.checkpointCount < AI1.checkpointCount)
            {
                ahead++;
            }
        }
        if (player.lapCount < AI2.lapCount)
        {
            ahead++;
        }
        else
        {
            if (player.checkpointCount < AI2.checkpointCount)
            {
                ahead++;
            }
        }
        if (player.lapCount < AI3.lapCount)
        {
            ahead++;
        }
        else
        {
            if (player.checkpointCount < AI3.checkpointCount)
            {
                ahead++;
            }
        }
        int placement = ahead + 1;
        return placement;
    }
    

}
