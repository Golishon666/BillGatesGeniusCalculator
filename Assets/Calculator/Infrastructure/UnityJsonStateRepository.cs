using System;
using System.Collections.Generic;
using System.IO;
using BillGatesGeniusCalculator.Calculator.Application;
using BillGatesGeniusCalculator.Calculator.Domain;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BillGatesGeniusCalculator.Calculator.Infrastructure
{
    public sealed class UnityJsonStateRepository : IStateRepository
    {
        private const string FileName = "calculator-state.json";

        private readonly string _filePath;

        public UnityJsonStateRepository()
            : this(Path.Combine(UnityEngine.Application.persistentDataPath, FileName))
        {
        }

        public UnityJsonStateRepository(string filePath)
        {
            _filePath = filePath;
        }

        public async UniTask<CalculatorState> LoadAsync()
        {
            return await UniTask.RunOnThreadPool(() =>
            {
                try
                {
                    if (!File.Exists(_filePath))
                    {
                        return new CalculatorState();
                    }

                    var json = File.ReadAllText(_filePath);
                    var dto = JsonUtility.FromJson<StateDto>(json);
                    if (dto == null)
                    {
                        return new CalculatorState();
                    }

                    return new CalculatorState(dto.CurrentInput, dto.History ?? new List<string>());
                }
                catch (Exception)
                {
                    return new CalculatorState();
                }
            });
        }

        public async UniTask SaveAsync(CalculatorState state)
        {
            var snapshot = new StateDto
            {
                CurrentInput = state?.CurrentInput ?? string.Empty,
                History = state?.History != null ? new List<string>(state.History) : new List<string>()
            };

            await UniTask.RunOnThreadPool(() =>
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_filePath, JsonUtility.ToJson(snapshot, true));
            });
        }

        [Serializable]
        private sealed class StateDto
        {
            public string CurrentInput;
            public List<string> History;
        }
    }
}
