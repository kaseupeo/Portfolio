using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class AISensor : MonoBehaviour
{
    public float distance;
    public float height;
    public float angle;
    public Color meshColor = Color.red;
    public int ScanFrequency = 30;
    public LayerMask layer;
    public LayerMask occulsionLayer;
    public Category objCatergory;


    private Collider[] cols = new Collider[50];
    private int count;
    private float scanInterval;
    private float scanTimer;

    [SerializeField] internal bool hasTarget;
    [SerializeField] private GameObject targetPlayer;

    public GameObject TargetPlayer
    {
        get => targetPlayer;
        set
        {
            if (value == null)
            {
                hasTarget = false;
                targetPlayer = value;
                return;
            }

            targetPlayer = value;
            hasTarget = true;
        }
    }

    [SerializeField] private List<Creature> detectedObjList = new();
    public IReadOnlyList<Creature> DetectedObjList => detectedObjList;

    private Mesh mesh;



    private void Start()
    {
        scanInterval = 1f / ScanFrequency;
    }

    private void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        //시야각, 후각과 같은 행동 패턴을 위해 콜라이더 배열로 주변 오브젝트를 미리 저장
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, cols, layer, QueryTriggerInteraction.Collide);
        detectedObjList.Clear();
        if (count == 0) return;
        foreach (Collider col in cols)
        {
            //플레이어인지와 상관없이 Creature인 경우에 탐지
            if (col != null && col.gameObject.TryGetComponent(out Creature creature))
            {
                //TODO: 추후 주변에 어떤 물체가 있는지, 물체에따라 상호작용하는 방법에 따라 조건을 수정할 예정
                //플레이어인지 확인하는 조건
                if (IsInSight(creature) && true == creature.IsPlayer)
                {
                    detectedObjList.Add(creature);
                    TargetPlayer = creature.gameObject;
                }
                else if (IsInSight(creature))
                {
                    detectedObjList.Add(creature);
                }



            }
        }

    }
    //FOV
    private bool IsInSight(Creature obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = obj.transform.position - transform.position;

        //시야각 높이보다 낮거나 높이 있을 경우
        if (direction.y > height || direction.y < 0)
        {
            return false;
        }

        //시야각에 들어오지 않았을 경우
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        //물체가 가로막고 있을 경우
        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occulsionLayer))
        {
            return false;
        }

        return true;
    }




    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;
        Vector3[] verticies = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + (Vector3.up * height);
        Vector3 topLeft = bottomLeft + (Vector3.up * height);
        Vector3 topRight = bottomRight + (Vector3.up * height);

        int vert = 0;

        //left side
        verticies[vert++] = bottomCenter;
        verticies[vert++] = bottomLeft;
        verticies[vert++] = topLeft;

        verticies[vert++] = topLeft;
        verticies[vert++] = topCenter;
        verticies[vert++] = bottomCenter;

        //right side
        verticies[vert++] = bottomCenter;
        verticies[vert++] = topCenter;
        verticies[vert++] = topRight;

        verticies[vert++] = topRight;
        verticies[vert++] = bottomRight;
        verticies[vert++] = bottomCenter;

        float curAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for (int i = 0; i < segments; ++i)
        {

            bottomCenter = Vector3.zero;
            bottomLeft = Quaternion.Euler(0, curAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, curAngle + deltaAngle, 0) * Vector3.forward * distance;

            topCenter = bottomCenter + (Vector3.up * height);
            topLeft = bottomLeft + (Vector3.up * height);
            topRight = bottomRight + (Vector3.up * height);

            curAngle += deltaAngle;

            //far side
            verticies[vert++] = bottomLeft;
            verticies[vert++] = bottomRight;
            verticies[vert++] = topRight;

            verticies[vert++] = topRight;
            verticies[vert++] = topLeft;
            verticies[vert++] = bottomLeft;

            //top
            verticies[vert++] = topCenter;
            verticies[vert++] = topLeft;
            verticies[vert++] = topRight;

            //bottom
            verticies[vert++] = bottomCenter;
            verticies[vert++] = bottomRight;
            verticies[vert++] = bottomLeft;
        }

        for (int i = 0; i < verticies.Length; ++i)
        {
            triangles[i] = i;
        }
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }
    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(cols[i].transform.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in detectedObjList)
        {
            Gizmos.DrawSphere(obj.transform.position, 0.2f);
        }
    }

}
