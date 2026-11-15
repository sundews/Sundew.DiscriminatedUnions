// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.KeywordUnions;

using System;
using System.Collections.Generic;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions;

/// <summary>
/// Tests the members that the generator adds to the partial union declarations of the tester project.
/// </summary>
public class KeywordUnionTests
{
    [Test]
    public void Cases_Then_AllCasesShouldBeListedInTheOrderTheyAreDeclared()
    {
        Completion.Cases.Should().Equal(typeof(Completed), typeof(Failed));
        GetCases<Batch<string>>().Should().Equal(typeof(Completed), typeof(List<string>));
    }

    [Test]
    public void Equals_Then_TheContainedCasesShouldBeCompared()
    {
        Completion completed = new Completed(1);
        Completion sameCompleted = new Completed(1);
        Completion otherCompleted = new Completed(2);
        Completion failed = new Failed("failed");

        completed.Equals(sameCompleted).Should().BeTrue();
        (completed == sameCompleted).Should().BeTrue();
        (completed != otherCompleted).Should().BeTrue();
        completed.Equals(failed).Should().BeFalse();
        completed.Equals((object)sameCompleted).Should().BeTrue();
        failed.Should().Be((Completion)new Failed("failed"));
        completed.GetHashCode().Should().Be(sameCompleted.GetHashCode());
        default(Completion).Should().Be(default(Completion));
        default(Completion).Should().NotBe(completed);
        default(Completion).GetHashCode().Should().Be(0);
    }

    [Test]
    public void Equals_Then_NothingShouldBeAllocated()
    {
        Completion completed = new Completed(1);
        Completion sameCompleted = new Completed(1);
        completed.Equals(sameCompleted);
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();

        var result = true;
        for (var i = 0; i < 1000; i++)
        {
            result &= completed.Equals(sameCompleted);
        }

        (GC.GetAllocatedBytesForCurrentThread() - allocatedBytes).Should().Be(0);
        result.Should().BeTrue();
    }

    [Test]
    public void ToString_Then_TheContainedCaseShouldBeDescribed()
    {
        Completion completed = new Completed(1);

        completed.ToString().Should().Be(new Completed(1).ToString());
        default(Completion).ToString().Should().BeEmpty();
    }

    [Test]
    public void ToString_When_UnionDeclaresIt_Then_TheDeclaredOneShouldBeUsed()
    {
        Described described = new Completed(1);

        described.ToString().Should().Be("described");
        described.Should().Be((Described)new Completed(1));
    }

    [Test]
    public void GeneratedFactory_Then_ResultShouldBeKnownAsTheUnionAndContainTheCase()
    {
        var completed = Completion.Completed(1);
        var failed = Completion.Failed("failed");
        var batch = Batch<string>.Completed(2);

        GetStaticType(completed).Should().Be(typeof(Completion));
        GetStaticType(batch).Should().Be(typeof(Batch<string>));
        completed.Should().Be((Completion)new Completed(1));
        failed.Should().Be((Completion)new Failed("failed"));
        batch.Value.Should().Be(new Completed(2));
    }

    [Test]
    public void Segregate_Then_EachCaseShouldGetAListOfItsItems()
    {
        var result = new Completion[] { new Completed(1), new Failed("failed"), new Completed(2) }.Segregate();

        GetStaticType(result).Should().Be(typeof(CompletionSegregation));
        result.Completed.Should().Equal(new Completed(1), new Completed(2));
        result.Failed.Should().Equal(new Failed("failed"));
    }

    [Test]
    public void Segregate_When_UnionIsGeneric_Then_ListsShouldBeOfTheConstructedCases()
    {
        var items = new List<string> { "first" };

        var result = new Batch<string>[] { new Completed(1), items }.Segregate();

        GetStaticType(result).Should().Be(typeof(BatchSegregation<string>));
        result.Completed.Should().Equal(new Completed(1));
        result.Lists.Should().ContainSingle().Which.Should().BeSameAs(items);
    }

    [Test]
    public void Segregate_When_CasesAreRelated_Then_ItemShouldGoToTheMostSpecificCaseItIs()
    {
        // A union declaration keeps the value and not the case it was created from, so an exception that was passed as the general case is still the specific one.
        Exception general = new ArgumentException("general");
        Exception specific = new InvalidOperationException("specific");

        var result = new[] { new Sorted(new InvalidOperationException()), new Sorted(specific), new Sorted(general), new Sorted(5), new Sorted((int?)null) }.Segregate();

        result.InvalidOperationExceptions.Should().HaveCount(2).And.Contain((InvalidOperationException)specific);
        result.Exceptions.Should().Equal(general);
        result.Int32s.Should().Equal(5);
    }

    private static Type GetStaticType<TValue>(TValue value)
    {
        // The type the compiler knows the value as, which is what a factory changes. The type of the value itself would be the same either way.
        _ = value;
        return typeof(TValue);
    }

    private static IReadOnlyList<Type> GetCases<TUnion>()
        where TUnion : IDiscriminatedUnion
    {
        return TUnion.Cases;
    }
}
