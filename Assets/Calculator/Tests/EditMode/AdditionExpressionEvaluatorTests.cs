using BillGatesGeniusCalculator.Calculator.Domain;
using NUnit.Framework;
using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Tests
{
    public sealed class AdditionExpressionEvaluatorTests
    {
        private AdditionExpressionEvaluator _evaluator;

        [SetUp]
        public void SetUp()
        {
            _evaluator = new AdditionExpressionEvaluator();
        }

        [TestCase("54+21", 75)]
        [TestCase("45+00", 45)]
        [TestCase("1+2+3", 6)]
        public void Evaluate_ReturnsSum_ForValidExpression(string expression, long expected)
        {
            var result = _evaluator.Evaluate(expression);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expected));
        }

        [TestCase("")]
        [TestCase("42")]
        [TestCase("45+-88")]
        [TestCase("98.12+48.1")]
        [TestCase("5/5")]
        [TestCase("+5")]
        [TestCase("5+")]
        [TestCase("5++5")]
        [TestCase("5 + 5")]
        [TestCase("abc")]
        [TestCase("5-5")]
        [TestCase("5*5")]
        public void Evaluate_ReturnsError_ForInvalidExpression(string expression)
        {
            var result = _evaluator.Evaluate(expression);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void Evaluate_ReturnsError_OnOverflow()
        {
            var result = _evaluator.Evaluate($"{long.MaxValue}+1");

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void Evaluate_ReturnsConfiguredResult_WhenAdditionalOperationsAreEnabled()
        {
            var config = CreateOperationsConfig(addition: true, subtraction: true, multiplication: true, division: true);
            var evaluator = new AdditionExpressionEvaluator(config);

            var result = evaluator.Evaluate("10+6/2*3-4");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(15));
        }

        [TestCase("5/0")]
        [TestCase("-5+1")]
        [TestCase("5/-1")]
        [TestCase("5//1")]
        public void Evaluate_ReturnsError_ForInvalidConfiguredOperationExpression(string expression)
        {
            var config = CreateOperationsConfig(addition: true, subtraction: true, multiplication: true, division: true);
            var evaluator = new AdditionExpressionEvaluator(config);

            var result = evaluator.Evaluate(expression);

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void OperationsConfig_EnablesOperations_FromFlags()
        {
            var config = CreateOperationsConfig(addition: true, subtraction: false, multiplication: true, division: false);

            Assert.That(config.IsEnabled('+'), Is.True);
            Assert.That(config.IsEnabled('-'), Is.False);
            Assert.That(config.IsEnabled('*'), Is.True);
            Assert.That(config.IsEnabled('/'), Is.False);
        }

        private static CalculatorOperationsConfig CreateOperationsConfig(
            bool addition,
            bool subtraction,
            bool multiplication,
            bool division)
        {
            var config = ScriptableObject.CreateInstance<CalculatorOperationsConfig>();
            config.Addition = addition;
            config.Subtraction = subtraction;
            config.Multiplication = multiplication;
            config.Division = division;
            return config;
        }
    }
}
