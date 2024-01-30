using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridData = AstralCandle.GridSystem;

public class MapGeneration : MonoBehaviour{

    [SerializeField, Range(3, 16), Tooltip("The number of points this shape has")] int shape = 4;
    [SerializeField, Tooltip("The distance outwards from the center")] int depth = 8;
    [SerializeField] int startDepth = 1; // TEMP - HOWEVER, IT WILL BE NEEDED SO WE IGNORE THE CENTER

    float GetRadianAngle(int i) => i * 2 * Mathf.PI / shape; 
    Vector3 GetDirection(int i) => new Vector3(Mathf.Cos(GetRadianAngle(i)), 0, Mathf.Sin(GetRadianAngle(i)));
    private void OnDrawGizmos() {
        // DISCLAIMER: NEEDS DUPLICATE CHECKING FOR FINAL PRODUCT
        Vector3 startPos = GridData.Grid.FloorToGrid(transform.position) + (Vector3.one * GridData.Grid.HALF_CELL_SIZE);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPos, Vector3.one * GridData.Grid.CELL_SIZE);
        Gizmos.color = Color.white;
        for(int d = startDepth; d < depth; d++){
            for(int p = 0; p < shape; p++){
                Vector3Int dirA = Vector3Int.RoundToInt(GetDirection(p) * d);
                Vector3Int dirB = Vector3Int.RoundToInt(GetDirection((p + 1) % shape) * d);
                Vector3Int dirBA = (dirB - dirA); // Direction we need to move

                float magnitude = dirBA.magnitude;
                for(int x = 0; x < magnitude; x++){
                    Vector3 pos = Vector3.Lerp(dirA, dirB, (float)x / magnitude);
                    pos = GridData.Grid.RoundToGrid(pos);
                    Gizmos.DrawWireCube(startPos + pos, Vector3.one * GridData.Grid.CELL_SIZE);
                }
            }
        }        
    }
}
