using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncorrectCircuitException : Exception
{
    public IncorrectCircuitException(string message) : base("Incorrect circuit : " + message) { }
}
