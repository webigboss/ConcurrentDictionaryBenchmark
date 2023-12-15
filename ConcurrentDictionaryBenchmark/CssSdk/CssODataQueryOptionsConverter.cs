// <copyright file="CssODataQueryOptionsConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.Substrate.CompactSignalStore.Item.DataModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNet.OData.Query;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts;
    using Microsoft.Substrate.CompactSignalStore.Item.Contracts.Signals;

    /// <summary>
    /// Converts OData query options to CssQueryOptions instance
    /// </summary>
    public class CssODataQueryOptionsConverter
    {
        /// <summary>
        /// StartFilter
        /// </summary>
        private DateTimeFilter startFilter;

        /// <summary>
        /// EndFilter
        /// </summary>
        private DateTimeFilter endFilter;

        /// <summary>
        /// SignalTypes
        /// </summary>
        private HashSet<SignalType> signalTypes = new HashSet<SignalType>();

        /// <summary>
        /// Sets the query options based on OData $filter query option
        /// </summary>
        /// <param name="filterQueryOption">OData filter query option</param>
        /// <returns>The <see cref="CssQueryOptions"/></returns>
        /// <exception cref="ArgumentException">Thrown on invalid query option</exception>
        public CssQueryOptions GetCssQueryOptions(FilterQueryOption filterQueryOption)
        {
            ThrowIfNull(nameof(filterQueryOption), filterQueryOption);

            return GetCssQueryOptions(filterQueryOption.FilterClause);
        }

        /// <summary>
        /// Sets the query options based on OData $filter query option
        /// </summary>
        /// <param name="filterClause">OData filter clause</param>
        /// <returns>The <see cref="CssQueryOptions"/></returns>
        /// <exception cref="ArgumentException">Thrown on invalid query option</exception>
        public CssQueryOptions GetCssQueryOptions(FilterClause filterClause)
        {
            ThrowIfNull(nameof(filterClause), filterClause);

            // The OData $filter option is an expression tree with potentially deeply-nested
            // conditions, but CSS only supports a simple set of query options. The conversion from
            // OData $filter to CSS query options is therefore a best effort and not complete; for
            // example, if multiple StartTime filters are specified, the last one is used. It is
            // designed to work with the $filter string that is passed by Weve.
            RecursivelySetFilterQueryOptions(filterClause.Expression);

            return new CssQueryOptions
            {
                SignalTypes = signalTypes,
                StartFilter = startFilter,
                EndFilter = endFilter,
            };
        }

        private void RecursivelySetFilterQueryOptions(SingleValueNode singleValueNode)
        {
            if (singleValueNode.Kind == QueryNodeKind.BinaryOperator)
            {
                var binaryOperatorNode = singleValueNode as BinaryOperatorNode;

                if (binaryOperatorNode.Left.Kind == QueryNodeKind.SingleValuePropertyAccess
                    && binaryOperatorNode.Right.Kind == QueryNodeKind.Constant)
                {
                    var propertyNode = binaryOperatorNode.Left as SingleValuePropertyAccessNode;
                    var valueNode = binaryOperatorNode.Right as ConstantNode;
                    var operatorKind = binaryOperatorNode.OperatorKind;

                    switch (propertyNode.Property.Name)
                    {
                        case nameof(Signal.SignalType):
                            SetSignalTypeFilter(valueNode.Value as ODataEnumValue, operatorKind);
                            break;
                        case nameof(Signal.StartTime):
                        case nameof(Signal.EndTime):
                            SetTimeFilter(propertyNode.Property.Name, valueNode, operatorKind);
                            break;
                        default:
                            throw new ArgumentException($"CSS only supports filters on these properties: ${nameof(Signal.SignalType)}, ${nameof(Signal.StartTime)}, ${nameof(Signal.EndTime)}");
                    }
                }

                RecursivelySetFilterQueryOptions(binaryOperatorNode.Left);
                RecursivelySetFilterQueryOptions(binaryOperatorNode.Right);
            }
        }

        private void SetTimeFilter(string name, ConstantNode valueNode, BinaryOperatorKind operatorKind)
        {
            var time = (DateTimeOffset)valueNode.Value;

            if (name == nameof(Signal.StartTime))
            {
                // CSS only supports one StartTime with GreaterThanOrEqual operator,
                // it will convert both GreaterThan and GreaterThanOrEqual to GreaterThanOrEqual to make the best effort of being the most accurate. and ignore all other operators.
                if (operatorKind == BinaryOperatorKind.GreaterThanOrEqual || operatorKind == BinaryOperatorKind.GreaterThan)
                {
                    startFilter = new DateTimeFilter { FilterProperty = FilterProperty.StartTime, DateTime = time, ComparisonOperator = ComparisonOperator.GreaterThanOrEqual };
                }
            }
            else if (name == nameof(Signal.EndTime))
            {
                // CSS only supports one EndTime with LessThanOrEqual operator,
                // it will convert both LessThanOrEqual and LessThan to LessThanOrEqual to make the best effort of being the most accurate. and ignore all other operators.
                if (operatorKind == BinaryOperatorKind.LessThanOrEqual || operatorKind == BinaryOperatorKind.LessThan)
                {
                    endFilter = new DateTimeFilter { FilterProperty = FilterProperty.EndTime, DateTime = time, ComparisonOperator = ComparisonOperator.LessThanOrEqual };
                }
            }
        }

        private void SetSignalTypeFilter(ODataEnumValue enumValue, BinaryOperatorKind operatorKind)
        {
            if (operatorKind != BinaryOperatorKind.Equal)
            {
                throw new ArgumentException($"{nameof(Signal.SignalType)} filter only supports 'eq' operator");
            }

            var signalType = Enum.Parse<SignalType>(enumValue.Value);

            if (signalType == SignalType.InvalidSignalType)
            {
                return;
            }

            signalTypes.Add(signalType);
        }

        /// <summary>
        /// Throws if the argument is null.
        /// </summary>
        /// <param name="name">The argument name</param>
        /// <param name="arg">The argument itself</param>
        /// <exception cref="ArgumentNullException">
        /// Throws if the arg is null.
        /// </exception>
        public static void ThrowIfNull(string name, object arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
