using System;
using UnityEngine;
using System.Collections.Generic;
namespace Dexmo.Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class EnumFlagsAttribute : PropertyAttribute { }
    public class EnumFlagsHelper
    {
        public List<T> GetActiveEnumElements<T>(int _enmuFlagsIntValue)
        {
            List<T> _result = new List<T>();
            return _result;
        }
    }
}