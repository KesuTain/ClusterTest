using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cluster
{
    public int type = 0;
    public List<GameObject> Elements = new List<GameObject>();
    public Vector3 position = new Vector3(0,0,0);
    public GameObject Label;

    public Cluster()
    {

    }

    public Cluster(int type)
    {
        this.type = type;
    }

    ~Cluster()
    {

    }
}

public class Clusterator : MonoBehaviour
{
    public GameObject[] Items;
    public float MinDist = 1;
    public List<Cluster> Clusters = new List<Cluster>();
    public GameObject Label;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(FormClusters());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        FormClusters();
        FormClustersCenters();
        ShowClusters();
    }

    public void FormClusters()
    {
        Items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < Items.Length; i++)
        {
            Cluster cluster = GetClusterOfObject(Items[i]);

            if (cluster == null)
            {
                cluster = new Cluster();
                cluster.Elements.Add(Items[i]);
            }

            int objType = GetTypeOfObject(Items[i]);
            if(objType != -1)
            {
                for (int j = 0; j < Items.Length; j++)
                {
                    if(i != j)
                    {
                        if(objType == GetTypeOfObject(Items[j]))
                        {
                            Cluster objCluster = GetClusterOfObject(Items[j]);

                            if (Vector3.Distance(Items[i].transform.position, Items[j].transform.position) <= MinDist)
                            {
                                GameObject addObject = Items[j];
                                if (objCluster != null)
                                {
                                    Clusters.Remove(cluster);
                                    cluster = objCluster;
                                    addObject = Items[i];
                                }
                                if(!cluster.Elements.Contains(addObject))
                                    cluster.Elements.Add(addObject);
                            }
                            else
                            {
                                GameObject removeObject = Items[j];
                                cluster.Elements.Remove(removeObject);

                            }
                        }
                    }
                }
            }

            if (!Clusters.Contains(cluster))
                Clusters.Add(cluster);
        }
    }

    public Cluster GetClusterOfObject(GameObject obj)
    {
        foreach(Cluster cluster in Clusters)
        {
            if (cluster.Elements.Contains(obj))
            {
                return cluster;
            }
        }
        return null;
    }

    public void FormClustersCenters()
    {
        foreach(Cluster cluster in Clusters)
        {
            Vector3 center = Vector3.zero;
            foreach (GameObject obj in cluster.Elements)
            {
                center += obj.transform.position;
            }
            cluster.position = center / cluster.Elements.Count;

            foreach (GameObject obj in cluster.Elements)
            {
                Debug.DrawLine(obj.transform.position, cluster.position);
            }
        }
    }

    public void ShowClusters()
    {
        foreach (Cluster cluster in Clusters)
        {
            Debug.DrawLine(cluster.position, cluster.position + Vector3.up, Color.blue);
        }
    }

    public int GetTypeOfObject(GameObject obj)
    {
        int type = -1;
        switch (obj.GetComponent<Renderer>().material.name)
        {
            case "Item1 (Instance)":
                type = 0;
                break;
            case "Item2 (Instance)":
                type = 1;
                break;
            case "Item3 (Instance)":
                type = 2;
                break;
            default:
                break;
        }
        return type;
    }

    public Cluster GetClusterOfType(int type)
    {
        Cluster foundedCluster = null;
        foreach (Cluster cluster in Clusters)
        {
            if(cluster.type == type)
            {
                foundedCluster = cluster;
                break;
            }
        }
        if(foundedCluster == null)
        {
            foundedCluster = new Cluster(type);
            Clusters.Add(foundedCluster);
        }
        return foundedCluster;
    }
}
