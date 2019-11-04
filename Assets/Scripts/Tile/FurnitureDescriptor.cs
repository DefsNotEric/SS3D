﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureDescriptor : MonoBehaviour
{
/**
    Component to be added to every furniture prefab to hold information
    Most of this class is a placeholder for now (04 November 2019)






 */
    // Compontent that should be attached to every furniture prefab. It holds flags of what the furniture type is, where it can go, etc.
    public enum FurnitureType{
        wall_furniture,
        floor_furniture,
        pipe_furniture,
        wire_furniture,
        door_furniture
    }

    //Boolean value to tell the manager that this furniture 'can' be multi-tile (like tables)
    public bool connectable;

    public FurnitureType furnitureType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
