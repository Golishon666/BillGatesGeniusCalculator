using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Tests
{
    public sealed class ArchitectureTests
    {
        [Test]
        public void OnlyViewClassesInProjectInheritMonoBehaviour()
        {
            var offenders = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name is "Calculator.Unity" or "MessageBox")
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(MonoBehaviour).IsAssignableFrom(type))
                .Where(type => !type.Name.EndsWith("View"))
                .Select(type => type.FullName)
                .ToArray();

            Assert.That(offenders, Is.Empty);
        }

        [Test]
        public void EveryViewMonoBehaviourHasInitializeMethod()
        {
            var offenders = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name is "Calculator.Unity" or "MessageBox")
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(MonoBehaviour).IsAssignableFrom(type))
                .Where(type => type.GetMethods().All(method => method.Name != "Initialize"))
                .Select(type => type.FullName)
                .ToArray();

            Assert.That(offenders, Is.Empty);
        }
    }
}
