using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    RadioCircle RC;

    List<Node> OldFinalPath;

    public GameObject TestSphere;
    public Vector3 EndPoint;
    public GameObject MouseCon;
    MouseContoller Msc;
    public GameObject Waypoint;
    public Vector3 StartPosition;
    public LayerMask WallMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float Distance;
    Vector3 bottomLeft;
    Node[,] grid;
    public List<Node> FinalPath;
    public int NumberOfXNodes;
    public int NumberOfYNodes;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    Vector3 StartToTarget;
    private void Start()
    {
        Msc = MouseCon.GetComponent<MouseContoller>();

        nodeDiameter = 2 * nodeRadius;


        RC = FindObjectOfType<RadioCircle>();

    }



    public void Run(Vector3 StartPos, Vector3 EndPos)
    {

        gridWorldSize = new Vector2(RC.radius * 2 + nodeDiameter, RC.radius * 2 + nodeDiameter);





        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);



        // Debug.Log("called Run");

        EndPoint = FindEmptySpot(EndPos);


        // StartPosition.position = FindEmptySpot(StartPosition.position);


        StartPosition = FindEmptySpot(StartPos);



        CreateGrid();



    }

    public void CreateGrid()
    {

        int StartX = Mathf.RoundToInt(-gridSizeX / 2) - 1;
        int StartY = Mathf.RoundToInt(-gridSizeY / 2) - 1;

        int EndX = Mathf.RoundToInt(gridSizeX / 2) + 1;
        int EndY = Mathf.RoundToInt(gridSizeY / 2) + 1;

        NumberOfXNodes = EndX - StartX + 1;
        NumberOfYNodes = EndY - StartY + 1;


        bottomLeft = new Vector3(-gridWorldSize.x / 2, -gridWorldSize.y / 2, 0);

        grid = new Node[NumberOfXNodes, NumberOfYNodes];


        for (int x = 0; x < NumberOfXNodes; x++)
        {
            for (int y = 0; y < NumberOfYNodes; y++)
            {

                Vector3 worldpoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);

                bool Wall = true;

                if (Physics2D.OverlapCircle(worldpoint, nodeDiameter, WallMask) || worldpoint.magnitude > RC.radius)
                {
                    Wall = false;

                    // GameObject TestSPhere = Instantiate(TestSphere, worldpoint, transform.rotation);
                    // TestSPhere.transform.localScale = new Vector3(nodeDiameter, nodeDiameter,1);
                }

                grid[x, y] = new Node(Wall, worldpoint, x, y);

                // if (Wall)
                // {
                //     GameObject TestSPhere = Instantiate(TestSphere, worldpoint, transform.rotation);
                //    TestSPhere.transform.localScale = new Vector3(nodeDiameter, nodeDiameter, 1);
                // }
            }

        }


    }

    public Node NodeFromWorldPosition(Vector3 a_WorldPosition)
    {

        float xpoint = ((a_WorldPosition.x - bottomLeft.x - nodeRadius) / gridWorldSize.x);
        float ypoint = ((a_WorldPosition.y - bottomLeft.y - nodeRadius) / gridWorldSize.y);


        xpoint = Mathf.Abs(Mathf.Clamp(xpoint, -1, 1));
        ypoint = Mathf.Abs(Mathf.Clamp(ypoint, -1, 1));

        int x = Mathf.RoundToInt((NumberOfXNodes) * xpoint) - 1;
        int y = Mathf.RoundToInt((NumberOfYNodes) * ypoint) - 1;
        // Debug.Log(""+x+"    "+y);
        return grid[x, y];


    }

    public List<Node> GetNeighboringNodes(Node a_Node)
    {
        List<Node> NeighboringNodes = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //if we are on the node tha was passed in, skip this iteration.
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = a_Node.gridX + x;
                int checkY = a_Node.gridY + y;

                //Make sure the node is within the grid.
                if (checkX >= 0 && checkX < NumberOfXNodes && checkY >= 0 && checkY < NumberOfYNodes)
                {
                    NeighboringNodes.Add(grid[checkX, checkY]); //Adds to the neighbours list.
                }

            }
        }
        return NeighboringNodes;
    }



    public bool SpawnWaypoints(Transform Ship)
    {
        bool FoundPath = false;
        // if (StartPosition != null)
        {
            Ship.GetComponent<Find_Target>().Waypoints.Clear();

            if (FinalPath != null && FinalPath != OldFinalPath && FinalPath.Count > 0)
            {
                FoundPath = true;

                OldFinalPath = FinalPath;




                //delete nodes with internal angle les than ninety
                foreach (Node node in FinalPath)
                {



                    if (Findangle(node) < 91)
                    {

                        GameObject waypoint = Waypoint;
                        waypoint.transform.position = node.Position;

                        //Instantiate(waypoint, node.Position, transform.rotation, Ship);

                        Ship.GetComponent<Find_Target>().Waypoints.Add(Instantiate(waypoint, node.Position, transform.rotation, Ship));

                    }


                }
                //Debug.Log(Ship.GetComponent<Find_Target>().Waypoints[0].name);
                //Debug.Log("check");

                GameObject SecondWayPoint = Waypoint;


                if (Ship.GetComponent<Find_Target>().Waypoints[Ship.GetComponent<Find_Target>().Waypoints.Count - 1])

                {

                    GameObject newWypoint = Instantiate(SecondWayPoint, new Vector3(EndPoint.x, EndPoint.y, Ship.transform.position.z), transform.rotation, Ship);
                    Ship.GetComponent<Find_Target>().Waypoints.Add(newWypoint);


                }



                if (Ship.GetComponent<Find_Target>().Waypoints.Count > 0 && Ship.GetComponent<Find_Target>().Waypoints[0] != null)
                {
                    Ship.GetComponent<Find_Target>().ClosestEnemy = Ship.GetComponent<Find_Target>().Waypoints[0];
                }
            }
            else
            {

                // Debug.Log("SpawnWaypoints did not run");

                // if(FinalPath == null)
                //    Debug.Log("no final path");

                // if(FinalPath == OldFinalPath)
                //    Debug.Log("Same final path");



            }
        }


        return FoundPath;

    }



    Vector3 FindEmptySpot(Vector3 point)
    {
        if (!Physics2D.OverlapCircle(point, nodeRadius * 2))
        {

            //Debug.Log("Empty Spot");


            return point;


        }


        else

        {
            Vector3 newPoint = point;

            float radius = nodeRadius * 2;

            int loopbreaker = 0;

            do
            {
                loopbreaker++;

                if (loopbreaker > 10)
                {
                    loopbreaker = 0;
                    radius++;

                    if (radius > 3)
                        break;
                }


                point = new Vector3(point.x + Random.Range(-radius, radius), point.y + Random.Range(-radius, radius), point.z);
            }
            while (Physics2D.OverlapCircle(point, radius));

            if (newPoint == point)
                Debug.Log("same point");

            Vector3 difference = newPoint - point;

            // Debug.Log("new Point difference " + difference);


            return newPoint;


        }
    }


    float Findangle(Node node)
    {
        Vector3 PreviousNode = new Vector3(0, 0, 0);
        Vector3 NextNode = new Vector3(0, 0, 0);

        if (FinalPath.Count > 0)
        {

            if (FinalPath.IndexOf(node) > 0)
                PreviousNode = FinalPath[FinalPath.IndexOf(node) - 1].Position;

            if (FinalPath.IndexOf(node) == 0)
                PreviousNode = StartPosition;

            if (FinalPath.IndexOf(node) + 1 < FinalPath.Count)
                NextNode = FinalPath[FinalPath.IndexOf(node) + 1].Position;

        }

        if (PreviousNode != null && NextNode != null && node != null)
        {

            Vector3 PreviousToCurrent = node.Position - PreviousNode;

            Vector3 CurrentToNext = NextNode - node.Position;

            float angle = Vector3.Angle(PreviousToCurrent, CurrentToNext);

            // Vector2 P1 = PreviousNode.transform.position;
            // Vector2 P2 = node.transform.position;
            // Vector2 P3 = NextNode.transform.position;
            //
            //
            //
            // float result =  Mathf.Atan2(P3.y - P1.y, P3.x - P1.x) -
            //    Mathf.Atan2(P2.y - P1.y, P2.x - P1.x);
            //
            //
            // result = Mathf.Abs( Mathf.Rad2Deg*result);

            // Debug.Log(""+ PreviousNode.name+"   " + node.name + "     "+ NextNode.name+ "    " + angle);

            return angle;
        }


        return 0;

    }

}