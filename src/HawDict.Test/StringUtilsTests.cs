// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HawDict.Test
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringUtils_FixSentenceSpacing_NullTest()
        {
            StringUtils.FixSentenceSpacing(null);
        }

        [TestMethod]
        public void StringUtils_FixSentenceSpacing_EmptyTest()
        {
            Assert.AreEqual("", StringUtils.FixSentenceSpacing(""));
        }

        [TestMethod]
        public void StringUtils_FixSentenceSpacing_NoChangeTests()
        {
            TestUtils.LoadAndExecuteTestCases<FixSentenceSpacingTestCase>("StringUtils_FixSentenceSpacing_NoChangeTests.txt");
        }

        [TestMethod]
        public void StringUtils_FixSentenceSpacing_ChangeTests()
        {
            TestUtils.LoadAndExecuteTestCases<FixSentenceSpacingTestCase>("StringUtils_FixSentenceSpacing_ChangeTests.txt");
        }

        public class FixSentenceSpacingTestCase : StringUtilsTestCase
        {
            public FixSentenceSpacingTestCase() : base(StringUtils.FixSentenceSpacing) { }
        }

        public abstract class StringUtilsTestCase : ITestCase
        {
            public string Input { get; private set; }
            public string ExpectedOutput { get; private set; }

            public readonly Func<string, string> FunctionToTest;

            public string ActualOutput { get; private set; }

            public StringUtilsTestCase(Func<string, string> functionToTest)
            {
                FunctionToTest = functionToTest;
            }

            public void Execute()
            {
                ActualOutput = FunctionToTest(Input);
                Assert.AreEqual(ExpectedOutput, ActualOutput);
            }

            public void Parse(string s)
            {
                var split = s.Split('\t');
                Input = split[0];
                ExpectedOutput = split[1];
            }
        }
    }
}