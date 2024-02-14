using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCircuitException : IncorrectCircuitException
{
    public ShortCircuitException() : base("Short circuit detected - Cannot be calculated") { }
}
