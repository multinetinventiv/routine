using NUnit.Framework;
using Routine.Core.Configuration.Convention;
using Routine.Engine.Extractor;
using Routine.Engine;
using Routine.Test.Core;
using System.Collections.Generic;
using System;

namespace Routine.Test.Engine.Extractor;

public class ResultClass
{
    public void VoidMethod() { }
    public string ParameterMethod(string parameter) => "ParameterMethod";
    public string StringMethod() => "StringMethod";
    public string StringMethod2() => "StringMethod2";
    public int IntMethod() => 1;
    public List<string> ListMethod() => new() { "ListMethod1", "ListMethod2" };
    public string NullMethod() => null;
}

public abstract class ExtractorContract<T> : CoreTestBase
{
    protected IConvention<IType, T> CreateConventionByPublicMethod(Func<IMethod, bool> filter) => CreateConventionByPublicMethod(filter, e => e);
    protected abstract IConvention<IType, T> CreateConventionByPublicMethod(Func<IMethod, bool> filter, Func<PropertyValueExtractor, PropertyValueExtractor> configurationDelegate);

    protected abstract IConvention<IType, T> CreateConventionByDelegate(Func<object, string> extractorDelegate);

    protected abstract string Extract(T extractor, object obj);

    [Test]
    public void PropertyValueExtactor__When_no_property_was_found_using_given_filter__AppliesTo_returns_false()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name == "NonExistingMethod");

        Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));
    }
    [Test]
    public void PropertyValueExtactor__When_given_filter_finds_inappropriate_method__AppliesTo_returns_false()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name == "ParameterMethod");

        Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));

        testing = CreateConventionByPublicMethod(o => o.Name == "VoidMethod");

        Assert.IsFalse(testing.AppliesTo(type.of<ResultClass>()));
    }

    [Test]
    public void PropertyValueExtactor__When_given_filter_finds_more_than_one_method__first_one_is_used()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name.Contains("StringMethod"));

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
    }

    [Test]
    public void PropertyValueExtactor__When_given_filter_finds_both_appropriate_and_inappropriate_methods__first_appropriate_method_is_used()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name.Contains("Method"));

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
    }

    [Test]
    public void PropertyValueExtactor__Extractor_can_convert_property_result()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name == "IntMethod", e => e.Return(v => "int:" + (int)v));

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual("int:1", Extract(extractor, new ResultClass()));
    }

    [Test]
    public void PropertyValueExtactor__Conversion_delegate_optionally_can_pass_target_object()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name == "IntMethod", e => e.Return((v, o) => ((ResultClass)o).StringMethod() + ":int:" + (int)v));

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual("StringMethod:int:1", Extract(extractor, new ResultClass()));
    }

    [Test]
    public void PropertyValueExtractor__By_default_returns_null_when_property_value_is_null()
    {
        var testing = CreateConventionByPublicMethod(o => o.Name == "NullMethod");

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual(null, Extract(extractor, new ResultClass()));
    }

    [Test]
    public void DelegateBasedExtractor__Extracts_using_given_delegate()
    {
        var testing = CreateConventionByDelegate(o => ((ResultClass)o).StringMethod());

        var extractor = testing.Apply(type.of<ResultClass>());

        Assert.AreEqual("StringMethod", Extract(extractor, new ResultClass()));
    }
}
