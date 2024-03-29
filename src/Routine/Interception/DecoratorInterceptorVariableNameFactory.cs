﻿namespace Routine.Interception;

internal class DecoratorInterceptorVariableNameFactory
{
    private static readonly object VARIABLE_NAME_LOCK = new();
    private static int _instanceCount;
    internal static string NextVariableName()
    {
        lock (VARIABLE_NAME_LOCK)
        {
            return "__decoratorVariable_" + _instanceCount++;
        }
    }
}
