﻿/*----------------------------------------------------------------
 *  Copyright (c) ThoughtWorks, Inc.
 *  Licensed under the Apache License, Version 2.0
 *  See LICENSE.txt in the project root for license information.
 *----------------------------------------------------------------*/


using System.Collections.Generic;
using System.Linq;
using Gauge.Dotnet.Models;
using NUnit.Framework;

namespace Gauge.Dotnet.UnitTests
{
    [TestFixture]
    public class StepRegistryTests
    {
        [Test]
        public void ShouldContainMethodForStepDefined()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo", new GaugeMethod {Name = "Foo"}),
                new KeyValuePair<string, GaugeMethod>("Bar", new GaugeMethod {Name = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            Assert.True(stepRegistry.ContainsStep("Foo"));
            Assert.True(stepRegistry.ContainsStep("Bar"));
        }

        [Test]
        public void ShouldGetAliasWhenExists()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("foo {}",
                    new GaugeMethod
                    {
                        StepValue = "foo {}",
                        Name = "Foo",
                        StepText = "foo <something>",
                        HasAlias = true
                    }),
                new KeyValuePair<string, GaugeMethod>("bar {}",
                    new GaugeMethod
                    {
                        StepValue = "bar {}",
                        Name = "Foo",
                        StepText = "boo <something>",
                        HasAlias = true
                    })
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            Assert.True(stepRegistry.HasAlias("foo {}"));
        }

        [Test]
        public void ShouldGetAllSteps()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo", new GaugeMethod {Name = "Foo"}),
                new KeyValuePair<string, GaugeMethod>("Bar", new GaugeMethod {Name = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            var allSteps = stepRegistry.AllSteps().ToList();

            Assert.AreEqual(allSteps.Count, 2);
            Assert.True(allSteps.Contains("Foo"));
            Assert.True(allSteps.Contains("Bar"));
        }

        [Test]
        public void ShouldGetEmptyStepTextForInvalidParameterizedStepText()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo", new GaugeMethod {Name = "Foo"}),
                new KeyValuePair<string, GaugeMethod>("Bar", new GaugeMethod {Name = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            Assert.AreEqual(stepRegistry.GetStepText("random"), string.Empty);
        }

        [Test]
        public void ShouldGetMethodForStep()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo", new GaugeMethod {Name = "Foo"}),
                new KeyValuePair<string, GaugeMethod>("Bar", new GaugeMethod {Name = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            var method = stepRegistry.MethodFor("Foo");

            Assert.AreEqual(method.Name, "Foo");
        }

        [Test]
        public void ShouldGetStepTextFromParameterizedStepText()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo {}",
                    new GaugeMethod
                    {
                        Name = "Foo",
                        StepValue = "foo {}",
                        StepText = "Foo <something>"
                    }),
                new KeyValuePair<string, GaugeMethod>("Bar", new GaugeMethod {Name = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);


            Assert.AreEqual(stepRegistry.GetStepText("Foo {}"), "Foo <something>");
        }

        [Test]
        public void ShouldNotHaveAliasWhenSingleStepTextIsDefined()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo",
                    new GaugeMethod {Name = "Foo", StepText = "Foo"}),
                new KeyValuePair<string, GaugeMethod>("Bar",
                    new GaugeMethod {Name = "Bar", StepText = "Bar"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            Assert.False(stepRegistry.HasAlias("Foo"));
            Assert.False(stepRegistry.HasAlias("Bar"));
        }

        [Test]
        public void ShouldRemoveStepsDefinedInAGivenFile()
        {
            var methods = new[]
            {
                new KeyValuePair<string, GaugeMethod>("Foo",
                    new GaugeMethod {Name = "Foo", StepText = "Foo", FileName = "Foo.cs"}),
                new KeyValuePair<string, GaugeMethod>("Bar",
                    new GaugeMethod {Name = "Bar", StepText = "Bar", FileName = "Bar.cs"})
            };
            var stepRegistry = new StepRegistry();
            foreach (var pair in methods)
                stepRegistry.AddStep(pair.Key, pair.Value);

            stepRegistry.RemoveSteps("Foo.cs");
            Assert.False(stepRegistry.ContainsStep("Foo"));
        }

        [Test]
        public void ShouldCheckIfFileIsCached()
        {
            var stepRegistry = new StepRegistry();
            stepRegistry.AddStep("Foo", new GaugeMethod {Name = "Foo", StepText = "Foo", FileName = "Foo.cs"});

            Assert.True(stepRegistry.IsFileCached("Foo.cs"));
            Assert.False(stepRegistry.IsFileCached("Bar.cs"));
        }

        [Test]
        public void ShouldNotContainStepPositionForExternalSteps()
        {
            var stepRegistry = new StepRegistry();
            stepRegistry.AddStep("Foo", new GaugeMethod {Name = "Foo", StepText = "Foo", FileName = "foo.cs"});
            stepRegistry.AddStep("Bar", new GaugeMethod {Name = "Bar", StepText = "Bar", IsExternal = true});

            var positions = stepRegistry.GetStepPositions("foo.cs");

            Assert.True(positions.Count() == 1);
            Assert.AreNotEqual(positions.First().StepValue, "Bar");
        }
    }
}