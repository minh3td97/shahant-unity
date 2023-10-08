using Shahant;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestView : View
{
    [SerializeField] string _testString;
    public string TestString => _testString;
    public int CountValue { get; }

}
