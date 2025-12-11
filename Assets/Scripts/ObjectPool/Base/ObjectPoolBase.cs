using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ObjectPoolBase : MonoBehaviour
{
    public abstract bool IsEmpty();
    public abstract ObjectBase CreateNewPooledItem();
    public abstract void Enqueue(ObjectBase item);
    public abstract ObjectBase Dequeue();
}

public class ObjectPoolBase<T> : ObjectPoolBase where T : ObjectBase
{
    [SerializeField] protected T prefabInst;
    [SerializeField] protected bool canContainMultipleObjects = false;

    [Tooltip("If canContainMultipleObjects initialize this")]
    [SerializeField] protected T[] prefabInstances;

    [SerializeField] protected Transform prefabsParent;
    [SerializeField] protected int initialPoolCount;

    protected Queue<T> queue = new Queue<T>();

    public virtual void InitPoolFirstTime()
    {
        T itemInst = null;

        if (!canContainMultipleObjects)
        {
            for (int i = 0; i < initialPoolCount; i++)
            {
                itemInst = (T)CreateNewPooledItem();
                Enqueue(itemInst);
            }
        }
        else
        {
            initialPoolCount *= prefabInstances.Count();
            var indexer = 0;

            for (int i = 0; i < initialPoolCount; i++)
            {
                if (indexer >= prefabInstances.Count()) indexer = 0;

                prefabInst = prefabInstances[indexer++];

                itemInst = (T)CreateNewPooledItem();
                Enqueue(itemInst);
            }
        }
    }

    public override ObjectBase CreateNewPooledItem()
    {
        if (canContainMultipleObjects)
        {
            prefabInst = prefabInstances[Random.Range(0, prefabInstances.Length)];
        }

        T inst = Instantiate(prefabInst);
        inst.gameObject.SetActive(false); // enqueue these objects later
        inst.transform.parent = prefabsParent;
        
        return inst;
    }

    public override void Enqueue(ObjectBase item)
    {
        queue.Enqueue((T)item);
    }

    public override ObjectBase Dequeue()
    {
        return queue.Dequeue();
    }

    public override bool IsEmpty()
    {
        return queue.Count < 1;
    }

    public virtual void ClearPool()
    {
        queue.Clear();
    }

    
}
