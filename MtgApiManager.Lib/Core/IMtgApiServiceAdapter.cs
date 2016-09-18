﻿// <copyright file="IMtgService.cs" company="Team7 Productions">
//     Copyright (c) 2014. All rights reserved.
// </copyright>
// <author>Jason Regnier</author>
namespace MtgApiManager.Lib.Core
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Used to make web service calls.
    /// </summary>
    public interface IMtgApiServiceAdapter
    {
        /// <summary>
        /// Do a Web Get for the given request Uri .
        /// </summary>
        /// <param name="requestUri">The URL to call.</param>
        /// <returns>The response string.</returns>
        Task<T> WebGetAsync<T>(Uri requestUri) 
            where T : Dto.MtgResponseBase;
    }
}