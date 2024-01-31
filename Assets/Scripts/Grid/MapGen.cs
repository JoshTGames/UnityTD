using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GridData = AstralCandle.GridSystem;

public class MapGen : MonoBehaviour{
    #region VARIABLES
    [SerializeField, Tooltip("If true, will show the debugs associated to this script")] bool showDebug = true;
    [Header("Generation Settings")]
    [SerializeField, Range(3, 8), Tooltip("The number of points this shape has")] int shape = 4;
    [Header("Tile settings")]
    [SerializeField, Tooltip("The map tile we will spawn for each cell")] GameObject mapTile;
    [SerializeField, Tooltip("The map tile we will spawn for each cell")] Mesh testForOptimisation;
    [SerializeField, Tooltip("The map tile we will spawn for each cell")] Material testForOptimisationMat;
    [SerializeField, Tooltip("How long does it take for the tile to spawn in/decay out?")] float depthInstantiationSpeed = 1;
    [SerializeField, Tooltip("The animation of how the tile eases in/out of the scene")] AnimationCurve sizeAnimation;

    int targetDepth, previousDepth;
    public int TARGET_DEPTH{
        get => targetDepth;
        set{
            // Updates values
            previousDepth = targetDepth;
            targetDepth = Mathf.Abs(value);

            if(previousDepth != targetDepth){
                GenerateMap();
                // GenerateMapOp(out matrix);
            }
        }
    }
    #endregion

    public Dictionary<Vector3, Cell> tiles = new Dictionary<Vector3, Cell>();
    
    /// <summary>
    /// Updates the map
    /// </summary>
    void GenerateMap(){
        Vector3 startPosition = GridData.Grid.CenterToCell(GridData.Grid.FloorToGrid(transform.position)); // The center position of each cell
        if(!tiles.ContainsKey(startPosition)){ tiles.Add(startPosition, new Cell(mapTile, startPosition, transform)); } // Ensures start position always exists

        bool isHigher = previousDepth < targetDepth;
        for(int d = previousDepth; ((isHigher)? d < targetDepth : d >= targetDepth); d+= (isHigher)? 1 : -1){ // For loop which can go up or down based on difference in values
            for(int p = 0; p < shape; p++){
                Vector3 dirA = Utilities.GetRadianDirectionXZ(p, shape) * d;
                Vector3 dirB = Utilities.GetRadianDirectionXZ((p + 1) % shape, shape) * d;
                Vector3 dirBA = (dirB - dirA); // The direction we want to move towards

                float magnitude = dirBA.magnitude;
                for(float x = 0; x < magnitude; x += 0.5f){
                    Vector3 pos = Vector3.Lerp(dirA, dirB, x / magnitude);
                    pos = GridData.Grid.RoundToGrid(pos);

                    Vector3 finalPosition = startPosition + pos;          

                    // Check for duplicates...
                    if(!tiles.ContainsKey(finalPosition) && isHigher){
                        tiles.Add(finalPosition, new Cell(mapTile, finalPosition, transform));
                        continue;
                    }
                    else if(tiles.ContainsKey(finalPosition) && isHigher){ tiles[finalPosition].markForRemoval = false; } // If all of a sudden it has changed... 
                    else if(tiles.ContainsKey(finalPosition) && !isHigher){ tiles[finalPosition].markForRemoval = true; }
                }
            }
        }
    }

    Matrix4x4[] matrix;
    void GenerateMapOp(out Matrix4x4[] _matrix){
        HashSet<Matrix4x4> newMatrix = new HashSet<Matrix4x4>();
        Vector3 startPosition = GridData.Grid.CenterToCell(GridData.Grid.FloorToGrid(transform.position)); // The center position of each cell

        newMatrix.Add(Matrix4x4.TRS(startPosition, Quaternion.identity, Vector3.one * GridData.Grid.CELL_SIZE));
        for(int d = 0; d < targetDepth; d++){
            for(int p = 0; p < shape; p++){
                Vector3 dirA = Utilities.GetRadianDirectionXZ(p, shape) * d;
                Vector3 dirB = Utilities.GetRadianDirectionXZ((p + 1) % shape, shape) * d;
                Vector3 dirBA = (dirB - dirA); // The direction we want to move towards

                float magnitude = dirBA.magnitude;
                for(float x = 0; x < magnitude; x += 0.5f){
                    Vector3 pos = Vector3.Lerp(dirA, dirB, x / magnitude);
                    pos = GridData.Grid.RoundToGrid(pos);

                    Vector3 finalPosition = startPosition + pos;
                    Matrix4x4 calculatedMatrix = Matrix4x4.TRS(finalPosition, Quaternion.identity, Vector3.one * GridData.Grid.CELL_SIZE);
                    if(newMatrix.Contains(calculatedMatrix)){ continue; }
                    newMatrix.Add(calculatedMatrix);
                }
            }
        }     
        _matrix = newMatrix.ToArray();
    }
    RenderParams rp;

    private void Start() {
        rp = new RenderParams(testForOptimisationMat);
    }

    private void LateUpdate() {
        List<Vector3> keys = new List<Vector3>(tiles.Keys);
        for(int i = keys.Count-1; i >= 0; i--){
            Cell cell = tiles[keys[i]];
            if(!cell.tile){ 
                tiles.Remove(keys[i]); 
                continue;
            }
            float time = (cell.markForRemoval)? -Time.deltaTime : Time.deltaTime;
            cell.Scale(depthInstantiationSpeed, time, sizeAnimation);
        }        
        // if(matrix == null || matrix.Length <=0){ return;}        
        // Graphics.RenderMeshInstanced(rp, testForOptimisation, 0, matrix);
    }

    private void OnDrawGizmos() {
        if(!showDebug){ return; }
        foreach(Vector3 value in tiles.Keys){
            Gizmos.DrawCube(value, Vector3.one * GridData.Grid.CELL_SIZE);
        }
    }
    public class Cell{
        public Transform tile{ get; private set; } // The active object
        public bool markForRemoval = false; // If true will decay the tile
        float elapsedTime = 0;
        
        // Constructor
        public Cell(GameObject _tile, Vector3 position, Transform folder = null){
            tile = Instantiate(_tile, position, Quaternion.identity, folder).transform;
        }

        public void Scale(float duration, float time, AnimationCurve animation){
            elapsedTime += time;
            elapsedTime = Mathf.Clamp(elapsedTime, 0, duration);
            if(markForRemoval && tile.localScale.sqrMagnitude <= 0){ Destroy(tile.gameObject); }
            Vector3 size = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, animation.Evaluate(elapsedTime / duration));
            tile.localScale = size;
        }
    }
}
