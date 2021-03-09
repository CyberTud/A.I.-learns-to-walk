using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Brain : MonoBehaviour
{

    public GameObject creature;
    
    float walk = 1; 
    int deads;  
    int generation = 0;
    // Start is called before the first frame update

    List<GameObject> population = new List<GameObject>();
    private int populationSize = 100;
    private int alpha = 26;

    float[] score = new float[10000];
 
     struct legs
    {
        public float m, M, o, p;

    };
    legs[,] leg= new legs[10005, 2];

    bool[] death = new bool[10000];


    public void initiateLeg(int type, int index)
    {

        leg[index, type].m = UnityEngine.Random.Range(0.7f, 2.10f);
        leg[index, type].M = UnityEngine.Random.Range(0.7f, 2.10f);
        leg[index, type].o = UnityEngine.Random.Range(0.7f, 2.10f);
        leg[index, type].p = UnityEngine.Random.Range(0.1f, 3f);

        if (leg[index, type].m > leg[index, type].M)
        {
            float aux = leg[index, type].M;
            leg[index, type].M = leg[index, type].m;
            leg[index, type].m = aux;

        }

    }


    public float EvaluateAt (int i, int type, float time)
    {
        return (leg[i, type].M - leg[i, type].m) / 2 * (1 + Mathf.Sin((time + leg[i, type].o) * Mathf.PI * 2 / leg[i, type].p)) + leg[i, type].m;
    }


     

    void Start()
    {
        Debug.Log("Generation " + generation);
         for (int i = 1; i <= populationSize; i++)
        {
            Vector3 pos = new Vector3(1.5f, 1, 0);

            GameObject bos = Instantiate(creature, pos, Quaternion.identity);

            
            //Transform legup1pos = bos.transform.Find("legup1");
            //legup1pos.eulerAngles = new Vector3(0, 0, 0);

            population.Add(bos);

            initiateLeg(0, i);
            initiateLeg(1, i);
           
        }

        
    }

    public void Swap(ref float a, ref float b)
    {
        float temp = a;
        a = b;
        b = temp;
    }

    void swapStruct(int i, int j)
    {

        Swap(ref leg[i, 0].m, ref leg[j, 0].m);
        Swap(ref leg[i, 0].M, ref leg[j, 0].M);
        Swap(ref leg[i, 0].p, ref leg[j, 0].p);
        Swap(ref leg[i, 0].o, ref leg[j, 0].o);

        Swap(ref leg[i, 1].m, ref leg[j, 1].m);
        Swap(ref leg[i, 1].M, ref leg[j, 1].M);
        Swap(ref leg[i, 1].p, ref leg[j, 1].p);
        Swap(ref leg[i, 1].o, ref leg[j, 1].o);

    }


    void copyStruct(int i, int j)
    {
        leg[i, 0].m = leg[j, 0].m;
        leg[i, 0].M = leg[j, 0].M;
        leg[i, 0].p = leg[j, 0].p;
        leg[i, 0].o = leg[j, 0].o;

        leg[i, 1].m = leg[j, 1].m;
        leg[i, 1].M = leg[j, 1].M;
        leg[i, 1].p = leg[j, 1].p;
        leg[i, 1].o = leg[j, 1].o;

    }

    void sortCreatures()
    {

        for(int i = 1; i < populationSize; i++)
            for(int j = i + 1; j <= populationSize; j++)
            {
                if(score[i] < score[j])
                {

                    Swap(ref score[i],ref score[j]);
                    swapStruct(i, j);

                }

            }


    }

    
    void Mutate(int index, int type)
    {
        switch ( Random.Range(0,3+1) )
        {
            case 0:
            leg[index, type].m += Random.Range(-0.1f, 0.1f);
            leg[index, type].m = Mathf.Clamp(leg[index, type].m, 0.7f, 2.10f);
            break;
            case 1:
            leg[index, type].M += Random.Range(-0.1f, 0.1f);
            leg[index, type].M = Mathf.Clamp(leg[index, type].M, 0.7f, 2.10f);
            break;
            case 2:
            leg[index, type].p +=Random.Range(-0.25f, 0.25f);
            leg[index, type].p = Mathf.Clamp(leg[index, type].p, 0.7f, 2.10f);
            break;
            case 3:
            leg[index, type].o += Random.Range(-0.25f, 0.25f);
            leg[index, type].o = Mathf.Clamp(leg[index, type].o, 0.1f, 3f);
            break;
        }

    }
    void Mutation()
    {

        for(int i = alpha; i <= populationSize/2; i ++)
        {
            copyStruct(i, i - alpha + 1);

            if(UnityEngine.Random.Range(-1f, 1f) > 0)
                Mutate(i, 1);
            else 
                Mutate(i, 0);

        }

        for(int i = populationSize + 1; i <= populationSize; i++)
        {
            initiateLeg(i, 0);
            initiateLeg(i, 1);

        }

    }


    void changeOrder(int index, int order)
    {

        SpriteRenderer spriteBody = population[index].transform.Find("body").GetComponent<SpriteRenderer> ();
        SpriteRenderer spriteleg1 = population[index].transform.Find("leg1").GetComponent<SpriteRenderer> ();
        SpriteRenderer spriteleg2 = population[index].transform.Find("leg2").GetComponent<SpriteRenderer> ();

        spriteBody.sortingOrder = order;
        spriteleg1.sortingOrder = order;
        spriteleg2.sortingOrder = order;

    } 

    // Update is called once per frame
    void Update()
    {
        /*
         Transform leg1pos = creature.transform.Find("leg1"); 
        leg1pos.eulerAngles = new Vector3(0, 0, (float)walk);

        Transform leg2pos = creature.transform.Find("leg2"); 
        leg2pos.eulerAngles = new Vector3(0, 0, (float)walk);
        */
        GameObject laser = GameObject.FindGameObjectWithTag("laser");

        if(deads == populationSize)
        {   
            generation++;
            Debug.Log("Generation " + generation);
            deads = 0;
            population.Clear();
            Vector3 t = new Vector3(-25f,3,0);
            laser.transform.position = t;
            sortCreatures();
            Mutation();
            for (int i = 1; i <= populationSize; i++)
            {
                death[i] = false;
                Vector3 pos = new Vector3(1.5f, 1, 0);

                GameObject creature1 = Instantiate(creature, pos, Quaternion.identity);
                population.Add(creature1);
               // Debug.Log(score[i]);
              // changeOrder(i-1, 1);
                
            }
           //changeOrder(0, 3);

        }
        Vector3 temp = new Vector3(0.01f,0,0);
        laser.transform.position += temp;
        for(int i = 1; i <= populationSize; i++)
        {

            if(death[i]) continue;

            GameObject creature = population[i-1];
            GameObject body = creature.transform.Find("body").gameObject;


            SpringJoint2D leg1pos = creature.transform.Find("leg1").GetComponent<SpringJoint2D> ();
            leg1pos.distance = EvaluateAt(i, 0, Time.time);

            
            SpringJoint2D leg2pos = creature.transform.Find("leg2").GetComponent<SpringJoint2D> ();
            leg2pos.distance = EvaluateAt(i, 1, Time.time);


            Collider2D legCollider;
            Collider2D anotherCollider;
            Collider2D laserCollider;
            Collider2D bodyCollider;

            legCollider = creature.transform.Find("leg1").GetComponent<Collider2D> ();
            bodyCollider = creature.transform.Find("body").GetComponent<Collider2D> ();
            anotherCollider = GameObject.FindGameObjectWithTag("ground").GetComponent<Collider2D>();
            laserCollider = GameObject.FindGameObjectWithTag("laser").GetComponent<Collider2D>();

            

            if(bodyCollider.IsTouching(anotherCollider) || bodyCollider.IsTouching(laserCollider) || legCollider.IsTouching(laserCollider))
            {

                score[i] = population[i-1].transform.Find("body").transform.position.x;
                Destroy(population[i-1]);
                death[i] = true;
                deads++;
               // Debug.Log(score[i]);
            }


        }

      
    
    }
}
