/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Accord.MachineLearning.VectorMachines;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Kernels;
using Accord.Statistics.Visualizations;
using Jube.Data.Poco;
using Jube.Engine.Exhaustive.Algorithms;
using Jube.Engine.Exhaustive.Variables;
using Jube.Engine.Helpers;
using log4net;
using System.Threading;
using Accord.Statistics.Distributions.Univariate;
using Jube.Data.Context;
using Jube.Data.Query;
using Jube.Data.Repository;
using Jube.Engine.Model.Processing.Payload;
using Newtonsoft.Json.Serialization;

namespace Jube.Engine.Exhaustive
{
    public class Training
    {
        public bool Stopping;
        private ILog _log;
        private Random _seeded;
        private DynamicEnvironment.DynamicEnvironment _environment;
        private static DefaultContractResolver _contractResolver;

        public Training(ILog log, Random seeded, DynamicEnvironment.DynamicEnvironment environment,DefaultContractResolver contractResolver)
        {
            _log = log;
            _seeded = seeded;
            _environment = environment;
            _contractResolver = contractResolver;
        }

        public void Start()
        {
            while (!Stopping)
            {
                var dbContext = DataConnectionDbContext.GetDbContextDataConnection(_environment.AppSettings("ConnectionString"));
                _log.Info("Exhaustive Training: Opening a database connection.");

                try
                {
                    var queryNext = new GetNextExhaustiveSearchInstanceQuery(dbContext);
                    var repositoryExhaustiveSearchInstance = new ExhaustiveSearchInstanceRepository(dbContext);
                    var repositoryExhaustiveSearchInstanceVariables =
                        new ExhaustiveSearchInstanceVariableRepository(dbContext);
                    var repositoryExhaustiveSearchInstanceVariableHistograms =
                        new ExhaustiveSearchInstanceVariableHistogramRepository(dbContext);
                    var repositoryExhaustiveSearchInstanceVariableMultiCollinearities =
                        new ExhaustiveSearchInstanceVariableMultiColiniarityRepository(dbContext);

                    _log.Info(
                        "Exhaustive Training: Opened a database connection. " +
                        "Will proceed to lookup a request for an Exhaustive Instance,  " +
                        "or in the absence of an Exhaustive Instance to create one.");

                    var exhaustiveSearchInstance = queryNext.Execute();

                    if (exhaustiveSearchInstance != null)
                    {
                        _log.Info(
                            $"Exhaustive Training: Found Exhaustive Search Instance ID {exhaustiveSearchInstance.Id}.  Updating Status to 1 for pickup.");
                        
                        var mockData = false;
                        if (_environment.AppSettings("UseMockDataExhaustive").Equals("True",StringComparison.OrdinalIgnoreCase))
                        {
                            LoadMockData(dbContext,exhaustiveSearchInstance.EntityAnalysisModelId, _log);

                            _log.Info(
                                $"Exhaustive Training: Found Exhaustive Search Instance ID {exhaustiveSearchInstance.Id}.  Is loading mock data.");
                            
                            mockData = true;
                            
                            _log.Info(
                                $"Exhaustive Training: Found Exhaustive Search Instance ID {exhaustiveSearchInstance.Id}.  Will use mock data for this training.");
                        }

                        repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 1);

                        _log.Info(
                            $"Exhaustive Training: Set status to 1 for {exhaustiveSearchInstance.Id}.  Going to fetch data for Entity Analysis Model ID {exhaustiveSearchInstance.Id}.");

                        double[][] data;
                        Dictionary<int, Variable> variables;
                        if (exhaustiveSearchInstance.Anomaly)
                        {
                            Data.Extraction.GetSampleData(dbContext,
                                exhaustiveSearchInstance.TenantRegistryId,
                                exhaustiveSearchInstance.EntityAnalysisModelId,
                                mockData,
                                out variables,
                                out data);
                        }
                        else
                        {
                            Data.Extraction.GetSampleData(dbContext,
                                exhaustiveSearchInstance.TenantRegistryId,
                                exhaustiveSearchInstance.EntityAnalysisModelId,
                                exhaustiveSearchInstance.FilterSql,
                                exhaustiveSearchInstance.FilterTokens,
                                mockData,
                                out variables,
                                out data);
                        }

                        _log.Info(
                            $"Exhaustive Training: Fetched data and there are {data.Length} records for {exhaustiveSearchInstance.Id} updating status to 2.");

                        repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 2);

                        _log.Info(
                            $"Exhaustive Training: Updated status to 2 for {exhaustiveSearchInstance.Id} will now calculate statistics.");

                        CalculateStatistics(exhaustiveSearchInstance.Id, ref variables, data,
                            repositoryExhaustiveSearchInstanceVariables,
                            repositoryExhaustiveSearchInstanceVariableHistograms, _log);

                        _log.Info(
                            $"Exhaustive Training: Calculated statistics for {exhaustiveSearchInstance.Id} and updating status to 3.");

                        repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 3);

                        _log.Info(
                            $"Exhaustive Training: Updated status to 3 for {exhaustiveSearchInstance.Id} will now normalise data.");

                        data = NormaliseData(variables, data, _log);

                        _log.Info(
                            $"Exhaustive Training: Normalised data for {exhaustiveSearchInstance.Id} updating status to 4.");

                        repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 4);

