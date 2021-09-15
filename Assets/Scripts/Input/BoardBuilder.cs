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
        GameObject board = instantiatePrefabs ? Instantiate(boardPrefab) : boardPrefab;
        GameObject gfxBoard = instantiatePrefabs ? (instantiateGFXBoard ? Instantiate(gfxBoardPrefab) : null) : gfxBoardPrefab;
        Transform charParent = spawnCharacterInRealBoard ? board.transform : (gfxBoard?.transform ?? board.transform);

        StartCoroutine(Watch(charParent));
    }


    // This coroutine quietly and continuously watches all input systems, waiting for when a new higher priority
    // input system is detected to pounce!
    // After which it will utterly destroy the previous input system character system deposing it and stripping it of
    // its right to rule the board, after which it will then set the newer higher priority input system character upon
    // the throne until it also inevitibly is unplugged, after which this coroutine will then return authority back
    // to the input system with the next-highest claim to the throne.
    // And thus - the cycle continues.
    IEnumerator Watch(Transform charParent)
    {
        Dictionary<InputSystemDefinition, InputAvailability> availabilityObjs = new Dictionary<InputSystemDefinition, InputAvailability>();
        defList.definitions.ForEach(n => availabilityObjs.Add(n, Instantiate(n.available)?.GetComponent<InputAvailability>()));
        InputSystemDefinition prevBest = null;
        GameObject character = null;

        while (true)
        {
            yield return null;         // Placing this line right here let's us both wait between each check, and wait one frame after setting up all input systems
            // C# Linq is very lazily evaluated, the below does not instantiate a table for each method, as iterators are used
            InputSystemDefinition best = availabilityObjs // How's this for self-documenting code - 'best' :)
                .Where(n => !(n.Value is null) && n.Value.Available)
                .Select(n => n.Key)
                .Maximum(n => n.priority);

            if (best is null)
                continue;

            // If the best input system has changed
            if (best != prevBest)
            {
                // Destroy the current character and replace it with the input-system appropriate one
                prevBest = best;
                Destroy(character);
                character = Instantiate(best.character, charParent.position, charParent.rotation, charParent);
                using (var e = PlayerReadyEvent.Get()) { }
            }
        }
    }
}
