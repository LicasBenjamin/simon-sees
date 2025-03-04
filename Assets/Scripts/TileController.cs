using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    bool[] tilePlayerIsOn = new bool[9];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Test()
    {
        print("parent function calling works!");
    }
    public void UpdateTileArray(int index)
    {
        if (!tilePlayerIsOn[index])
        {
            tilePlayerIsOn = new bool[9];
            tilePlayerIsOn[index] = true;
            print(string.Join(",", tilePlayerIsOn));
        }
    }
}