                        _log.Info(
                            $"Exhaustive Training: Updated status to 4 for {exhaustiveSearchInstance.Id} will now train a one class support vector machine with Gaussian RBF.");

                        double[] outputs = default;
                        var classCount = 0d;
                        if (exhaustiveSearchInstance.Anomaly)
                        {
                            var anomaly = Unsupervised.Learn(data, _log);

                            _log.Info(
                                $"Exhaustive Training: Trained One Class Support Vector Machine for {exhaustiveSearchInstance.Id} updating status to 5.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 5);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 5 for {exhaustiveSearchInstance.Id} will now recall One Class Support Vector Machine to derive classifications for Neural Network training.");

                            outputs = GetClassVariableByAnomaly(anomaly, data,
                                exhaustiveSearchInstance.AnomalyProbability, _log);

                            classCount = outputs.Sum();

                            _log.Info(
                                $"Exhaustive Training: Recalled One Class Support Vector Machine and found {classCount} class for {exhaustiveSearchInstance.Id} updating status to 6.");
                        }

                        if (exhaustiveSearchInstance.Filter)
                        {
                            _log.Info(
                                $"Exhaustive Training: Switched to create a classification from data for {exhaustiveSearchInstance.Id} updating status to 6.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 6);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 6 for {exhaustiveSearchInstance.Id} will now extract classifications and features for Neural Network training.");

                            Data.Extraction.GetClassData(dbContext,
                                exhaustiveSearchInstance.EntityAnalysisModelId,
                                exhaustiveSearchInstance.FilterSql,
                                exhaustiveSearchInstance.FilterTokens,
                                variables,
                                data,
                                outputs,
                                mockData,
                                out data,
                                out outputs);

                            classCount = outputs.Sum();

                            _log.Info(
                                $"Exhaustive Training: Recalled Filter and found {classCount} further class for {exhaustiveSearchInstance.Id} updating status to 6.");
                        }

                        _log.Info(
                            $"Exhaustive Training: Finished preparing class data for {exhaustiveSearchInstance.Id} updating status to 7.");

                        repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 7);

