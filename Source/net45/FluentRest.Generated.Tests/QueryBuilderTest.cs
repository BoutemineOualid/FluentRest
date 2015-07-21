// <copyright file="QueryBuilderTest.cs" company="LoreSoft">Copyright © 2015 LoreSoft</copyright>
using System;
using FluentRest;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentRest.Generated.Tests
{
    /// <summary>This class contains parameterized unit tests for QueryBuilder</summary>
    [PexClass(typeof(QueryBuilder))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class QueryBuilderTest
    {
    }
}
