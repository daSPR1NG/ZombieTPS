using UnityEngine;
using System;

namespace Khynan_Coding
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MaxAttribute : PropertyAttribute
    {
        public readonly float MaxValue;
        
        public MaxAttribute(float maxValue)
        {
            MaxValue = maxValue;
        }
    }
}