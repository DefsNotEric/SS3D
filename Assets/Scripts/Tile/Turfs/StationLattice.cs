using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
    Turf: Station Lattice Turf
            Most critical/complex turf of the station right now.

    Turf Transistions into the following turfs:
     -  Catwalk

    Turf supports:
     - Adding or removing plating
     - building wall
     - building floor
     - building pipes
     - building wires
 */

public class StationLattice : Turf
{
    public bool allowWires = true;
    public bool allowPipes = true;
    public bool allowWall = true;
    public bool allowFloor = true;

    [SerializeField]
    public bool hasPlating = false;
    [SerializeField]
    public byte hasFloorWall = 0;
    // 0: Lattice has no floor or wall
    // 1: Lattice has a floor.
    // 2: Lattice has a wall.

    public enum StationWallTypes{
        none,
        girder,
        plated,
        plated_reinforced,
        regular,
        reinforced,
        glass,
        glass_reinforced
        };

    [SerializeField]
    private StationWallTypes stationWallType = StationWallTypes.regular;
    
    public override void UpdateUpperModel(){
        if (upperTurf != null)
        {
            #if UNITY_EDITOR
            DestroyImmediate(upperTurf);
            #else
            Destroy(upperTurf);
            #endif
        }

        switch(hasFloorWall)
        {
            case 0:
                //no upper turf
                break;
            case 1:
                UpdateFloorModel();
                break; 
            case 2:
                UpdateWallModel();
                break;
            default:
                Debug.LogError("Uknown hasFloorWall");
                break;
        }
    }

    public override void UpdateLowerModel(){
        if (lowerTurf != null)
        {
            #if UNITY_EDITOR
            DestroyImmediate(lowerTurf);
            #else
            Destroy(lowerTurf);
            #endif
        }
        
        GameObject buildTarget;
        if(hasPlating)
        {
            buildTarget = Resources.Load("Turf/Lower/station_lattice/lower_plated") as GameObject;
        }
        else
        {
            buildTarget = Resources.Load("Turf/Lower/station_lattice/lower") as GameObject;
        }
        //Debug.Log(buildTarget);
        base.lowerTurf = Instantiate(buildTarget, transform.position, Quaternion.identity, transform) as GameObject;
        base.lowerTurf.name = "lowerTurf";  
    }

    public override byte[] GetNetworkData(){
        //Description of Lattice Network Data Packet:
        //[0]bool-byte:
        //  [0] allowWires
        //  [1] allowPipes
        //  [2] allowWall
        //  [3] allowFloor
        //  [4] hasPlating
        //  [5] connectiveUpper; //adaptive models yes/no like walls
        //  [6] connectiveLower;

        //[1] hasFloorWall Byte
        //[2] StationWallTypeByte

        byte[] outData = new byte[3];
        outData[0] = 0;
        outData[0] ^= (byte) (allowWires ? 0b00000001 : 0b0000000);
        outData[0] ^= (byte) (allowPipes ? 0b00000010 : 0b0000000);
        outData[0] ^= (byte) (allowWall ? 0b00000100 : 0b0000000);
        outData[0] ^= (byte) (allowFloor ? 0b00001000 : 0b0000000);
        outData[0] ^= (byte) (hasPlating ? 0b00010000 : 0b0000000);
        outData[0] ^= (byte) (connectiveUpper ? 0b00100000 : 0b0000000);
        outData[0] ^= (byte) (connectiveLower ? 0b01000000 : 0b0000000);

        outData[1] = hasFloorWall;
        outData[2] = (byte) stationWallType;

        return outData;
    }

    public override void SetNetworkData(byte[] inData){
        //Description of Lattice Network Data Packet:
        //[0]bool-byte:
        //  [0] allowWires
        //  [1] allowPipes
        //  [2] allowWall
        //  [3] allowFloor
        //  [4] hasPlating
        //  [5] connectiveUpper = false; //adaptive models yes/no like walls
        //  [6] connectiveLower = false;

        //[1] hasFloorWall Byte
        //[2] StationWallTypeByte

        allowWires = (inData[0] & 0b00000001) == 0b00000001;
        allowPipes = (inData[0] & 0b00000010) == 0b00000010;
        allowWall = (inData[0] & 0b00000100) == 0b00000100;
        allowFloor = (inData[0] & 0b00001000) == 0b00001000;
        hasPlating = (inData[0] & 0b00010000) == 0b00010000;
        connectiveUpper = (inData[0] & 0b00100000) == 0b00100000;
        connectiveLower = (inData[0] & 0b01000000) == 0b01000000;

        hasFloorWall = inData[1];
        stationWallType = (StationWallTypes) inData[2];

        base.UpdateTurf();
        base.UpdateNeighbourTurfs();
    }

    public void AddWallGirder(){
        if(allowWall == true){
            allowFloor = false;
            hasFloorWall = 2;
            stationWallType = StationWallTypes.girder;
            base.UpdateTurf();
        }else{
            Debug.Log("Wall building not allowed");
        }
    }

    public void RemoveWallGirder(){
        allowFloor = true;
        hasFloorWall = 0;
        stationWallType = StationWallTypes.none;
        base.connectiveUpper = false;
        base.UpdateTurf();
    }

