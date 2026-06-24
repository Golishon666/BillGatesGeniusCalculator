using System.IO;
using System.Threading.Tasks;
using BillGatesGeniusCalculator.Calculator.Domain;
using BillGatesGeniusCalculator.Calculator.Infrastructure;
using NUnit.Framework;

namespace BillGatesGeniusCalculator.Calculator.Tests
{
    public sealed class RepositoryTests
    {
        [Test]
        public async Task SaveAndLoadAsync_RestoresState()
        {
            var path = Path.Combine(Path.GetTempPath(), "calculator-state-test.json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var repository = new UnityJsonStateRepository(path);
            await repository.SaveAsync(new CalculatorState("34+47", new[] { "5+5=10" }));

            var loaded = await repository.LoadAsync();

            Assert.That(loaded.CurrentInput, Is.EqualTo("34+47"));
            Assert.That(loaded.History, Is.EqualTo(new[] { "5+5=10" }));
            File.Delete(path);
        }

        [Test]
        public async Task LoadAsync_ReturnsEmptyState_WhenFileIsCorrupted()
        {
            var path = Path.Combine(Path.GetTempPath(), "calculator-state-corrupted-test.json");
            File.WriteAllText(path, "not json");
            var repository = new UnityJsonStateRepository(path);

            var loaded = await repository.LoadAsync();

            Assert.That(loaded.CurrentInput, Is.Empty);
            Assert.That(loaded.History, Is.Empty);
            File.Delete(path);
        }
    }
}
