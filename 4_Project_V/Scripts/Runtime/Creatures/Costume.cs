using System;
using System.Collections.Generic;
using UnityEngine;

public class Costume : MonoBehaviour
{
    [SerializeField] private Category category;
    [SerializeField] private bool isNone;

    public Category Category => category;
    public bool IsNone => isNone;
}