                        if (classCount > 0)
                        {
                            _log.Info(
                                $"Exhaustive Training: Updated status to 7 for {exhaustiveSearchInstance.Id} will proceed to make the dataset symmetric for oversampling the class.");

                            DatasetSymmetry(data, outputs, out data, out outputs, _log);

                            _log.Info(
                                $"Exhaustive Training: Has made the dataset symmetric for {exhaustiveSearchInstance.Id} updating status to 7.  Dataset has a length of {outputs.Length} with {outputs.Sum()} class value of 1.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 8);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 8 for {exhaustiveSearchInstance.Id} will proceed to inspect variables for correlation to class value 1.");

                            CalculateCorrelations(variables, data, outputs,
                                repositoryExhaustiveSearchInstanceVariables, _log);

                            _log.Info(
                                $"Exhaustive Training: Processed correlations for {exhaustiveSearchInstance.Id} updating status to 8.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 9);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 9 for {exhaustiveSearchInstance.Id} will proceed to inspect multi-colinearity.");

                            variables = CalculateMulticolinarity(variables, data,
                                repositoryExhaustiveSearchInstanceVariableMultiCollinearities, _log);

                            _log.Info(
                                $"Exhaustive Training: Inspected multi-colinearity for {exhaustiveSearchInstance.Id} updating status to 9.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 10);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 10 for {exhaustiveSearchInstance.Id} will proceed to start the Neural Network training algorithms.");

                            var supervised = new Supervised(_environment, repositoryExhaustiveSearchInstance,
                                exhaustiveSearchInstance.Id,
                                _seeded,
                                variables, data, outputs, dbContext, _log);
                            supervised.Train();

                            _log.Info(
                                $"Exhaustive Training: Finished training {exhaustiveSearchInstance.Id} updating status to 11.");

                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 11);
                        }
                        else
                        {
                            repositoryExhaustiveSearchInstance.UpdateStatus(exhaustiveSearchInstance.Id, 12);

                            _log.Info(
                                $"Exhaustive Training: Updated status to 12 for {exhaustiveSearchInstance.Id} which denotes a zero class count.");
                        }

                        repositoryExhaustiveSearchInstance.UpdateCompleted(exhaustiveSearchInstance.Id);
                        
                    }
                    else
                    {
                        _log.Info(
                            "Exhaustive Training: Has not found anything that needs training.  Waiting.");

                        Thread.Sleep(60000);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(
                        $"Exhaustive Training: Has experienced an error as {ex}  waiting to poll again.");

                    Thread.Sleep(60000);
                }
                finally
                {
                    _log.Info("Exhaustive Training: Finished and closing database context.");
                    
                    dbContext.Close();
                    dbContext.Dispose();
                }
            }
        }

        private void CalculateCorrelations(IReadOnlyDictionary<int, Variable> variables, double[][] data,
            double[] outputs, ExhaustiveSearchInstanceVariableRepository repository, ILog log)
        {
            for (var i = 0; i < variables.Count - 1; i++)
            {
                log.Info(
                    $"Exhaustive Training: About to calculate correlation for ExhaustiveSearch Instance Variable Id {variables[i].ExhaustiveSearchInstanceVariableId}.");

                var independent = data.GetColumn(i);

                var performance = new Performance();
                variables[i].Correlation = performance.SpearmansCoeff(outputs, independent);

                log.Info(
                    $"Exhaustive Training: Calculated correlation as {variables[i].Correlation} for Exhaustive Search Instance Variable Id {variables[i].ExhaustiveSearchInstanceVariableId}.  Will proceed to rank them.");
            }

            var sortedCorrelations = from c in variables
                orderby Math.Abs(Math.Round(c.Value.Correlation, 2)) descending
                select c;

            log.Info(
                "Exhaustive Training: Ranked correlations.  Will store them alongside the existing variable record with ranking.");

            var variableSequence = 0;
            foreach (var (_, value) in sortedCorrelations)
            {
                log.Info(
                    $"Exhaustive Training: Ranked correlations.  Will proceed to store the correlation for Exhaustive Search Instance Variable Id {value.ExhaustiveSearchInstanceVariableId}.");

                repository.UpdateCorrelation(value.ExhaustiveSearchInstanceVariableId,
                    value.Correlation, variableSequence);

                log.Info(
                    $"Exhaustive Training: Ranked correlations. Has stored correlation for {value.ExhaustiveSearchInstanceVariableId}.");

                variableSequence += 1;
            }
        }

        private static void DatasetSymmetry(IReadOnlyList<double[]> inData, double[] inOutputs, out double[][] outData,
            out double[] outOutputs, ILog log)
        {
            var countPositive = (int) inOutputs.Sum();
            var countNegative = inOutputs.Length - countPositive;

            log.Info(
                $"Exhaustive Training: There are {countPositive} values in the affirmative and {countNegative} values in the negative.");

            var remove = new List<int>();
            if (countNegative != countPositive)
            {
                int removeCount;
                int removeValue;
                if (countNegative > countPositive)
                {
                    removeCount = countNegative - countPositive;
                    removeValue = 0;
                    log.Info(
                        $"The dataset is not symmetric and requires {removeCount} affirmative values to be removed.");
                }
                else
                {
                    removeCount = countPositive - countNegative;
                    removeValue = 1;
                    log.Info(
                        $"Exhaustive Training: The dataset is not symmetric and requires {removeCount} negative values to be removed.");
                }

                var seeded = new Random(Guid.NewGuid().GetHashCode());
                while (removeCount > 0)
                {
                    var randomRow = seeded.Next(0, inOutputs.Length - 1);
                    if (Math.Abs(inOutputs[randomRow] - removeValue) < 0.0001)
                    {
                        remove.Add(randomRow);
                        log.Info($"Exhaustive Training: Removed Record {randomRow} from the dataset.");
                        removeCount -= 1;
                    }
                }
            }

            log.Info($"Exhaustive Training: Is about to rebuild the array given {remove.Count} records to remove.");

            var symmetricData = new List<double[]>();
            var symmetricOutputs = new List<double>();
            for (var i = 0; i < inOutputs.Length - 1; i++)
            {
                if (!remove.Contains(i))
                {
                    symmetricData.Add(inData[i]);
                    symmetricOutputs.Add(inOutputs[i]);
                }
            }

            log.Info(
                $"Exhaustive Training: Has rebuilt the array and it is now {symmetricOutputs.Count} records long. Setting output variables.");

            outData = symmetricData.ToArray();
            outOutputs = symmetricOutputs.ToArray();

            log.Info("Exhaustive Training: Set output variables. Symmetry concluded.");
        }

        private static double[] GetClassVariableByAnomaly(SupportVectorMachine<Gaussian> model,
            IReadOnlyList<double[]> data,
            double probability, ILog log)
        {
            var outputs = new double[data.Count];

            for (var i = 0; i < data.Count - 1; i++)
            {
                log.Info(
                    $"Exhaustive Training: Processing row {i}.");

                var p = model.Probability(data[i]);

                log.Info(
                    $"Exhaustive Training: Processed row {i} and returned probability {p}.");

                outputs[i] = p < probability ? 1 : 0;

                log.Info(
                    $"Exhaustive Training: Processed row {i} and created class {outputs[i]}.");
            }

            return outputs;
        }

        private static double[][] NormaliseData(IReadOnlyDictionary<int, Variable> variables, double[][] data, ILog log)
        {
            for (var i = 0; i < data.Length - 1; i++)
            {
                log.Info(
                    $"Exhaustive Training: Starting to normalise row {i}.");

                for (var j = 0; j < variables.Count; j++)
                {
                    if (variables[j].NormalisationType == 2)
                    {
                        data[i][j] = (data[i][j] - variables[j].Mean) / variables[j].Sd;
                    }

                    if (double.IsNaN(data[i][j]) || double.IsInfinity(data[i][j]))
                    {
                        data[i][j] = 0;
                    }

                    log.Info(
                        $"Exhaustive Training: Finished normalising row {i}.");
                }
            }

            return data;
        }

        private static double SwapNanInfinityToZero(double value)
        {
            if (double.IsNaN(value))
            {
                return 0;
            }

            if (double.IsInfinity(value))
            {
                return 0;
            }

            return value;
        }

        private static void CalculateStatistics(int exhaustiveSearchInstanceId, ref Dictionary<int, Variable> variables,
            double[][] data,
            ExhaustiveSearchInstanceVariableRepository repositoryExhaustiveSearchInstanceVariable,
            ExhaustiveSearchInstanceVariableHistogramRepository repositoryExhaustiveSearchInstanceVariableHistogram,
            ILog log
        )
        {
            log.Info(
                $"Exhaustive Training: Is starting calculation of statistics for Exhaustive Search Instance {exhaustiveSearchInstanceId}.  There are {variables.Count} variables.");

            for (var i = 0; i < variables.Count - 1; i++)
            {
                log.Info(
                    $"Exhaustive Training: Processing variable {i} for Exhaustive Search Instance {exhaustiveSearchInstanceId}.  There are {variables.Count} variables.");

                var variableData = data.GetColumn(i);
                var modelExhaustiveSearchInstanceVariable = new ExhaustiveSearchInstanceVariable
                {
                    ExhaustiveSearchInstanceId = exhaustiveSearchInstanceId,
                    VariableSequence = i,
                    Name = variables[i].Name,
                    ProcessingTypeId = variables[i].ProcessingTypeId
                };

                log.Info(
                    $"Exhaustive Training: Variable Name: {modelExhaustiveSearchInstanceVariable.Name} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Mean = SwapNanInfinityToZero(variableData.Mean());
                modelExhaustiveSearchInstanceVariable.Mean = variables[i].Mean;

                log.Info(
                    $"Exhaustive Training: Variable Mean: {modelExhaustiveSearchInstanceVariable.Mean} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Mode = SwapNanInfinityToZero(variableData.Mode());
                modelExhaustiveSearchInstanceVariable.Mode = variables[i].Mode;

                log.Info(
                    $"Exhaustive Training: Variable Mode: {modelExhaustiveSearchInstanceVariable.Mean} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Max = SwapNanInfinityToZero(variableData.Max());
                modelExhaustiveSearchInstanceVariable.Maximum = variables[i].Max;

                log.Info(
                    $"Exhaustive Training: Variable Maximum: {modelExhaustiveSearchInstanceVariable.Maximum} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Min = SwapNanInfinityToZero(variableData.Min());
                modelExhaustiveSearchInstanceVariable.Minimum = variables[i].Min;

                log.Info(
                    $"Exhaustive Training: Variable Minimum: {modelExhaustiveSearchInstanceVariable.Minimum} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Kurtosis = SwapNanInfinityToZero(variableData.Kurtosis());
                modelExhaustiveSearchInstanceVariable.Kurtosis = variables[i].Kurtosis;

                log.Info(
                    $"Exhaustive Training: Variable Kurtosis: {modelExhaustiveSearchInstanceVariable.Kurtosis} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Skewness = SwapNanInfinityToZero(variableData.Skewness());
                modelExhaustiveSearchInstanceVariable.Skewness = variables[i].Skewness;

                log.Info(
                    $"Exhaustive Training: Variable Skewness: {modelExhaustiveSearchInstanceVariable.Skewness} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variableData.Quartiles(out var q1, out var q3);
                variables[i].Iqr = SwapNanInfinityToZero(q3 - q1);
                modelExhaustiveSearchInstanceVariable.Iqr = variables[i].Iqr;

                log.Info(
                    $"Exhaustive Training: Variable Iqr: {modelExhaustiveSearchInstanceVariable.Iqr} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].Sd = SwapNanInfinityToZero(variableData.StandardDeviation());
                modelExhaustiveSearchInstanceVariable.StandardDeviation = variables[i].Sd;

                log.Info(
                    $"Exhaustive Training: Variable Standard Deviation: {modelExhaustiveSearchInstanceVariable.StandardDeviation} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                if (variables[i].Max is 0 and 0)
                {
                    variables[i].NormalisationType = 0; //Empty
                }
                else if (variables[i].Min == 0 && Math.Abs(variables[i].Max - 1) < 0.0001)
                {
                    variables[i].NormalisationType = 1; //Binary
                }
                else
                {
                    variables[i].NormalisationType = 2; //Z Score
                }

                modelExhaustiveSearchInstanceVariable.NormalisationTypeId = variables[i].NormalisationType;

                log.Info(
                    $"Exhaustive Training: Variable Normalisation Type: {modelExhaustiveSearchInstanceVariable.NormalisationTypeId} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                var minNormalised = ZScore(variables[i].Min, variables[i].Mean, variables[i].Sd,
                    variables[i].NormalisationType);
                
                var maxNormalised = ZScore(variables[i].Max, variables[i].Mean, variables[i].Sd,
                    variables[i].NormalisationType);
                
                if (variables[i].Mode == 0)
                {
                    var meanNormalised = ZScore(variables[i].Mean, variables[i].Mean, variables[i].Sd,
                        variables[i].NormalisationType);
                    
                    if (variables[i].Mean == 0)
                    {
                        if (variables[i].Min < variables[i].Max)
                        {
                            variables[i].TriangularDistribution = new TriangularDistribution(minNormalised,
                                maxNormalised, meanNormalised);
                        }
                    }
                    else
                    {
                        variables[i].TriangularDistribution = new TriangularDistribution(minNormalised,
                            maxNormalised, 0.01d);
                    }
                }
                else
                {
                    var modeNormalised = ZScore(variables[i].Mode, variables[i].Mean, variables[i].Sd,
                        variables[i].NormalisationType);
                
                    if (variables[i].Min < variables[i].Max)
                    {
                        variables[i].TriangularDistribution = new TriangularDistribution(minNormalised,
                            maxNormalised, modeNormalised);
                    }
                }

                var distinctDoubles = new List<double>();
                foreach (var value in variableData)
                {
                    if (!distinctDoubles.Contains(value))
                    {
                        distinctDoubles.Add(value);
                    }
                }

                variables[i].DistinctCount = distinctDoubles.Count;
                modelExhaustiveSearchInstanceVariable.DistinctValues = variables[i].DistinctCount;

                log.Info(
                    $"Exhaustive Training: Variable Distinct Values: {modelExhaustiveSearchInstanceVariable.DistinctValues} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                const int maxBins = 20;

                log.Info(
                    $"Exhaustive Training: Maximum number of bins is {maxBins} and will proceed to calculate the optimal number of bins for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                var numberOfBins = default(double);
                try
                {
                    if (Math.Abs(variables[i].Max - 1) < 0.0001 & variables[i].Min == 0 &
                        variables[i].DistinctCount == 2)
                    {
                        numberOfBins = 2;

                        log.Info(
                            $"Exhaustive Training: Variable is inferred to be binary with 2 bins for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");
                    }
                    else if (numberOfBins > variables[i].DistinctCount)
                    {
                        numberOfBins = variables[i].DistinctCount;

                        log.Info(
                            $"Exhaustive Training: Variable is set to distinct values of {variables[i].DistinctCount} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");
                    }
                    else if (variables[i].Iqr == 0)
                    {
                        numberOfBins = maxBins;

                        log.Info(
                            $"Exhaustive Training: Variable is set to distinct values of {variables[i].DistinctCount} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");
                    }
                    else
                    {
                        numberOfBins = Math.Round(1d + 3.3d * Math.Log(data.Length));

                        log.Info(
                            $"Exhaustive Training: Has used Sturges Rule to create {numberOfBins} bins for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");
                    }

                    if (numberOfBins > maxBins)
                    {
                        numberOfBins = maxBins;

                        log.Info(
                            $"Exhaustive Training: Has clipped to {numberOfBins} bins for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");
                    }
                }
                catch (Exception ex)
                {
                    numberOfBins = maxBins;

                    log.Error(
                        $"Exhaustive Training: Has {numberOfBins} bins for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id} after exception of {ex}.");
                }

                variables[i].Bins = (int) numberOfBins;
                modelExhaustiveSearchInstanceVariable.Bins = variables[i].Bins;
                log.Info(
                    $"Exhaustive Training: Number of Bins Values: {modelExhaustiveSearchInstanceVariable.Bins} for Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id}.");

                variables[i].ExhaustiveSearchInstanceVariableId
                    = repositoryExhaustiveSearchInstanceVariable.Insert(modelExhaustiveSearchInstanceVariable)
                        .Id;

                log.Info(
                    $"Exhaustive Training: Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id} inserted and is about to compute histogram with {modelExhaustiveSearchInstanceVariable.Bins} bins.");

                var histogram = new Histogram();
                histogram.Compute(variableData);

                log.Info(
                    $"Exhaustive Training: Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id} has computed histogram with {modelExhaustiveSearchInstanceVariable.Bins} bins.  Will proceed to process bins.");

                var binSequence = 0;
                foreach (var histogramBin in histogram.Bins)
                {
                    var modelExhaustiveSearchInstanceVariableHistogram = new ExhaustiveSearchInstanceVariableHistogram
                    {
                        BinSequence = binSequence,
                        BinRangeStart = histogramBin.Range.Min,
                        BinRangeEnd = histogramBin.Range.Max,
                        ExhaustiveSearchInstanceVariableId = variables[i].ExhaustiveSearchInstanceVariableId,
                        Frequency = histogramBin.Value
                    };

                    log.Info(
                        $"Exhaustive Training: Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id} bin {binSequence} has start: {modelExhaustiveSearchInstanceVariableHistogram.BinRangeStart}, end {modelExhaustiveSearchInstanceVariableHistogram.BinRangeEnd} and frequency of {modelExhaustiveSearchInstanceVariableHistogram.Frequency}.");

                    repositoryExhaustiveSearchInstanceVariableHistogram.Insert(
                        modelExhaustiveSearchInstanceVariableHistogram);

                    log.Info(
                        $"Exhaustive Training: Exhaustive Search Instance Variable ID {modelExhaustiveSearchInstanceVariable.Id} bin {binSequence} inserted will move next bin.");
                    binSequence += 1;
                }
            }
        }

        private Dictionary<int, Variable> CalculateMulticolinarity(Dictionary<int, Variable> variables, double[][] data,
            ExhaustiveSearchInstanceVariableMultiColiniarityRepository repository, ILog log)
        {
            for (var i = 0; i < variables.Count - 1; i++)
            {
                log.Info(
                    $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} will evaluate for strong correlations to this variable.");

                var crossCorrelations = new Dictionary<int, double>();
                for (var j = 0; j < variables.Count - 1; j++)
                {
                    if (i != j)
                    {
                        log.Info(
                            $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} will evaluate correlation against {variables[j].ExhaustiveSearchInstanceVariableId}.");

                        var performance = new Performance();
                        var correlation = performance.SpearmansCoeff(data.GetColumn(i), data.GetColumn(j));
                        crossCorrelations.Add(j, correlation);

                        log.Info(
                            $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} has evaluated correlation against {variables[j].ExhaustiveSearchInstanceVariableId} and returned {correlation}.");
                    }
                    else
                    {
                        log.Info(
                            $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} will not evaluate to itself.");
                    }
                }

                log.Info(
                    $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} will sort correlations for ranking.");

                var sortedCrossCorrelations = from pair in crossCorrelations
                    orderby Math.Abs(Math.Round(pair.Value, 2)) descending
                    select pair;

                log.Info(
                    $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} has sorted correlations for ranking. Will insert these correlations into the database.");

                var seqCrossCorrelation = 1;
                foreach (var (key, value) in sortedCrossCorrelations)
                {
                    var model = new ExhaustiveSearchInstanceVariableMultiCollinearity
                    {
                        ExhaustiveSearchInstanceVariableId = variables[i].ExhaustiveSearchInstanceVariableId,
                        TestExhaustiveSearchInstanceVariableId = variables[key].ExhaustiveSearchInstanceVariableId,
                        Correlation = value,
                        CorrelationAbsRank = seqCrossCorrelation
                    };

                    log.Info(
                        $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} will insert correlation for test key {variables[key].ExhaustiveSearchInstanceVariableId}.");

                    repository.Insert(model);

                    log.Info(
                        $"Exhaustive Training: Exhaustive Search Instance Variable ID {variables[i].ExhaustiveSearchInstanceVariableId} has inserted correlation for test key {variables[key].ExhaustiveSearchInstanceVariableId} and will proceed to next test correlation.");

                    seqCrossCorrelation += 1;
                }
            }

            log.Info(
                "Exhaustive Training: Has concluded calculation of correlation for each correlation,  have updated variables and will return.");

            return variables;
        }

        private static void LoadMockData(DbContext dbContext, int entityAnalysisModelId, ILog log)
        {
            var repository = new MockArchiveRepository(dbContext);
            
            log.Info("Exhaustive Training: Will delete the MockArchive table for mock data.");

            repository.Delete();
            
            log.Info("Exhaustive Training: Deleted MockArchive.");
            
            var variables = new List<string>();

            var iRow = 1;

            var directoryChopped =
                GetParentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, 4);
            var fileLocation = Path.Combine(directoryChopped, "Jube.Engine/Exhaustive/Mock.csv");

            log.Info($"Exhaustive Training: Will try open and iterate through {fileLocation}.");

            foreach (var line in File.ReadLines(fileLocation))
            {
                log.Info($"Exhaustive Training:Found line data {line}.");
                
                var splitsString = line.Split(",");
                if (iRow > 1)
                {
                    try
                    {
                        log.Info($"Exhaustive Training:Found line at line {iRow} {splitsString.Length} splits.");

                        var splitsDouble = new double[splitsString.Length];

                        var row = new EntityAnalysisModelInstanceEntryPayload();
                        for (var i = 0; i < splitsString.Length - 1; i++)
                        {
                            splitsDouble[i] = double.Parse(splitsString[i]);

                            if (i == 0)
                            {
                                row.Tag.Add("Fraud", splitsDouble[i] > 0 ? 1 : 0);
                            }
                            else
                            {
                                row.Abstraction.Add(variables[i], splitsDouble[i]);
                            }
                        }

                        var json = new EntityAnalysisModelInstanceEntryPayloadJson();

                        var model = new MockArchive
                        {
                            EntityAnalysisModelId = entityAnalysisModelId,
                            EntryKeyValue = iRow.ToString(),
                            ResponseElevation = 0,
                            EntityAnalysisModelActivationRuleId = 0,
                            ActivationRuleCount = 0,
                            CreatedDate = DateTime.Now,
                            ReferenceDate = DateTime.Now,
                            EntityAnalysisModelInstanceEntryGuid = Guid.NewGuid()
                        };
                        
                        var sr = new StreamReader(json.BuildJson(row,_contractResolver));
                        model.Json = sr.ReadToEnd();

                        repository.Insert(model);

                        log.Info($"Exhaustive Training:Found line {line} has been added to the array.");
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Exhaustive Training: Line {iRow} has caused an exception {ex}.");
                    }
                }
                else
                {
                    log.Info($"Exhaustive Training:Found header data {line}.");
                    foreach (var split in line.Split(","))
                    {
                        log.Info("Exhaustive Training:Adding variable.");
                        variables.Add(split);
                    }
                }
                
                iRow += 1;
                log.Info($"Exhaustive Training:Concluded line {line}.");
            }

            log.Info(
                $"Exhaustive Training:Concluded and created array of length {iRow} will cast and return array.");
        }

        private static string GetParentDirectory(string path, int parentCount)
        {
            if (string.IsNullOrEmpty(path) || parentCount < 1)
                return path;

            var parent = Path.GetDirectoryName(path);

            if (--parentCount > 0)
                return GetParentDirectory(parent, parentCount);

            return parent;
        }

        private static double ZScore(double value,double mean,double sd,int normalisationTypeId)
        {
            try
            {
                if (normalisationTypeId == 2)
                {
                    return (value - mean) / sd;
                }
            
                return value;
            }
            catch
            {
                return 0;
            }
        }
    }
}