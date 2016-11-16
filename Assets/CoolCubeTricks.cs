using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CoolCubeTricks : MonoBehaviour
{
//  Renderer ren;
    public float easeTime = 1f;

    Vector3 startPosition;
    public Transform endPoint;

    public AnimationCurve curve;

  void Start()
  {
      float startScale = 1f;
      float endScale = 4f;

      Waiters.Interpolate(1.2f, t => {
          transform.localScale = Vector3Lerp(
              startScale * Vector3.one,
              endScale * Vector3.one,
              Easing.EaseInOut(t, EaseType.Cubic)
          );
      })
      .ThenInterpolate(5f, t => {
          transform.localScale = Vector3Lerp(
              endScale * Vector3.one,
              startScale * Vector3.one,
              Easing.EaseInOut(t, EaseType.Cubic)
          );
      });

      Quaternion startRot = transform.localRotation;
      Quaternion endRot = Random.rotation;

      Waiters
        .Wait(1f)
        .ThenInterpolate(1.2f, t => {
          transform.localRotation = Quaternion.Lerp(
              startRot,
              endRot,
              Easing.EaseInOut(t, EaseType.Sine)
          );
      });

//    Debug.Log("hi i m cube :)");

//    ren = GetComponent<Renderer>();
  }

  public static float Lerp(float a, float b, float t)
  {
      return t * b + (1 - t) * a;
  }

  public static Vector3 Vector3Lerp(Vector3 a, Vector3 b, float t)
  {
      return new Vector3(
        t * b.x + (1 - t) * a.x,
        t * b.y + (1 - t) * a.y,
        t * b.z + (1 - t) * a.z
      );
  }

    Waiter positionWaiter; 
  
  void Update()
  {
  
      if(Input.GetKeyDown(KeyCode.R)) {
          SceneManager.LoadScene(0);
      }

      if(Input.GetKeyDown(KeyCode.Space)) {
          if(positionWaiter != null) {
              Destroy(positionWaiter);
          }

          endPoint.localPosition = new Vector3(
              Random.Range(-5f, 5f),
              Random.Range(-5f, 5f),
              Random.Range(-5f, 5f)
          );

          startPosition = transform.localPosition;

          positionWaiter = Waiters.Interpolate(3f, t => {
              transform.localPosition = Vector3Lerp(
                  startPosition,
                  endPoint.localPosition,
                  Easing.Ease(t, curve)
              );
          });
      }


//    ren.material.color = new Color(Random.value, Random.value, Random.value);
  }
}
