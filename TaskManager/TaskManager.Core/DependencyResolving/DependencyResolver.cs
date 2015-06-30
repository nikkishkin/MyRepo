﻿using System;
using System.Collections.Generic;

namespace TaskManager.Core.DependencyResolving
{
    public static class DependencyResolver
    {
        private static readonly Dictionary<Type, Func<object>> RegisteredTypes = new Dictionary<Type, Func<object>>();

        public static void Register<TSource>(Func<object> factoryFunc)
        {
            RegisteredTypes.Add(typeof(TSource), factoryFunc);
        }

        public static TSource Resolve<TSource>()
        {
            return (TSource)RegisteredTypes[typeof(TSource)]();
        }
    }
}
