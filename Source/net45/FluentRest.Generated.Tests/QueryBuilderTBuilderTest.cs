// <copyright file="QueryBuilderTBuilderTest.cs" company="LoreSoft">Copyright © 2015 LoreSoft</copyright>
using System;
using System.Collections.Generic;
using FluentRest;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentRest.Generated.Tests
{
    /// <summary>This class contains parameterized unit tests for QueryBuilder`1</summary>
    [PexClass(typeof(QueryBuilder<>))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class QueryBuilderTBuilderTest
    {
        /// <summary>Test stub for Header(String, IEnumerable`1&lt;String&gt;)</summary>
        [PexMethod]
        public TBuilder HeaderTest<TBuilder>(
            [PexAssumeNotNull]QueryBuilder<TBuilder> target,
            string name,
            IEnumerable<string> values
        )
            where TBuilder : QueryBuilder<TBuilder>
        {
            TBuilder result = target.Header(name, values);
            return result;
            // TODO: add assertions to method QueryBuilderTBuilderTest.HeaderTest(QueryBuilder`1<!!0>, String, IEnumerable`1<String>)
        }
    }
}
