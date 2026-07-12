using System;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SearchableEnumAttribute : PropertyAttribute
    {
    }
}