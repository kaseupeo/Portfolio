using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MapGenerator : MonoBehaviour
{
    private const float XSpacing = 1.7321f;
    private const float ZSpacing = 1.5f;
    private const float XSubSpacing = 0.86607f;
    
    [Range(1f, 3f)]
    [SerializeField] private float hexMargin = 1f;
    
    [Header("Hexagons to Instantiate")]
    [SerializeField] private List<GameObject> hexagonTileList;
    
    [Header("Hexagons Field Size")]
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    
    [Header("Random Y pos (Step is 0.5)")]
    [SerializeField] private bool randomY = false;
    [SerializeField] private float maxY;
    [SerializeField, Range(0,100)] private int chanceY;
    
    [Header("Random rotation")]
    [SerializeField] private bool randomRotation = false;
    
    [Header("Fill settings")]
    [SerializeField] private bool addFillParts = false;
    [SerializeField, Range(0, 100)] private int chanceFill;
    [SerializeField] private List<GameObject> fillPrefabList;
    [SerializeField] private bool randomizeFillChildren = false;
    [SerializeField] private bool randomizeChildrenRotation = false;
    
    public void GenerateField()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) 
            DestroyImmediate(transform.GetChild(i).gameObject);

        for (int i = 0; i < sizeX; i++)
        {
            for (int a = 0; a < sizeZ; a++)
            {
                Vector3 pos = new Vector3(i * (XSpacing * hexMargin), 0f, a * (ZSpacing * hexMargin));

                if (a % 2 != 0) 
                    pos.x += (XSubSpacing * hexMargin);

                if (randomY)
                {
                    if(Random.Range(0,100) <= chanceY)
                        pos.y = Random.Range(1, (int)(maxY / 0.5f)) * 0.5f;
                }
                
                GameObject newHex = Instantiate(hexagonTileList[Random.Range(0, hexagonTileList.Count)], pos, new Quaternion());
                newHex.transform.parent = transform;

                if (randomRotation) 
                    newHex.transform.eulerAngles = new Vector3(0f, Random.Range(0, 7) * 60f, 0f);

                if(addFillParts)
                {
                    if (Random.Range(0, 100) <= chanceFill)
                    {
                        List<GameObject> toDestroy = new List<GameObject>();
                        GameObject fill = Instantiate(fillPrefabList[Random.Range(0, fillPrefabList.Count)], newHex.transform);
                        fill.transform.localPosition = new Vector3(0f, -1f, 0f);

                        fill.transform.localEulerAngles = Random.Range(0, 4) switch
                        {
                            0 => new Vector3(0, 0, 0),
                            1 => new Vector3(0, 90, 0),
                            2 => new Vector3(0, 180, 0),
                            3 => new Vector3(0, 270, 0),
                            _ => fill.transform.localEulerAngles
                        };

                        if (!randomizeFillChildren) 
                            continue;
                        
                        for (int o = 0; o < fill.transform.childCount; o++)
                        {
                            fill.transform.GetChild(o).gameObject.SetActive(Random.Range(0, 2) != 0);
                            if (fill.transform.GetChild(o).gameObject.activeSelf)
                            {
                                if (randomizeChildrenRotation) 
                                    fill.transform.GetChild(o).localEulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
                            }
                            else
                            {
                                toDestroy.Add(fill.transform.GetChild(o).gameObject);
                            }
                        }

                        for (int o = toDestroy.Count - 1; o >= 0; o--) 
                            DestroyImmediate(toDestroy[o]);
                    }
                }
            }
        }
    }
    
    public void DeleteField()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) 
            DestroyImmediate(transform.GetChild(i).gameObject);
    }
}
