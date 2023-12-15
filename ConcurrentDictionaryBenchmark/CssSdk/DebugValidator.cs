// <copyright file="DebugValidator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Validation methods to be used in Debug build.
    /// </summary>
    public static class DebugValidator
    {
        /// <summary>
        /// Throws if the argument is null.
        /// </summary>
        /// <param name="name">The argument name</param>
        /// <param name="arg">The argument itself</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if the arg is null.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1801 // Review unused parameters
        public static void ThrowIfNull(string name, object arg)
#pragma warning disable CA1801 // Review unused parameters
        {
#if DEBUG
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
#endif
        }
    }
}
