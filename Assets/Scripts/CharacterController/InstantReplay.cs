using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// TODO: Test and implement into the game

// This class is unfinished and untested, however this class is supopsed to
// manage recording the player state repeatedly every few frames then replay
// the player's movements back to them highlighting their errors and should
// give helpful input regarding how they can improve.

public class InstantReplay : MonoBehaviour
{
    public static InstantReplay Instance { get; private set; }

    [Tooltip("How many times per second a snapshot will be taken, this will cap out at the total physics ticks per second")]
    [SerializeField]
    int snapshotsPerSecond = 10;

    int SnapshotsPerSecond => Mathf.Clamp(snapshotsPerSecond, 1, (int)(1.0f / Time.fixedDeltaTime));

    // TODO: This should probably account for some currently unexplored edge cases
    // bool ShouldRecord => BoardController.Instance.InputAccepted;

    // Set these to anything
    // TODO: Clean this up to be a little more 'proper'
    [SerializeField] Transform fakeHead;
    [SerializeField] Transform fakeBoard;

    [SerializeField] RenderTexture replayTex;

    #region Bookkeeping

    // Using linked list as we can quite rapidly append to it without needing a large occasional realocation
    // and we'll only be reading this data sequentially
    LinkedList<InstantReplayDat> replay = new LinkedList<InstantReplayDat>();

    float snapClock = 0.0f;

    float snapDelta = 1.0f;

    Vector2 headPos;

    bool recording = false;

    #endregion

    void OnEnable()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        VisualControlEvent.RegisterListener(OnVisualControlEvent);
        WaveEndEvent.RegisterListener(OnWaveEndEvent);
        GameLost.RegisterListener(OnGameLost);
        VRButtonEvent.RegisterListener(OnVRButtonEvent);
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
            // These will only be registered to, if we were the singleton instance
            VisualControlEvent.UnregisterListener(OnVisualControlEvent);
            WaveEndEvent.UnregisterListener(OnWaveEndEvent);
            GameLost.UnregisterListener(OnGameLost);
            VRButtonEvent.UnregisterListener(OnVRButtonEvent);
        }
    }

    void Start()
    {
        // The reason I'm caching this value is that it is *paramount* that it not be modified in the inspector during runtime
        snapDelta = 1.0f / SnapshotsPerSecond;
        fakeBoard.gameObject.SetActive(false);
        fakeHead.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!recording)
            return;

        snapClock += Time.fixedDeltaTime;

        if (snapClock >= snapDelta)
        {
            snapClock -= snapDelta;
            // Take a screenshot
            replay.AddLast(Snapshot());
        }
    }

    public static void StartRecord() => Instance.recording = true;
    public static void StopRecord() => Instance.recording = false;
    [Button]
    public static void StartReplay() => Instance.StartCoroutine(Instance.Replay());

    InstantReplayDat Snapshot() => new InstantReplayDat
    {
        time = Time.timeSinceLevelLoad,
        headPos = this.headPos,
        boardPos = BoardController.Instance.Motor.TransientPosition,
        boardRot = BoardController.Instance.Motor.TransientRotation,
    };

    void OnVisualControlEvent(VisualControlEvent e) => headPos = e.dir;

    void OnWaveEndEvent(WaveEndEvent e) => InstantReplay.StopRecord();
    void OnGameLost(GameLost e) => InstantReplay.StopRecord();

    void OnVRButtonEvent(VRButtonEvent e) => InstantReplay.StartReplay();

    // I will not do this using recursive coroutines, as much as I am tempted from the linked list iteration
    IEnumerator Replay()
    {
        fakeBoard.gameObject.SetActive(true);
        fakeHead.gameObject.SetActive(true);

        float initTime = Time.timeSinceLevelLoad;
        float animInitTime = replay.First.Value.time;

        LinkedListNode<InstantReplayDat> current = replay.First;

        while (current != null)
        {
            if (current?.Next is null)
                break;

            float time = Time.timeSinceLevelLoad - initTime;
            float animTime = current.Value.time - animInitTime;

            // Move to the next snapshot
            if (time >= animTime)
                current = current.Next;

            InstantReplayDat snap = current.Value;

            // How much time is left for this snapshot
            float snapTimeLeft = animTime - time;

            // Move head/board to their new position/rotation over time
            // float boardPosVel = (snap.boardPos - fakeBoard.position).magnitude / snapTimeLeft;
            // float boardRotVel = Quaternion.Angle(fakeBoard.rotation, fakeBoard.rotation) / snapTimeLeft;
            // float headPosVel = (snap.headPos - fakeHead.localPosition).magnitude / snapTimeLeft;

            // // Move everything towards their correct position/rotation using the appropriate velocities
            // fakeBoard.position = Vector3.MoveTowards(fakeBoard.position, snap.boardPos, Time.deltaTime * boardPosVel);
            // fakeBoard.rotation = Quaternion.RotateTowards(fakeBoard.rotation, snap.boardRot, Time.deltaTime * boardRotVel);
            // fakeHead.localPosition = Vector3.MoveTowards(fakeHead.localPosition, snap.headPos, Time.deltaTime * headPosVel);

            fakeBoard.position = snap.boardPos;
            fakeBoard.rotation = snap.boardRot;
            fakeHead.localPosition = snap.headPos;


            yield return null;
        }

        fakeBoard.gameObject.SetActive(false);
        fakeHead.gameObject.SetActive(false);
        replayTex.DiscardContents();
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