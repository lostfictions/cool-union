using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

//Adapted from Renaud's handy waiters

//TODO: option to use fixed update?

public class Waiter : MonoBehaviour
{
    //A public friendly name or synopsis so we can look at it in the inspector.
    public string description = "";

    Queue<Task> taskQueue = new Queue<Task>();
    Action onDestroy = () => { };
    Func<bool> isCancelled = () => false;

    void Update()
    {
        Task task = taskQueue.Peek();
        task.timeWaited += Time.deltaTime;
        if(!isCancelled()) {
            task.onTick(task.timeWaited);

            if(task.condition(task.timeWaited)) {
                task.Cleanup();
                if(taskQueue.Count > 1) {
                    var t = taskQueue.Dequeue();
                    Waiters.taskCache.Return(t);
                }
                else {
                    Destroy(this);
                }
            }
        }
        else {
            task.Cleanup();
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        onDestroy();
    }

    public Waiter Cleanup(Action action)
    {
        Task t = taskQueue.Peek();
        t.onCleanup = action;

        return this;
    }

    public Waiter Then(Action action)
    {
        Task t = Waiters.taskCache.Take();
        t.Reset(_ => true, _ => action());

        taskQueue.Enqueue(t);

        return this;
    }

    public Waiter ThenDoUntil(Func<float, bool> cond, Action<float> action)
    {
        Task t = Waiters.taskCache.Take();
        t.Reset(cond, action);

        taskQueue.Enqueue(t);

        return this;
    }

    public Waiter ThenWait(float durationSeconds)
    {
        Task t = Waiters.taskCache.Take();
        t.Reset(waited => waited > durationSeconds, null);

        taskQueue.Enqueue(t);

        return this;
    }

    public Waiter ThenInterpolate(float durationSeconds, Action<float> action)
    {
        Task t = Waiters.taskCache.Take();
        t.Reset(
            waited => waited > durationSeconds,
            waited => action(Mathf.Clamp01(waited / durationSeconds))
        );

        taskQueue.Enqueue(t);

        return this;
    }

    public Waiter OnDestroy(Action action)
    {
        onDestroy = action;

        return this;
    }

    public Waiter CancelledWhen(Func<bool> predicate)
    {
        isCancelled = predicate;

        return this;
    }

    public Waiter Description(string description)
    {
        this.description = description;

        return this;
    }
}

public static class Waiters
{
    internal static readonly Pool<Task> taskCache = new Pool<Task>();

    static GameObject globalWaiter;

    public static GameObject GlobalWaiter
    {
        get
        {
            if(!globalWaiter) {
                globalWaiter = new GameObject("globalWaiter");
            }
            return globalWaiter;
        }
    }

    public static Waiter Wait(float secondsToWait, GameObject attachTo = null)
    {
        Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
        return w.ThenWait(secondsToWait);
    }

    public static Waiter DoUntil(Func<float, bool> condition, Action<float> action, GameObject attachTo = null)
    {
        Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
        return w.ThenDoUntil(condition, action);
    }

    //interpolatorAction parameter will always go from 0 to 1 (time waited / interpolation duration)
    public static Waiter Interpolate(
        float durationSeconds,
        Action<float> interpolatorAction,
        GameObject attachTo = null)
    {
        Waiter w = (attachTo ?? GlobalWaiter).AddComponent<Waiter>();
        return w.ThenInterpolate(durationSeconds, interpolatorAction);
    }
}

internal class Task
{
    internal Func<float, bool> condition;
    internal Action<float> onTick;
    internal Action onCleanup;

    internal float timeWaited;

    internal void Reset(Func<float, bool> condition, Action<float> onTick)
    {
        this.timeWaited = 0;
        this.onCleanup = null;
        this.condition = condition;
        this.onTick = onTick ?? (_ => { });
    }

    internal void Cleanup()
    {
        if(onCleanup != null) {
            onCleanup();
        }
    }
}
