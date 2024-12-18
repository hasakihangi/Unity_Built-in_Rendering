using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    public Material material;

    public float dissolveAmount;
    public float rimIntensity;
    
    private int _DissolveAmount;
    private int _RimIntensity;
    
    // Start is called before the first frame update
    void Start()
    {
        _DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        _RimIntensity = Shader.PropertyToID("_RimIntensity");
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat(_DissolveAmount, dissolveAmount);
        material.SetFloat(_RimIntensity, rimIntensity);
    }
}
