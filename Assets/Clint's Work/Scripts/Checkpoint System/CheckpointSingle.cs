using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    public CheckpointMain track;
    public void SetTrack(CheckpointMain track)
    {
        this.track = track;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger hit by: " + other.name);
        if(other.TryGetComponent<CarController>(out CarController player))
        {
            track.PlayerThroughCheckpoint(this, player);
        }else if(other.TryGetComponent<CarDriverAI>(out CarDriverAI AI))
        {
            track.PlayerThroughCheckpoint(this, AI);
        }
    }
}
