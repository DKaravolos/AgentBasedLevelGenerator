using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class GameManagerLD : MonoBehaviour
{
    public LevelDigger diggerPrefab;
    public NavMeshSurface navmeshPrefab;

    //Privates
    private LevelDigger diggerInstance;
    private NavMeshSurface navmesh;

    private void Start()
    {
        StartCoroutine(BeginGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            navmesh.BuildNavMesh();
        }
    }

    private IEnumerator BeginGame()
    {
        //Setup main camera
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        Camera.main.clearFlags = CameraClearFlags.Skybox;

        //Create the level
        diggerInstance = Instantiate(diggerPrefab) as LevelDigger;
        diggerInstance.transform.position = Vector3.zero;
        yield return StartCoroutine(diggerInstance.Generate());

        //Setup minimap camera
        Camera.main.rect = new Rect(0f, 0f, 0.4f, 0.4f);
        Camera.main.clearFlags = CameraClearFlags.Depth;

        //Build Navmesh
        navmesh = Instantiate(navmeshPrefab) as NavMeshSurface;
        navmesh.BuildNavMesh();
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        Destroy(diggerInstance.gameObject);
        Destroy(navmesh.gameObject);
        StartCoroutine(BeginGame());
    }
}
