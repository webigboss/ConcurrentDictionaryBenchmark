// <copyright file="SdkHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Build.Tasks;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;
    using Microsoft.Substrate.CompactSignalStore.Sdk.Contract;

    /// <summary>
    /// Helper class with helper methods
    /// </summary>
    internal static class SdkHelper
    {
        /// <summary>
        /// Add common headers to the request message
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="requestMessage"></param>
        internal static void AddCommonHeaders(ICssSdkRequestContext requestContext, HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add(SdkConstants.AcceptHeaderKey, SdkConstants.AcceptHeaderJsonValue);
            requestMessage.Headers.Add(SdkConstants.AuthorizationHeaderKey, requestContext.Token);
            requestMessage.Headers.Add(SdkConstants.AnchorMailboxHeaderKey, requestContext.GetRouteKey());

            if (requestContext.AdditionalHeaders != null)
            {
                foreach (var header in requestContext.AdditionalHeaders)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Ensure the response is successful and throw exception with headers and content
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static async ValueTask EnsureSuccessStatusCodeAndThrowHeaderAndContent(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var headers = JsonSerializer.Serialize(response.Headers);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new HttpRequestException($"Request failed with status code: {response.StatusCode}, headers: {headers}, content: {content}");
        }

        /// <summary>
        /// Get the cut off date for routing requests to SIGS per deployment ring and signal type.
        /// </summary>
        /// <param name="deploymentRing"></param>
        /// <param name="signalType"></param>
        /// <param name="defaultRouteToSigsCutOffDate"></param>
        /// <returns></returns>
        internal static DateTimeOffset GetRouteToSigsCutoffDateByRingBySignalType(string deploymentRing, SignalType signalType, DateTimeOffset defaultRouteToSigsCutOffDate)
        {
            if (SdkConstants.RouteToSigsCutOffDateByRingBySignalType.TryGetValue(deploymentRing, out var routeToSigsCutOffDateByRing))
            {
                if (routeToSigsCutOffDateByRing.TryGetValue(signalType, out var routeToSigsCutOffDateByRingBySignalType))
                {
                    return routeToSigsCutOffDateByRingBySignalType;
                }

                return defaultRouteToSigsCutOffDate;
            }

            return defaultRouteToSigsCutOffDate;
        }

        /// <summary>
        /// Get the filter literal of given css query options.
        /// sample: (SignalType eq 'ViewMessage' or SignalType eq 'FileAccessed') and StartTime ge 2023-06-13T00:00:00Z and EndTime le 2023-07-01T00:00:00Z
        /// </summary>
        /// <param name="cssQueryOptions">css query option</param>
        /// <param name="overwriteSignalTypes">optional signal types to overwrite the given <paramref name="cssQueryOptions.SignalTypes"/></param>
        /// <returns>filter literal</returns>
        internal static string GetFilterLiteralFromCssQueryOptions(ICssQueryOptions cssQueryOptions, IEnumerable<SignalType> overwriteSignalTypes = null)
        {
            var signalTypes = overwriteSignalTypes != null ? overwriteSignalTypes : cssQueryOptions.SignalTypes;
            var signalTypesLiteral = $"({string.Join(" or ", signalTypes.Select(t => $"{nameof(SignalType)} eq '{t}'"))})";
            var startFilterLiteral = $"{cssQueryOptions.StartFilter.FilterProperty} {GetComparisonOperatorLiteral(cssQueryOptions.StartFilter.ComparisonOperator)} {cssQueryOptions.StartFilter.DateTime:yyyy-MM-dd'T'HH:mm:ss'Z'}";
            var endFilterLiteral = cssQueryOptions.EndFilter != null ?
                $"{cssQueryOptions.EndFilter.FilterProperty} {GetComparisonOperatorLiteral(cssQueryOptions.EndFilter.ComparisonOperator)} {cssQueryOptions.EndFilter.DateTime:yyyy-MM-dd'T'HH:mm:ss'Z'}"
                : string.Empty;

            return string.Join(" and ", new[] { signalTypesLiteral, startFilterLiteral, endFilterLiteral }.Where(l => !string.IsNullOrEmpty(l)));
        }

        private static string GetComparisonOperatorLiteral(ComparisonOperator comparisonOperator)
        {
            return comparisonOperator switch
            {
                ComparisonOperator.GreaterThanOrEqual => "ge",
                ComparisonOperator.LessThanOrEqual => "le",
                _ => "ge",
            };
        }   
    }
}
