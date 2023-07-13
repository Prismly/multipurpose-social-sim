using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used to help place structures along roads
//Still a WIP
public class StructureHelper : MonoBehaviour
{
    //public GameObject prefab;
    [Tooltip("Structures with quantity -1 should be on the bottom of the list.")]
    public StructureType[] structureTypes;
    public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>(); //like roads, structures are stored in a Dictionary

    public void PlaceStructures(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpaces(roadPositions);
        List<Vector3Int> blockedPositions = new List<Vector3Int>();
        foreach (var freeSpot in freeEstateSpots)
        {
            if (blockedPositions.Contains(freeSpot.Key))
            {
                continue;
            }
            var rotation = Quaternion.identity; //default door position is UP
            switch (freeSpot.Value)
            {
                case Direction.Down:
                    rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Direction.Left:
                    rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Right:
                    rotation = Quaternion.Euler(0, 0, -90);
                    break;
                default:
                    break;
            }
            for (int i = 0; i < structureTypes.Length; i++)
            {
                if (structureTypes[i].quantity == -1)
                {
                    var building = SpawnPrefab(structureTypes[i].GetPrefab(), freeSpot.Key, rotation);
                    structuresDictionary.Add(freeSpot.Key, building);
                    break;
                }
                if (structureTypes[i].IsBuildingAvailable())
                {
                    if (structureTypes[i].sizeRequired > 1)
                    {
                        var halfSize = Mathf.FloorToInt(structureTypes[i].sizeRequired / 2.0f);
                        List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                        if (VerifyBuildingLocation(halfSize, freeEstateSpots, freeSpot, ref tempPositionsBlocked))
                        {
                            blockedPositions.AddRange(tempPositionsBlocked);
                            var building = SpawnPrefab(structureTypes[i].GetPrefab(), freeSpot.Key, rotation);
                            structuresDictionary.Add(freeSpot.Key, building);
                            foreach(var pos in tempPositionsBlocked)
                            {
                                structuresDictionary.Add(pos, building);
                            }
                        }
                    } else
                    {
                        var building = SpawnPrefab(structureTypes[i].GetPrefab(), freeSpot.Key, rotation);
                        structuresDictionary.Add(freeSpot.Key, building);
                    }
                    break;
                }
            }
            //Instantiate(prefab, freeSpot.Key, rotation, transform);
        }
    }

    private bool VerifyBuildingLocation(int halfSize, Dictionary<Vector3Int, Direction> freeEstateSpots,
        KeyValuePair<Vector3Int, Direction> freeSpot, ref List<Vector3Int> tempPositionsBlocked)
    {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up)
        {
            direction = Vector3Int.right;
        } else
        {
            direction = Vector3Int.up;
        }
        for (int i = 1; i < halfSize; i++)
        {
            var pos1 = freeSpot.Key + direction * i;
            var pos2 = freeSpot.Key - direction * i;
            if(!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2))
            {
                return false;
            }
            tempPositionsBlocked.Add(pos1);
            tempPositionsBlocked.Add(pos2);
        }
        return true;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation)
    {
        var newStructure = Instantiate(prefab, position, rotation, transform);
        return newStructure;
    }

    private Dictionary<Vector3Int, Direction> FindFreeSpaces(List<Vector3Int> roadPositions)
    {
        Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
        foreach (var position in roadPositions)
        {
            var neighborDirections = PlacementHelper.findNeighbor(position, roadPositions);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if(neighborDirections.Contains(direction)== false)
                {
                    var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                    if (freeSpaces.ContainsKey(newPosition))
                    {
                        continue;
                    }
                    freeSpaces.Add(newPosition, PlacementHelper.GetReverseDirection(direction));
                }
            }
        }
        return freeSpaces;
    }
}
