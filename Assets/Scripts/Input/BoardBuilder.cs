using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Excessives.LinqE;

public class BoardBuilder : MonoBehaviour
{
    [Tooltip("The prefab for the board on which all physics simulations and tracking is done")]
    [SerializeField] GameObject boardPrefab;
    [Tooltip("This prefab will track the real board, but also have water bobbing")]
    [SerializeField] GameObject gfxBoardPrefab;
    [Tooltip("The list of input definitions to be used by this board builder")]
    [SerializeField] InputDefinitionList defList;

    [Tooltip("If this is true, then the fake GFX-only board will be instantiated")]
    [SerializeField] bool instantiateGFXBoard = true;
    [Tooltip("If this is false, it will attempt to place the character on the GFX board (if it is instantiated in the first place) instead of the normal board")]
    [SerializeField] bool spawnCharacterInRealBoard = true;

    [Tooltip("Whether to instantiated the board and GFXBoard instances or to assume that boardPrefab and gfxBoardPrefab values represent the in-scene instances")]
    [SerializeField] bool instantiatePrefabs = true;

    void Start()
    {
        StartCoroutine(FindInput());
    }

    // This instantiates all input availability objects and waits to see which ones *survive*,
    // after which it will see which remaining input system has the highest priority and build the player
    // based on that
    IEnumerator FindInput()
    {
        Dictionary<InputSystemDefinition, GameObject> availabilityObjs = new Dictionary<InputSystemDefinition, GameObject>();
        defList.definitions.ForEach(n => availabilityObjs.Add(n, Instantiate(n.available)));

        yield return null;

        InputSystemDefinition def = availabilityObjs
            .Where(n => n.Value != null) // Filter out all unavailable input systems
            ?.Select(n => n.Key) // Select the definitions of all available input systems
            ?.Maximum(n => n.priority); // Select the input definition with the highest priority

        if (def is null) // Look at me using the newfangled 'is null' check :)
        {
            // This will really only ever fire if devs start porting this to console later down the line
            Debug.LogError("Somehow not a single input definition is available, not even keyboard mode!");
            yield break;
        }

        BuildPlayer(def);
    }

    void BuildPlayer(InputSystemDefinition def)
    {
        // Setup character
        GameObject board = instantiatePrefabs ? Instantiate(boardPrefab) : boardPrefab;
        GameObject gfxBoard = instantiatePrefabs ? (instantiateGFXBoard ? Instantiate(gfxBoardPrefab) : null) : gfxBoardPrefab;
        Transform charParent = spawnCharacterInRealBoard ? board.transform : (gfxBoard?.transform ?? board.transform);
        GameObject character = Instantiate(def.character, charParent.position, charParent.rotation, charParent); // Now where to place this character

        using (var e = PlayerReadyEvent.Get()) { }
    }

}
