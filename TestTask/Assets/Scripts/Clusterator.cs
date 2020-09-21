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


    public int Size { get { return Elements.Count; } set { } }

    public Cluster()
    {

    }

    public void Add(GameObject obj)
    {
        if (!this.Elements.Contains(obj))
        {
            this.Elements.Add(obj);
            this.CalculateCenter();
        }
    }

    public void CalculateCenter()
    {
        Vector3 center = Vector3.zero;
        foreach (GameObject obj in this.Elements)
        {
            center += obj.transform.position;
        }
        this.position = center / this.Elements.Count;
    }

    public void Remove(GameObject obj)
    {
        this.Elements.Remove(obj);
        this.CalculateCenter();
    }

    public bool Empty()
    {
        if (this.Elements.Count == 0)
            return true;
        else
            return false;
    }

    public bool Single()
    {
        if (this.Elements.Count == 1)
            return true;
        else
            return false;
    }

    public bool Contains(GameObject obj)
    {
        if (this.Elements.Contains(obj))
            return true;
        else
            return false;
    }

    public Cluster(int type)
    {
        this.type = type;
    }

    public Cluster(GameObject obj)
    {
        this.Add(obj);
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

    public CameraController CC;
    // Start is called before the first frame update
    void Start()
    {
        FormClusters();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShowClusters();
        ShowClustersCenters();
    }

    public void Main()
    {
        MinDist = CC.CurrentZoom / 5;
        FormClusters();
    }

    public void FormClusters()
    {
        Items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < Items.Length; i++)
        {
            Cluster cluster = GetClusterOfObject(Items[i]);

            int objType = GetTypeOfObject(Items[i]);
            if(objType != -1)
            {
                for (int j = 0; j < Items.Length; j++)
                {
                    if(i != j)
                    {
                        if(objType == GetTypeOfObject(Items[j]))
                        {
                            float currentDist = Vector3.Distance(Items[i].transform.position, Items[j].transform.position);

                            Cluster jCluster = GetClusterOfObject(Items[j]);

                            if (currentDist <= MinDist)
                            {
                                if (cluster.Size >= jCluster.Size)
                                {
                                    jCluster.Remove(Items[j]);
                                    cluster.Add(Items[j]);

                                    if (jCluster.Empty())
                                        Clusters.Remove(jCluster);
                                }
                                else
                                {
                                    cluster.Remove(Items[i]);
                                    jCluster.Add(Items[i]);
                                    break;
                                }
                            }
                            else
                            {
                                if (cluster.Size >= jCluster.Size)
                                {
                                    if(cluster.Contains(Items[j]))
                                    {
                                        cluster.Remove(Items[j]);
                                        Clusters.Add(new Cluster(Items[j]));
                                    }
                                }
                                else
                                {
                                    jCluster.Remove(Items[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!Clusters.Contains(cluster) && cluster.Elements.Count > 0)
            {
                Clusters.Add(cluster);
            }

            if (cluster.Elements.Count == 0)
                Clusters.Remove(cluster);
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
        return new Cluster(obj);
    }

    public void ShowClustersCenters()
    {
        foreach(Cluster cluster in Clusters)
        {
            foreach (GameObject obj in cluster.Elements)
            {
                cluster.CalculateCenter();
                cluster.Label = Instantiate<GameObject>(Label, cluster.position, Quaternion.identity);
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
}
