// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GridData = AstralCandle.GridSystem;

// public class MapGeneration : MonoBehaviour{
//     public static MapGeneration instance;
//     Dictionary<Vector3, Cell> registeredCells = new Dictionary<Vector3, Cell>();
//     [SerializeField] bool showDebug = true;
//     [SerializeField, Range(3, 8), Tooltip("The number of points this shape has")] int shape = 4;
//     [SerializeField] MapTile mapTile;

//     int targetDepth = 0, previousDepth = 0;
//     public int TARGET_DEPTH{
//         get => targetDepth;
//         set{
//             previousDepth = targetDepth;
//             targetDepth = value;
//             if(previousDepth != targetDepth){ GenerateMap(previousDepth, value); }
//         }
//     }

//     #region MAP FUNCTIONS
//     /// <summary>
//     /// Calculates the radian angle of i out of the the number of given points
//     /// </summary>
//     /// <param name="i">The number we will be calculating for</param>
//     /// <returns>A radian angle</returns>
//     float GetRadianAngle(int i) => i * 2 * Mathf.PI / shape; 
//     /// <summary>
//     /// Converts radian angles to usable directions in unity
//     /// </summary>
//     /// <param name="i">The circle point we want to calculate for</param>
//     /// <returns>A Vector3 direction between -1 and 1</returns>
//     Vector3 GetDirection(int i) => new Vector3(Mathf.Cos(GetRadianAngle(i)), 0, Mathf.Sin(GetRadianAngle(i)));

//     /// <summary>
//     /// For each depth will generate a circle which will either be removed/added to a dictionary depending on the displacement between previousDepth and targetDepth
//     /// </summary>
//     /// <param name="_previousDepth">The previous depth we were at before calling this function</param>
//     /// <param name="_targetDepth">The current depth we want to move towards</param>
//     void GenerateMap(int _previousDepth, int _targetDepth){
//         Vector3 startPos = GridData.Grid.FloorToGrid(transform.position) + (Vector3.one * GridData.Grid.HALF_CELL_SIZE);
        
//         bool isHigher = _previousDepth < _targetDepth;
//         for(int d = _previousDepth; ((isHigher)? d < _targetDepth : d > _targetDepth); d+= (isHigher)? 1 : -1){ // For loop which can go up or down based on difference in values
//             for(int p = 0; p < shape; p++){
//                 Vector3Int dirA = Vector3Int.RoundToInt(GetDirection(p) * d);
//                 Vector3Int dirB = Vector3Int.RoundToInt(GetDirection((p + 1) % shape) * d);
//                 Vector3Int dirBA = (dirB - dirA); // Direction we need to move
//                 float magnitude = dirBA.magnitude;
//                 for(int x = 0; x < magnitude; x++){
//                     Vector3 pos = Vector3.Lerp(dirA, dirB, (float)x / magnitude);
//                     pos = startPos + GridData.Grid.RoundToGrid(pos);

//                     // ADD TO DICTIONARY
//                     if(registeredCells.ContainsKey(pos) && !isHigher){ 
//                         registeredCells[pos].Destroy();
//                         registeredCells.Remove(pos);
//                     }
//                     else if(!registeredCells.ContainsKey(pos) && isHigher){                    
//                         registeredCells.Add(pos, new Cell(mapTile, pos));
//                     }
//                 }
//             }
//         }
//     }
//     #endregion

//     private void Awake() => instance = this;

//     /// <summary>
//     /// Displays the cells
//     /// </summary>
//     private void OnDrawGizmos() {
//         if(!showDebug){ return; }
//         foreach(KeyValuePair<Vector3, Cell> keyValue in registeredCells){
//             Gizmos.DrawWireCube(keyValue.Key, Vector3.one * GridData.Grid.CELL_SIZE);
//         }
//     }

//     public class Cell{
//         MapTile mapTile;

//         public Cell(MapTile _mapTile, Vector3 position) => Spawn(_mapTile, position);


//         public void Destroy() => mapTile.isActive = false;
//         public void Spawn(MapTile _mapTile, Vector3 position){
//             // Summon cell on map
//             mapTile = Instantiate(_mapTile.gameObject, position, Quaternion.identity, null).GetComponent<MapTile>();
//         }
//     }
// }
