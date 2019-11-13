using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StationCatwalk : Turf
{
    public override void UpdateUpperModel(){
        Debug.Log("oof");
    }

    public override void UpdateLowerModel(){
        Debug.Log("oof");
    }

    public override byte[] GetNetworkData(){
        return new byte[] {};
    }

    public override void SetNetworkData(byte[] inData){}

}