    public void AddFloor(){
        if(allowFloor == true){
            allowWall = false;
            hasFloorWall = 1;
            base.UpdateTurf();
        }else{
            Debug.Log("Floor building not allowed");
        }
    }

    public void RemoveFloor(){
        allowWall = true;
        hasFloorWall = 0;
        base.UpdateTurf();
    }


    public void AddPlating(){
        hasPlating = true;
        base.UpdateTurf();
    }

    public void RemovePlating(){
        hasPlating = false;
        base.UpdateTurf();
    }

    public void UpgradeWall(StationWallTypes newWallType){
        if(stationWallType == StationWallTypes.girder){
            stationWallType = newWallType;
            base.connectiveUpper = true;
            base.UpdateTurf();
            base.UpdateNeighbourTurfs();
        }else{
            Debug.LogError("Station wall already upgraded");
        }
    }

    public void DowngradeWall(){
        if(stationWallType != StationWallTypes.girder){
            stationWallType = StationWallTypes.girder;
            base.connectiveUpper = false;
            base.UpdateTurf();
            base.UpdateNeighbourTurfs();
        }else{
            Debug.LogError("Station wall already downgraded");
        }
    }


    private void UpdateWallModel(){

        string modelName;
        switch(stationWallType)
        {
            case StationWallTypes.none:
                //should not happen
                Debug.LogError("no walltype selected but wall is present");
                modelName = "station_wall";
                base.connectiveUpper = false;
                break;
            case StationWallTypes.girder:
                modelName = "station_wall_girder";
                base.connectiveUpper = false;
                break;
            case StationWallTypes.plated:
                modelName = "station_wall_plated";
                base.connectiveUpper = false;
                break;
            case StationWallTypes.plated_reinforced:
                modelName = "station_wall_plated_reinforced";
                base.connectiveUpper = false;
                break;
            case StationWallTypes.regular:
                modelName = "station_wall";
                base.connectiveUpper = true;
                break;
            case StationWallTypes.reinforced:
                modelName = "station_wall_reinforced";
                base.connectiveUpper = true;
                break;
            case StationWallTypes.glass:
                modelName = "station_wall_glass";
                base.connectiveUpper = true;
                break;
            case StationWallTypes.glass_reinforced:
                modelName = "station_wall_glass_reinforced";
                base.connectiveUpper = true;
                break;
            default:
                //should not happen
                Debug.LogError("walltype does not exist selected but wall is present");
                modelName = "station_wall";
                break;
        }

        if(!base.connectiveUpper){
            upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper"), transform.position, Quaternion.identity, transform) as GameObject;
            upperTurf.name = "upperTurf";
        }else{
            switch(connectiveUpperNESW & 0x0f){
                case 0: //None?
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                case 1: //W
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_N"), transform.position, Quaternion.Euler(0,-90,0), transform) as GameObject;
                    break;
                case 2: //S
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_N"), transform.position, Quaternion.Euler(0,180,0), transform) as GameObject;
                    break;
                case 3: //SW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NE"), transform.position, Quaternion.Euler(0,180,0), transform) as GameObject;
                    break;
                case 4: //E
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_N"), transform.position, Quaternion.Euler(0,90,0), transform) as GameObject;
                    break;
                case 5: //EW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NS"), transform.position, Quaternion.Euler(0,90,0), transform) as GameObject;
                    break;
                case 6: //ES
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NE"), transform.position, Quaternion.Euler(0,90,0), transform) as GameObject;
                    break;
                case 7: //ESW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NES"), transform.position, Quaternion.Euler(0,90,0), transform) as GameObject;
                    break;
                case 8: //N
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_N"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                case 9: //NW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NE"), transform.position, Quaternion.Euler(0,-90,0), transform) as GameObject;
                    break;
                case 10: //NS
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NS"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                case 11: //NSW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NES"), transform.position, Quaternion.Euler(0,180,0), transform) as GameObject;
                    break;
                case 12: //NE
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NE"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                case 13: //NEW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NES"), transform.position, Quaternion.Euler(0,-90,0), transform) as GameObject;
                    break;
                case 14: //NES
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NES"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                case 15: //NESW
                    upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper_NESW"), transform.position, Quaternion.Euler(0,0,0), transform) as GameObject;
                    break;
                default:
                    Debug.Log(connectiveUpperNESW);
                    //this should not happen
                    break;
            }
            upperTurf.name = "upperTurf";
        }
    }
    private void UpdateFloorModel(){
        string modelName;
        modelName = "station_floor";
        if(!base.connectiveUpper){
                upperTurf = Instantiate(Resources.Load("Turf/Upper/"+modelName+"/upper"), transform.position, Quaternion.identity, transform) as GameObject;
                upperTurf.name = "upperTurf";
        }
    }

    private void OnDestroy() {
        if (upperTurf != null)
        {
            #if UNITY_EDITOR
            DestroyImmediate(upperTurf);
            #else
            Destroy(upperTurf);
            #endif
        }
        if (lowerTurf != null)
        {
            #if UNITY_EDITOR
            DestroyImmediate(lowerTurf);
            #else
            Destroy(lowerTurf);
            #endif
        }
    }

}