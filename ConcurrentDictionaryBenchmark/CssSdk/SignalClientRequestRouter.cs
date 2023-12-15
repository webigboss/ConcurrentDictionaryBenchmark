// <copyright file="SignalClientRequestRouter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using Microsoft.Substrate.CompactSignalStore.Item.DataModels;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// Implementation of <see cref="ICssSdkRequestRouter"/> that routes all requests per query filter.
    /// </summary>
    public class SignalClientRequestRouter : ICssSdkRequestRouter
    {
        /// <summary>
        /// Edm model for parsing the filter.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// Edm model entity created from the <see cref="Signal"/> class, for parsing the filter.
        /// </summary>
        private readonly IEdmEntitySet signalEntitySet;

        /// <summary>
        /// Cut off date for routing requests to SIGS.
        /// it means queries before (or partially overlapping) this date will be routed to SIGS.
        /// </summary>
        private readonly DateTimeOffset routeToSigsCutOffDate = SdkConstants.DefaultRouteToSigsCutOffDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalClientRequestRouter"/> class.
        /// </summary>
        public SignalClientRequestRouter()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Signal>(nameof(Signal))
                .EntityType.HasKey(x => x.Id)
                .Filter(nameof(Signal.StartTime), nameof(Signal.EndTime), nameof(Signal.SignalType)); // TODO: figure out why this doesn't work.
                
            this.model = builder.GetEdmModel();
            this.signalEntitySet = this.model.FindDeclaredEntitySet(nameof(Signal));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalClientRequestRouter"/> class with a custom cut off date.
        /// this is mainly for functional test, consumer of the client doesn't need to provide this date.
        /// </summary>
        /// <param name="defaultRouteToSigsCutOffDate"></param>
        /// 
        internal SignalClientRequestRouter(DateTimeOffset defaultRouteToSigsCutOffDate) : this()
        {
            this.routeToSigsCutOffDate = defaultRouteToSigsCutOffDate;
        }

        /// <inheritdoc/>
        public RoutingResult Route(string filter, ICssSdkRequestContext requestContext)
        {
            var queryOptionParser = GetQueryOptionParser(filter);

            try
            {
                var filterClause = queryOptionParser.ParseFilter();
                var cssQueryOptions = new CssODataQueryOptionsConverter().GetCssQueryOptions(filterClause);
                cssQueryOptions.Validate();

                var signalTypesToSigs = new List<SignalType>();
                var signalTypesToCss = new List<SignalType>();

                foreach (var signalType in cssQueryOptions.SignalTypes)
                {
                    if (requestContext.SignalTypesToSigs != null && requestContext.SignalTypesToSigs.Contains(signalType))
                    {
                        signalTypesToSigs.Add(signalType);
                        continue;
                    }

                    if (CanCssServe(signalType, cssQueryOptions.StartFilter.DateTime, requestContext.DeploymentRing))
                    {
                        signalTypesToCss.Add(signalType);
                    }
                    else
                    {
                        signalTypesToSigs.Add(signalType);
                    }
                }

                if (signalTypesToSigs.Count > 0 && signalTypesToCss.Count > 0)
                {
                    return new RoutingResult
                    {
                        To = ClientType.Sigs | ClientType.Css,
                        SplitFilters = GetSplitFilters(cssQueryOptions: cssQueryOptions, signalTypesToCss: signalTypesToCss, signalTypesToSigs: signalTypesToSigs),
                        Reason = "Part of signal types are configured routing to Sigs, Split the query and merging results later."
                    };
                }
                else if (signalTypesToSigs.Count > 0 && signalTypesToCss.Count == 0)
                {
                    return new RoutingResult
                    {
                        To = ClientType.Sigs,
                        Reason = "Routing all signal types to Sigs, either the signal type(s) are configured to be routed to Sigs, or the date range cannot be served by Css."
                    };
                }
                else
                {
                    return new RoutingResult
                    {
                        To = ClientType.Css,
                        Reason = "Routing all signal types to Css."
                    };
                }
            }
            catch (ArgumentException ex) 
            {
                // CSS does not support the filter, so we will route to SIGS
                return new RoutingResult { To = ClientType.Sigs, Reason = $"Fallback to Sigs due to {nameof(ArgumentException)}: {ex.Message}" };
            }
        }

        /// <summary>
        /// check if the query can be served by CSS based of the route to sigs cut off date per signal type per ring.
        /// </summary>
        /// <param name="cssQueryOptions"></param>
        /// <param name="deploymentRing"></param>
        /// <returns></returns>
        private bool IsQueryServedByCss(ICssQueryOptions cssQueryOptions, string deploymentRing)
        {
            return cssQueryOptions.SignalTypes.All(
                signalType => cssQueryOptions.StartFilter.DateTime >= SdkHelper.GetRouteToSigsCutoffDateByRingBySignalType(deploymentRing, signalType, this.routeToSigsCutOffDate));
        }

        /// <summary>
        /// check if the signal type can be served by CSS based of the route to sigs cut off date per ring.
        /// </summary>
        /// <param name="signalType"></param>
        /// <param name="startTime"></param>
        /// <param name="deploymentRing"></param>
        /// <returns></returns>
        private bool CanCssServe(SignalType signalType, DateTimeOffset startTime, string deploymentRing)
        {
            return startTime >= SdkHelper.GetRouteToSigsCutoffDateByRingBySignalType(deploymentRing, signalType, this.routeToSigsCutOffDate);
        }

        /// <summary>
        /// Split the query options for MarkMessageAsRead to Sigs and the rest to Css in string literal format.
        /// </summary>
        /// <param name="cssQueryOptions">css query options</param>
        /// <param name="signalTypesToCss">signal types to css</param>
        /// <param name="signalTypesToSigs">signal types to sigs</param>
        /// <returns></returns>
        private IReadOnlyDictionary<ClientType, string> GetSplitFilters(ICssQueryOptions cssQueryOptions, IEnumerable<SignalType> signalTypesToCss, IEnumerable<SignalType> signalTypesToSigs)
        {
            return new Dictionary<ClientType, string>()
            {
                { ClientType.Sigs, SdkHelper.GetFilterLiteralFromCssQueryOptions(cssQueryOptions, signalTypesToSigs) },
                { ClientType.Css, SdkHelper.GetFilterLiteralFromCssQueryOptions(cssQueryOptions, signalTypesToCss) },
            };
        }

        private ODataQueryOptionParser GetQueryOptionParser(string filter)
        {
            var queryOptionParser = new ODataQueryOptionParser(
                this.model,
                new ODataPath(new EntitySetSegment(this.signalEntitySet)),
                new Dictionary<string, string> { { "$filter", filter } });

            //enable case insensitive
            queryOptionParser.Resolver.EnableCaseInsensitive = true;

            return queryOptionParser;
        }
    }
}
