using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

//MonoBehaviour를 상속받지 않는 클래스에서 코루틴을 사용할 수 있도록 도와주는 클래스
//단점:한번에 여러개의 코루틴을 시작할 수 없을 수도 있음
public class CoroutineHandler : MonoBehaviour
{
    IEnumerator enumerator = null;

    private void Coroutine(IEnumerator coro)
    {
        enumerator = coro;
        StartCoroutine(coro);
    }

    void Update()
    {
        if (enumerator != null)
        {
            if (enumerator.Current == null)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Stop()
    {
        StopCoroutine(enumerator.ToString());
        Destroy(gameObject);
    }

    public static CoroutineHandler Start_Coroutine(IEnumerator coro)
    {
        GameObject obj = new GameObject("CoroutineHandler");
        CoroutineHandler handler = obj.AddComponent<CoroutineHandler>();
        if (handler)
        {
            handler.Coroutine(coro);
        }
        return handler;
    }

}

// reference
public class A
{
    public void Init()
    {
        CoroutineHandler.Start_Coroutine(DestFunc());
    }

    IEnumerator DestFunc()
    {
        yield return new WaitForSeconds(0.3f);

        yield return null;

    }

}

//다른 방법의 코루틴핼퍼 
public class CoroutineHelper : MonoBehaviour
{
    private static MonoBehaviour monoInstance;

    [RuntimeInitializeOnLoadMethod]
    private static void Initializer()
    {
        monoInstance = new GameObject($"[{nameof(CoroutineHelper)}]").AddComponent<CoroutineHelper>();
        DontDestroyOnLoad(monoInstance.gameObject);
    }

    public new static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return monoInstance.StartCoroutine(coroutine);
    }

    public new static void StopCoroutine(Coroutine coroutine)
    {
        monoInstance.StopCoroutine(coroutine);
    }
}

// reference
public class B
{
    Coroutine cor;
    void OnEnable() => cor = CoroutineHelper.StartCoroutine(DestFunc());
    void OnDisable() => CoroutineHelper.StopCoroutine(cor);

    public void Init()
    {
        cor =  CoroutineHelper.StartCoroutine(DestFunc());
    }

    IEnumerator DestFunc()
    {
        yield return new WaitForSeconds(0.3f);

        yield return null;
        
    }
    public void Stop()
    {
        CoroutineHelper.StopCoroutine(cor);
    }

}

public class MutableWaitForSeconds : CustomYieldInstruction
{
    private float endTime;

    public MutableWaitForSeconds() : this(0f) { }
    public MutableWaitForSeconds(float seconds) => Reset(seconds);

    public void Reset(float seconds) => this.endTime = Time.time + seconds;

    public override bool keepWaiting => Time.time < endTime;
}