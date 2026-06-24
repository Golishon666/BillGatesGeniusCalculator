using BillGatesGeniusCalculator.Calculator.Domain;
using NUnit.Framework;

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
    }
}
