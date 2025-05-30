﻿#if UNITY_2023_2_OR_NEWER
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace DebugToolkit.Tests
{
    public class TabTest : TestBase
    {
        private DebugViewTabTest _debugViewTabTest;

        [OneTimeSetUp]
        public override void OneTimeSetUp() => base.OneTimeSetUp();

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            _debugViewTabTest = new DebugViewTabTest();
            _debugViewTabTest.Start();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await base.TearDown();

            // Destroy the instance. In some cases, it might be better not to do this.
            _debugViewTabTest = null;
        }

        // Test if ScrollView is properly added to the tab
        [Test]
        public void ScrollView_IsCorrectlyAddedAtTab()
        {
            var tab = _debugViewTabTest.Root.Q<Tab>();
            Assert.That(tab.Q<ScrollView>(), Is.Not.Null, "ScrollView should be added to Tab.");
        }

        // Test if TabView is created when AddTab is called without an existing TabView
        [Test]
        public void TabView_IsCorrectlyAdded_WhenAddTab()
        {
            var anotherWindow = _debugViewTabTest.Root.AddWindow("AnotherWindow");
            var (tabRoot, tab) = anotherWindow.AddTab();
            Assert.That(anotherWindow.Q<TabView>(), Is.Not.Null, "TabView should be added to Window when AddTab.");
        }

        // Test if the tab label is correctly set
        [Test]
        public void TabLabel_IsCorrectlySet()
        {
            var anotherWindow = _debugViewTabTest.Root.AddWindow("AnotherWindow");
            var (tabRoot, tab) = anotherWindow.AddTab("NewTab");
            Assert.That(anotherWindow.Q<Tab>(), Is.Not.Null);
            Assert.That(anotherWindow.Q<Tab>().label, Is.EqualTo("NewTab"), "Tab label should be set correctly.");
        }
    }
}
#endif
