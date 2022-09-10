﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Refactive.Internal;

internal static class ExceptionHelpers
{
    private static readonly Lazy<IExceptionServices> Services = new Lazy<IExceptionServices>(Initialize);

    [DoesNotReturn]
    public static void Throw(this Exception exception) => Services.Value.Rethrow(exception);

    private static IExceptionServices Initialize()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return PlatformEnlightenmentProvider.Current.GetService<IExceptionServices>() ?? new DefaultExceptionServices();
#pragma warning restore CS0618 // Type or member is obsolete
    }
}

/// <summary>
/// (Infrastructure) Services to rethrow exceptions.
/// </summary>
/// <remarks>
/// This type is used by the Rx infrastructure and not meant for public consumption or implementation.
/// No guarantees are made about forward compatibility of the type's functionality and its usage.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IExceptionServices
{
    /// <summary>
    /// Rethrows the specified exception.
    /// </summary>
    /// <param name="exception">Exception to rethrow.</param>
    [DoesNotReturn]
    void Rethrow(Exception exception);
}