using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Test and implement into the game

// This class is unfinished and untested, however this class is supopsed to
// manage recording the player state repeatedly every few frames then replay
// the player's movements back to them highlighting their errors and should
// give helpful input regarding how they can improve.

public class InstantReplay : MonoBehaviour
{

    [Tooltip("How many times per second a snapshot will be taken, this will cap out at the total physics ticks per second")]
    [SerializeField]
    int snapshotsPerSecond = 10;

    int SnapshotsPerSecond => Mathf.Clamp(snapshotsPerSecond, 1, (int)(1.0f / Time.fixedDeltaTime));

    // TODO: This should probably account for some currently unexplored edge cases
    bool ShouldRecord => BoardController.Instance.InputAccepted;

    // Set these to anything
    // TODO: Clean this up to be a little more 'proper'
    Transform fakeHead;
    Transform fakeBoard;

    #region Bookkeeping

    // Using linked list as we can quite rapidly append to it without needing a large occasional realocation
    // and we'll only be reading this data sequentially
    LinkedList<InstantReplayDat> replay = new LinkedList<InstantReplayDat>();

    float snapClock = 0.0f;

    float snapDelta = 1.0f;

    Vector2 headPos;

    #endregion

    void OnEnable() => VisualControlEvent.RegisterListener(OnVisualControlEvent);
    void OnDisable() => VisualControlEvent.UnregisterListener(OnVisualControlEvent);

    void Start()
    {
        // The reason I'm caching this value is that it is *paramount* that it not be modified in the inspector during runtime
        snapDelta = 1.0f / SnapshotsPerSecond;
    }

    void FixedUpdate()
    {

        snapClock += Time.fixedDeltaTime;

        if (snapClock >= snapDelta)
        {
            snapClock -= snapDelta;
            // Take a screenshot
            replay.AddLast(Snapshot());
        }
    }

    InstantReplayDat Snapshot() => new InstantReplayDat
    {
        time = Time.timeSinceLevelLoad,
        headPos = this.headPos,
        boardPos = BoardController.Instance.Motor.TransientPosition,
        boardRot = BoardController.Instance.Motor.TransientRotation,
    };

    void OnVisualControlEvent(VisualControlEvent e) => headPos = e.dir;

    // I will not do this using recursive coroutines, as much as I am tempted from the linked list iteration
    IEnumerator Replay()
    {
        float initTime = Time.timeSinceLevelLoad;
        float animInitTime = replay.First.Value.time;

        LinkedListNode<InstantReplayDat> current = replay.First;

        while (current != null)
        {
            float time = Time.timeSinceLevelLoad - initTime;
            float animTime = current.Value.time - animInitTime;

            // Move to the next snapshot
            if (time >= animTime)
                current = current.Next;

            InstantReplayDat snap = current.Value;

            // How much time is left for this snapshot
            float snapTimeLeft = animTime - time;

            // Move head/board to their new position/rotation over time
            float boardPosVel = (snap.boardPos - fakeBoard.position).magnitude / snapTimeLeft;
            float boardRotVel = Quaternion.Angle(fakeBoard.rotation, fakeBoard.rotation) / snapTimeLeft;
            float headPosVel = (snap.headPos - fakeHead.position).magnitude / snapTimeLeft;

            // Move everything towards their correct position/rotation using the appropriate velocities
            fakeBoard.position = Vector3.MoveTowards(fakeBoard.position, snap.boardPos, Time.deltaTime * boardPosVel);
            fakeBoard.rotation = Quaternion.RotateTowards(fakeBoard.rotation, snap.boardRot, Time.deltaTime * boardRotVel);
            fakeHead.position = Vector3.MoveTowards(fakeHead.position, snap.headPos, Time.deltaTime * headPosVel);

            yield return null;
        }
    }
}

// TODO: Decide if this would better be a struct
public class InstantReplayDat
{
    public float time; // Game time when snapshot is taken, helps position/rotation interpolation
    public Vector3 headPos;
    public Vector3 boardPos;
    public Quaternion boardRot; // Probably don't need a whole quat for this
}