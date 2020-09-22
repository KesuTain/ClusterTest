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
    private Vector3 center;
    
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
        center = Vector3.zero;
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
    public GameObject FireWood;
    public GameObject Berries;
    public GameObject Flex;

    public CameraController CC;
    void Start()
    {
        FormClusters();
    }

    void FixedUpdate()
    {
        ShowClusters();
        DrawClustersCenters();
    }

    public void Main()
    {
        MinDist = CC.CurrentZoom / 5;
        DeleteAllCenters();
        FormClusters();
        ShowClustersCenters();
    }

    public void FormClusters()
    {
        Items = GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < Items.Length; i++)
        {
            Cluster cluster = GetClusterOfObject(Items[i]);

            cluster.type = GetTypeOfObject(Items[i]);
            if(cluster.type != -1)
            {
                for (int j = 0; j < Items.Length; j++)
                {
                    if(i != j)
                    {
                        if(cluster.type == GetTypeOfObject(Items[j]))
                        {
                            float currentDist = Vector3.Distance(Items[i].transform.position, Items[j].transform.position);

                            Cluster jCluster = GetClusterOfObject(Items[j]);

                            //Если дистанция между объектами меньше минимальной дистанции.
                            if (currentDist <= MinDist)
                            {
                                //Если размер кластера больше размера соседнего кластера того же типа, 
                                //то перенести в свой кластер.
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
                                //Если объектов больше в своём кластере, чем в соседнем.
                                if (cluster.Size >= jCluster.Size)
                                {
                                    //Если в соседнем кластере есть тот же объект, что и своём, то удалить этот объект
                                    //И создать новый кластер.
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

            //Сформировать кластер если его ещё нет и элементов больше 0
            if (!Clusters.Contains(cluster) && cluster.Elements.Count > 0)
            {
                Clusters.Add(cluster);
            }

            //Если элементов в кластере нет, то удалить его
            if (cluster.Elements.Count == 0)
                Clusters.Remove(cluster);
        }
        CheckSingleClasters();
    }

    public void CheckSingleClasters()
    {
        //Проверка одиночных кластеров
        foreach (Cluster cluster in Clusters)
        {
            if (cluster.Single() == true)
            {
                switch (cluster.Elements[0].GetComponent<Renderer>().material.name)
                {
                    case "Item1 (Instance)":
                        cluster.type = 0;
                        break;
                    case "Item2 (Instance)":
                        cluster.type = 1;
                        break;
                    case "Item3 (Instance)":
                        cluster.type = 2;
                        break;
                    default:
                        break;
                }
            }
        }
    }
    //Проверяем есть ли объект в какой либо кластере.
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
            }
            switch (cluster.type)
            {
                case 0:
                    cluster.Label = Instantiate(FireWood, cluster.position + Vector3.up, Quaternion.identity);
                    break;
                case 1:
                    cluster.Label = Instantiate(Flex, cluster.position + Vector3.up, Quaternion.identity);
                    break;
                case 2:
                    cluster.Label = Instantiate(Berries, cluster.position + Vector3.up, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }

    void DrawClustersCenters()
    {
        foreach (Cluster cluster in Clusters)
        {
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

    void DeleteAllCenters()
    {
        GameObject[] Centers;
        Centers = GameObject.FindGameObjectsWithTag("Label");
        for (int i = 0; i < Centers.Length; i++)
        {
            Destroy(Centers[i]);
        }
    }
}
