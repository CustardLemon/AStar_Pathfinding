using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public float MaxProportionOfNodesToCheck = 0.33f;

    public GameObject TestSphere;
    public MouseContoller Msc;

    Grid grid;
    public Transform StartPosition = null;
    public Transform TargetPosition;

    private void Awake()
    {

        grid = GetComponent<Grid>();
        Msc = FindObjectOfType<MouseContoller>();

    }


    public void Run(Vector3 StartPos, Vector3 EndPos)
    {


        FindPath(StartPos, EndPos);
        // Instantiate(TestSphere, StartPosition.position, transform.rotation);
        // Instantiate(TestSphere, TargetPosition.position, transform.rotation);

        // Debug.Log("Ran");

    }
    void FindPath(Vector3 a_start, Vector3 a_target)
    {


        Node StartNode = grid.NodeFromWorldPosition(a_start);
        Node TargetNode = grid.NodeFromWorldPosition(a_target);

        //grid.FreeTargetNode();

        // Instantiate(TestSphere, StartNode.Position, transform.rotation);
        // Instantiate(TestSphere, TargetNode.Position, transform.rotation);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].hCost < CurrentNode.hCost)
                {
                    CurrentNode = OpenList[i];
                }
            }
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);



            if (CurrentNode == TargetNode || ClosedList.Count > grid.NumberOfYNodes * grid.NumberOfYNodes * MaxProportionOfNodesToCheck)
            {
                //Debug.Log("Completed Pathfding       Number of Nodes Checked  "+ClosedList.Count);

                GetFinalPath(StartNode, TargetNode);
                break;
            }

            foreach (Node NeighborNode in grid.GetNeighboringNodes(CurrentNode))
            {
                if ((!NeighborNode.IsWall) || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                int MoveCost = CurrentNode.gCost + GetManHattenDistance(CurrentNode, NeighborNode);

                if (MoveCost < NeighborNode.FCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.gCost = MoveCost;
                    NeighborNode.hCost = GetManHattenDistance(NeighborNode, TargetNode);
                    NeighborNode.Parent = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }
        // Debug.Log("No path       Number of Explored Nodes "+ClosedList.Count +"     Grid Size  " +grid.NumberOfXNodes* grid.NumberOfYNodes  );
    }

    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = a_EndNode;

        while (CurrentNode != a_StartingNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        FinalPath.Reverse();

        grid.FinalPath = FinalPath;

        //foreach(Node node in FinalPath)
        //{
        //    Instantiate(TestSphere, node.Position, transform.rotation);
        //
        //}

        // Debug.Log("Final Path count 2 "+ FinalPath.Count);
    }

    int GetManHattenDistance(Node a_NodeA, Node a_NodeB)
    {
        int ix = Mathf.Abs(a_NodeA.gridX - a_NodeB.gridX);
        int iy = Mathf.Abs(a_NodeA.gridY - a_NodeB.gridY);

        return ix + iy;
    }
}
