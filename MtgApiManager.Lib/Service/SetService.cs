﻿// <copyright file="SetService.cs">
//     Copyright (c) 2014. All rights reserved.
// </copyright>
// <author>Jason Regnier</author>
namespace MtgApiManager.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Dto.Set;
    using Model;
    using MtgApiManager.Lib.Core;
    using Utility;

    /// <summary>
    /// Object representing a MTG set.
    /// </summary>
    public class SetService
        : ServiceBase<SetService, Set>, IMtgQueryable<SetService, SetQueryParameter>
    {
        /// <summary>
        /// The list of queries to apply.
        /// </summary>
        private NameValueCollection _whereQueries = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetService"/> class. Defaults to version 1.0 of the API.
        /// </summary>
        /// <param name="serviceAdapter">The service adapter used to interact with the MTG API.</param>
        public SetService(IMtgApiServiceAdapter serviceAdapter)
            : this(serviceAdapter, ApiVersion.V1_0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetService"/> class. Defaults to version 1.0 of the API.
        /// </summary>
        public SetService()
            : this(new MtgApiServiceAdapter(), ApiVersion.V1_0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetService"/> class.
        /// </summary>
        /// <param name="serviceAdapter">The service adapter used to interact with the MTG API.</param>
        /// <param name="version">The version of the API</param>
        public SetService(IMtgApiServiceAdapter serviceAdapter, ApiVersion version)
            : base(serviceAdapter, version, ApiEndPoint.Sets)
        {
            this._whereQueries = new NameValueCollection();
        }

        /// <summary>
        /// Gets all the <see cref="TModel"/> defined by the query parameters.
        /// </summary>
        /// <returns>A <see cref="Exceptional{List{Set}}"/> representing the result containing all the items.</returns>
        public override Exceptional<List<Set>> All()
        {
            try
            {
                var query = this.BuildUri(this._whereQueries);
                var rootSetList = this.CallWebServiceGet<RootSetListDto>(query).Result;

                return Exceptional<List<Set>>.Success(this.MapSetsList(rootSetList));
            }
            catch (AggregateException ex)
            {
                return Exceptional<List<Set>>.Failure(ex.Flatten().InnerException);
            }
        }

        /// <summary>
        /// Gets all the <see cref="TModel"/> defined by the query parameters.
        /// </summary>
        /// <returns>A <see cref="Exceptional{List{Set}}"/> representing the result containing all the items.</returns>
        public async override Task<Exceptional<List<Set>>> AllAsync()
        {
            try
            {
                var query = this.BuildUri(this._whereQueries);
                var rootSetList = await this.CallWebServiceGet<RootSetListDto>(query);

                return Exceptional<List<Set>>.Success(this.MapSetsList(rootSetList));
            }
            catch (Exception ex)
            {
                return Exceptional<List<Set>>.Failure(ex);
            }
        }

        /// <summary>
        /// Find a specific set by its set code.
        /// </summary>
        /// <param name="code">The set code to query for.</param>
        /// <returns>A <see cref="Exceptional{Set}"/> representing the result containing a <see cref="Set"/> or an exception.</returns>
        public Exceptional<Set> Find(string code)
        {
            try
            {
                var rootSet = this.CallWebServiceGet<RootSetDto>(this.BuildUri(code)).Result;
                var model = new Set(rootSet.Set);

                return Exceptional<Set>.Success(model);
            }
            catch (AggregateException ex)
            {
                return Exceptional<Set>.Failure(ex.Flatten().InnerException);
            }
        }

        /// <summary>
        /// Find a specific card by its set code.
        /// </summary>
        /// <param name="code">The set code to query for.</param>
        /// <returns>A <see cref="Exceptional{Set}"/> representing the result containing a <see cref="Set"/> or an exception.</returns>
        public async Task<Exceptional<Set>> FindAsync(string code)
        {
            try
            {
                var rootSet = await this.CallWebServiceGet<RootSetDto>(this.BuildUri(code));
                var model = new Set(rootSet.Set);

                return Exceptional<Set>.Success(model);
            }
            catch (Exception ex)
            {
                return Exceptional<Set>.Failure(ex);
            }
        }

        /// <summary>
        /// Adds a query parameter.
        /// </summary>
        /// <typeparam name="U">The type of property to add the query for.</typeparam>
        /// <param name="property">The property to add the query for.</param>
        /// <param name="value">The value of the query.</param>
        /// <returns>The instance of its self with the new query parameter.</returns>
        public SetService Where<U>(Expression<Func<SetQueryParameter, U>> property, string value)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            MemberExpression expression = property.Body as MemberExpression;
            var queryName = QueryUtility.GetQueryPropertyName<SetQueryParameter>(expression.Member.Name);
            this._whereQueries[queryName] = value;

            return this;
        }

        /// <summary>
        /// Maps a collection of set DTO objects to the set model.
        /// </summary>
        /// <param name="setListDto">The list of sets DTO objects.</param>
        /// <returns>A list of set models.</returns>
        private List<Set> MapSetsList(RootSetListDto setListDto)
        {
            if (setListDto == null)
            {
                throw new ArgumentNullException("setListDto");
            }

            if (setListDto.Sets == null)
            {
                return null;
            }

            return setListDto.Sets
                .Select(x => new Set(x))
                .ToList();
        }
    }
}