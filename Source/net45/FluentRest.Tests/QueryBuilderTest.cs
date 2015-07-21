﻿using System;
using System.Linq;
using Xunit;

namespace FluentRest.Tests
{
    public class QueryBuilderTest
    {
        [Fact]
        public void QueryStringNull()
        {
            var request = new FluentRequest();
            var builder = new QueryBuilder(request);

            string value = null;
            builder.BaseUri("http://test.com/");
            builder.QueryString("Test", value);

            var uri = request.RequestUri();

            Assert.Equal("http://test.com/?Test=", uri.ToString());
        }

        [Fact]
        public void QueryStringMultipleValue()
        {
            var request = new FluentRequest();
            var builder = new QueryBuilder(request);

            builder.BaseUri("http://test.com/");
            builder.QueryString("Test", "Test1");
            builder.QueryString("Test", "Test2");

            var uri = request.RequestUri();

            Assert.Equal("http://test.com/?Test=Test1&Test=Test2", uri.ToString());
        }

        [Fact]
        public void HeaderSingleValue()
        {
            var request = new FluentRequest();
            var builder = new QueryBuilder(request);

            string value = null;
            builder.BaseUri("http://test.com/");
            builder.Header("Test", "Test");

            Assert.True(builder.Request.Headers.ContainsKey("Test"));
            Assert.True(builder.Request.Headers["Test"].First() == "Test");
        }

        [Fact]
        public void HeaderNullValue()
        {
            var request = new FluentRequest();
            var builder = new QueryBuilder(request);

            string value = null;
            builder.BaseUri("http://test.com/");
            builder.Header("Test", "Test");

            Assert.True(builder.Request.Headers.ContainsKey("Test"));

            builder.Header("Test", value);
            Assert.False(builder.Request.Headers.ContainsKey("Test"));
        }
    }
}
