using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CoolCubeTricks : MonoBehaviour
{
  Renderer ren;

  void Start()
  {
    Debug.Log("hi i m cube :)");

    ren = GetComponent<Renderer>();
  }
  
  void Update()
  {
    ren.material.color = new Color(Random.value, Random.value, Random.value);
  }
}
