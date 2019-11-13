using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Component of tiles that deals with the Turf
     
    The turf is the 'physical' tile. Meaning the lattice/floor/wall/plenum.

    A Turf exists of an upper-turf (Main tiled floor / Wall )
    and Lower-turf (Lattice/plenum )

    This is an abstract class and is used to create the actual 'Turf-Classes'
    abstract methods required to implement:
    UpdateLowerModel()  ->  Update lowerTurf GameObject
    UpdateUpperModel()  ->  Update upperTurf GameObject
    GetNetworkData()    ->  Get a ByteArray of Turf
    SetNetworkData()    ->  Update Turf from ByteArray

 */


abstract public class Turf : MonoBehaviour
{
    Dictionary<string, byte> directions = new Dictionary<string, byte>(){
        { "N", 8 },
        { "E", 4 },
        { "S", 2 },
        { "W", 1 },
        { "NE", 128},
        { "SE", 64 },
        { "SW", 32 },
        { "NW", 16 }
    };
    
    public bool connectiveUpper = false; //adaptive models yes/no like walls
    public bool connectiveLower = false;
    public byte connectiveUpperNESW = 0;
    public byte connectiveLowerNESW = 0;
    
    [HideInInspector]
    public GameObject lowerTurf = null;
    [HideInInspector]
    public GameObject upperTurf = null;

    public abstract void UpdateUpperModel();
    public abstract void UpdateLowerModel();
    public abstract byte[] GetNetworkData();
    public abstract void SetNetworkData(byte[] inData);

    public void BuildTurf(){
        BuildUpper();
        BuildLower();
        UpdateNeighbourTurfs();
    }
    private void BuildUpper(){
        UpdateUpper();
    }
    private void BuildLower(){
        UpdateLower();
    }

    public void UpdateNeighbourTurfs(){
        Transform tileN = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z + 1));
        Transform tileE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z));
        Transform tileS = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z - 1));
        Transform tileW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z));
        // Transform tileNE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z + 1));
        // Transform tileSE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z - 1));
        // Transform tileSW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z - 1));
        // Transform tileNW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z + 1));
        if(tileN != null){
            //Debug.Log("updating: "+tileN.name);
            tileN.gameObject.GetComponent<Turf>().UpdateTurf();
        }
        if(tileE != null){
            //Debug.Log("updating: "+tileE.name);
            tileE.gameObject.GetComponent<Turf>().UpdateTurf();
        }
        if(tileS != null){
            //Debug.Log("updating: "+tileS.name);
            tileS.gameObject.GetComponent<Turf>().UpdateTurf();
        }
        if(tileW != null){
            //Debug.Log("updating: "+tileW.name);
            tileW.gameObject.GetComponent<Turf>().UpdateTurf();
        }
        // if(tileNE != null){
        //     tileNE.gameObject.GetComponent<Turf>().updateTurf();
        // }
        // if(tileSE != null){
        //     tileSE.gameObject.GetComponent<Turf>().updateTurf();
        // }
        // if(tileSW != null){
        //     tileSW.gameObject.GetComponent<Turf>().updateTurf();
        // }
        // if(tileNW != null){
        //     tileNW.gameObject.GetComponent<Turf>().updateTurf();
        // }
    }
    public virtual void UpdateTurf(){
            UpdateUpper();
            UpdateLower();       
    }

    private void UpdateUpper(){
        if (connectiveUpper){
            connectiveUpperNESW = 0;
            Transform tileN = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z + 1));
            Transform tileE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z));
            Transform tileS = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z - 1));
            Transform tileW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z));
            Transform tileNE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z + 1));
            Transform tileSE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z - 1));
            Transform tileSW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z - 1));
            Transform tileNW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z + 1));
            if(tileN != null){
                if (tileN.gameObject.GetComponent<Turf>().connectiveUpper){
                    connectiveUpperNESW ^= directions["N"];
                }
            }
            if(tileE != null){
                if (tileE.gameObject.GetComponent<Turf>().connectiveUpper){
                    connectiveUpperNESW ^= directions["E"];
                }
            }
            if(tileS != null){
                if (tileS.gameObject.GetComponent<Turf>().connectiveUpper){
                    connectiveUpperNESW ^= directions["S"];
                }
            }
            if(tileW != null){
                if (tileW.gameObject.GetComponent<Turf>().connectiveUpper){
                    connectiveUpperNESW ^= directions["W"];
                }
            }
            // if(tileNE != null){
            //     if (tileNE.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["NE"];
            //     }
            // }
            // if(tileSE != null){
            //     if (tileSE.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["SE"];
            //     }
            // }
            // if(tileSW != null){
            //     if (tileSW.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["SW"];
            //     }
            // }
            // if(tileNW != null){
            //     if (tileNW.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["NW"];
            //     }
            // }
            
        }
        //Debug.Log(modelName);
        UpdateUpperModel();
    }
    private void UpdateLower(){
        if (connectiveLower){
            connectiveLowerNESW = 0;
            Transform tileN = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z + 1));
            Transform tileE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z));
            Transform tileS = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x, gameObject.transform.position.z - 1));
            Transform tileW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z));
            Transform tileNE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z + 1));
            Transform tileSE = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x + 1, gameObject.transform.position.z - 1));
            Transform tileSW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z - 1));
            Transform tileNW = transform.parent.Find(string.Format("tile_{0}_{1}",gameObject.transform.position.x - 1, gameObject.transform.position.z + 1));
            if(tileN != null){
                if (tileN.gameObject.GetComponent<Turf>().connectiveLower){
                    connectiveLowerNESW ^= directions["N"];
                }
            }
            if(tileE != null){
                if (tileE.gameObject.GetComponent<Turf>().connectiveLower){
                    connectiveLowerNESW ^= directions["E"];
                }
            }
            if(tileS != null){
                if (tileS.gameObject.GetComponent<Turf>().connectiveLower){
                    connectiveLowerNESW ^= directions["S"];
                }
            }
            if(tileW != null){
                if (tileW.gameObject.GetComponent<Turf>().connectiveLower){
                    connectiveLowerNESW ^= directions["W"];
                }
            }
            // if(tileNE != null){
            //     if (tileNE.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["NE"];
            //     }
            // }
            // if(tileSE != null){
            //     if (tileSE.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["SE"];
            //     }
            // }
            // if(tileSW != null){
            //     if (tileSW.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["SW"];
            //     }
            // }
            // if(tileNW != null){
            //     if (tileNW.gameObject.GetComponent<Turf>().connectiveUpper){
            //         connectiveUpperNESW ^= directions["NW"];
            //     }
            // }
            
        }
        //Debug.Log(modelName);
        UpdateLowerModel();
    }
}
