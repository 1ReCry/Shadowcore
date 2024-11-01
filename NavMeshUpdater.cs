using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUpdater : MonoBehaviour
{
    NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    void FixedUpdateUpdate()
    {
        if(Globals.navUpdateRequest)
        {
            Globals.navUpdateRequest = false;
            //navMeshSurface.BuildNavMesh();
        }
    }
}
