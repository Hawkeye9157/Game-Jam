using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private CheckpointMain track;
    public void SetTrack(CheckpointMain track)
    {
        this.track = track;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<CarController>(out CarController player))
        {
            track.PlayerThroughCheckpoint(this, player);
        }else if(other.TryGetComponent<CarDriverAI>(out CarDriverAI AI))
        {
            track.PlayerThroughCheckpoint(this, AI);
        }
    }
}
