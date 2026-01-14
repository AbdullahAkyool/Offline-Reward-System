using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager Instance { get; private set; }

    private readonly List<IUpdateable> activeUpdatebles = new List<IUpdateable>(); //aktif updateler
    private readonly List<IUpdateable> pendingUpdateables = new List<IUpdateable>(); //eklenmeyi bekleyenler
    private readonly List<IUpdateable> pendingRemoves = new List<IUpdateable>(); //kaldirilmayi bekleyenler

    private bool _isUpdating;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(IUpdateable updateable)
    {
        if (updateable == null) return;

        if (_isUpdating) 
        {
            if (!pendingUpdateables.Contains(updateable))
                pendingUpdateables.Add(updateable); //for ile kontrol edildigi icin ayni anda eklenmez
        }
        else
        {
            if (!activeUpdatebles.Contains(updateable))
                activeUpdatebles.Add(updateable);
        }
    }

    public void Unregister(IUpdateable updateable)
    {
        if (updateable == null) return;

        if (_isUpdating)
        {
            if (!pendingRemoves.Contains(updateable))
                pendingRemoves.Add(updateable);
        }
        else
        {
            activeUpdatebles.Remove(updateable);
        }
    }

    private void Update()
    {
        _isUpdating = true;

        float deltaTime = Time.deltaTime;

        for (int i = 0; i < activeUpdatebles.Count; i++)
        {
            activeUpdatebles[i].OnUpdate(deltaTime);
        }

        _isUpdating = false;

        ProcessPendingOperations(); //ekleme ve cikarma islemlerini yap
    }

    private void ProcessPendingOperations()
    {
        foreach (var item in pendingUpdateables)
        {
            if (!activeUpdatebles.Contains(item))
                activeUpdatebles.Add(item);
        }
        pendingUpdateables.Clear();

        foreach (var item in pendingRemoves)
        {
            activeUpdatebles.Remove(item);
        }
        pendingRemoves.Clear();
    }
}
