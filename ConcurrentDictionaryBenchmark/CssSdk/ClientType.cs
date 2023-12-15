// <copyright file="ClientType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Contract
{
    using System;

    /// <summary>
    /// Sdk Client types 
    /// </summary>
    [Flags]
    public enum ClientType
    {
        Css = 1,
        Sigs = 2,
    }
}
