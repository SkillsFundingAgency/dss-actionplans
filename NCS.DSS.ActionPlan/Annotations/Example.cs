﻿using System;

namespace NCS.DSS.ActionPlan.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Example : Attribute
    {
        public string Description { get; set; }
    }
}
