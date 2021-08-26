using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControlEvent : GlobalEvent<InteractionControlEvent>, System.IDisposable 
{
    public Interaction input;

    public Interactables inputInteractable;
}
// This is a simple structure that holds the input data itself.
// This is used so that each input receptor or listener can also hold the input
// data locally without needing more than one assignment.
public struct Interaction {
    public bool isInteracting;
}