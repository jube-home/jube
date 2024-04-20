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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Jube.Data.Security;

namespace Jube.DynamicEnvironment;

public class DynamicEnvironment
{
    private readonly Dictionary<string, string> _appSettings;
    private readonly ILog _log;

    public DynamicEnvironment(ILog log)
    {
        try
        {
            _log = log;

            _log.Info("Dynamic Variables:  The default and hardcoded values are going to be added.");

            _appSettings = new Dictionary<string, string>
            {
                {"ModelSynchronisationWait", "10000"},
                {"EnableNotification", "True"},
                {"EnableTtlCounter", "True"},
                {"ConnectionString", null},
                {"CacheConnectionString", null},
                {"EnableSearchKeyCache", "True"},
                {"EnableCasesAutomation", "True"},
                {"CasesAutomationWait", "60000"},
                {"EnableEntityModel", "True"},
                {"ArchiverPersistThreads", "1   "},
                {"ModelInvokeAsynchronousThreads", "1"},
                {"BulkCopyThreshold", "100"},
                {"ActivationWatcherBulkCopyThreshold", "100"},
                {"ReprocessingThreads", "1"},
                {"ThreadPoolManualControl", "False"},
                {"MinThreadPoolThreads", "1"},
                {"MaxThreadPoolThreads", "1000"},
                {"MaximumModelInvokeAsyncQueue", "10000"},
                {"SMTPHost", null},
                {"SMTPPort", "587"},
                {"SMTPUser", null},
                {"SMTPPassword", null},
                {"SMTPFrom", null},
                {"ClickatellAPIKey", null},
                {"HttpAdaptationUrl", "https://localhost:5001"},
                {"HttpAdaptationTimeout", "1000"},
                {"HttpAdaptationValidateSsl", "False"},
                {"ReprocessingBulkLimit", "10000"},
                {"EnableMigration", "True"},
                {"EnableSanction", "True"},
                {"NegotiateAuthentication", "False"},
                {"EnableExhaustiveTraining", "True"},
                {"EnableCacheIndex", "True"},
                {"EnableReprocessing", "True"},
                {"UseMockDataExhaustive", "True"},
                {"SanctionLoaderWait", "60000"},
                {"EnableSanctionLoader", "False"},
                {"ActivationWatcherAllowPersist", "True"},
                {"ActivationWatcherPersistThreads", "1"},
                {"AMQP", "False"},
                {"AMQPUri", null},
                {"JWTValidAudience", "http://localhost:5001"},
                {"JWTValidIssuer", "http://localhost:5001"},
                {"JWTKey", null},
                {"PasswordHashingKey", null},
                {"EnablePublicInvokeController", "True"},
                {"EnableEngine", "True"},
                {"ExhaustiveTrialsLimit", "1000"},
                {"ExhaustiveMinVariableCount", "5"},
                {"ExhaustiveMaxVariableCount", "30"},
                {"ExhaustiveTrainingDataSamplePercentage", "0.6"},
                {"ExhaustiveCrossValidationDataSamplePercentage", "0.2"},
                {"ExhaustiveTestingDataSamplePercentage", "0.2"},
                {"ExhaustiveValidationTestingActivationThreshold", "0.5"},
                {"ExhaustiveTopologySinceImprovementLimit", "10"},
                {"ExhaustiveLayerDepthLimit", "4"},
                {"ExhaustiveLayerWidthLimitInputLayerFactor", "4"},
                {"ExhaustiveTopologyComplexityLimit", "10000"},
                {"ExhaustiveActivationFunctionExplorationEpochs", "3"},
                {"ExhaustiveTopologyExplorationEpochs", "3"},
                {"ExhaustiveTopologyFinalisationEpochs", "20"},
                {"ExhaustiveSimulationsCount", "100"},
                {"EnableCallback", "True"},
                {"CallbackTimeout", "3000"},
                {"StreamingActivationWatcher", "True"},
                {"WaitPollFromActivationWatcherTable", "5000"},
                {"WaitTtlCounterDecrement", "60000"},
                {"ShowWelcomeMessage", "True"}
            };

            _log.Info(
                "Dynamic Variables:  The default and hardcoded values have been added,  will now debug write out.");

            foreach (var (key, value) in _appSettings)
                _log.Debug(
                    $"Jube Environment Synchronisation Start: Default and hardcoded value Key: {key} Value:{value}.");

            var runningDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var pathConfig = Path.Combine(runningDirectory ?? throw new InvalidOperationException(),
                "Jube.environment");
            var configFile = new FileInfo(pathConfig);

            if (configFile.Exists)
            {
                var sr = new StreamReader(configFile.FullName);
                var line = sr.ReadLine();
                while (line != null)
                {
                    var lineSplits = line.Split('=', 2);
                    if (!lineSplits[0].StartsWith("#")) //'Commented, falling back to default.
                    {
                        if (_appSettings.ContainsKey(lineSplits[0]))
                        {
                            if (lineSplits[1] == _appSettings[lineSplits[0]])
                            {
                                log.Info("Jube Environment Synchronisation Start: File configuration variable " +
                                         lineSplits[0] +
                                         " enjoys the same value in Application Settings so has not been overwritten.");
                            }
                            else
                            {
                                _appSettings[lineSplits[0]] = lineSplits[1];

                                log.Info("Jube Environment Synchronisation Start: Environment variable " +
                                         lineSplits[0] + " has been updated as " + lineSplits[1] +
                                         " in Application Settings so has not been overwritten.");
                            }
                        }
                        else
                        {
                            log.Info("Jube Environment Synchronisation Start: File configuration variable " +
                                     lineSplits[0] + " is not a valid Application Setting.");
                        }
                    }

                    line = sr.ReadLine();
                }

                _log.Info(
                    "Dynamic Variables:  The configuration file values have been added,  will now debug write out.");

                foreach (var (key, value) in _appSettings)
                    _log.Debug(
                        $"Jube Environment Synchronisation Start: Configuration file values value Key: {key} Value:{value}.");

            }
            else
            {
                _log.Info(
                    "Dynamic Variables:  Jube.environment file does not exist.  Default values will be carried forward.");
            }

            foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
                if (_appSettings.ContainsKey(Convert.ToString(environmentVariable.Key) ?? string.Empty))
                {
                    if (Convert.ToString(environmentVariable.Value) ==
                        _appSettings[environmentVariable.Key.ToString() ?? string.Empty])
                    {
                        log.Info("Jube Environment Synchronisation Start: Environment variable " +
                                 environmentVariable.Key +
                                 " enjoys the same value in Application Settings so has not been overwritten.");
                    }
                    else
                    {
                        _appSettings[environmentVariable.Key.ToString() ?? string.Empty] =
                            Convert.ToString(environmentVariable.Value);

                        log.Info("Jube Environment Synchronisation Start: Environment variable " +
                                 environmentVariable.Key + " has been updated as " + environmentVariable.Value +
                                 " in Application Settings so has been overwritten.");
                    }
                }
                else
                {
                    log.Info("Jube Environment Synchronisation Start: Environment variable " +
                             environmentVariable.Key + " is not a valid Application Setting.");
                }

            _log.Info(
                "Dynamic Variables:  The environment variables have been added,  will now debug write out.");
                
            foreach (var (key, value) in _appSettings)
                _log.Debug(
                    $"Jube Environment Synchronisation Start: Environment variables value Key: {key} Value:{value}.");
                
            _log.Info(
                "Dynamic Variables:  Will check that key values now exist,  if not,  they will be created randomly.");
                
            CheckEssentialValuesAndCreateRandomly();

            _log.Info(
                "Dynamic Variables: Checked for encryption keys and created them randomly if not exist.");
            
            DumpOutToJubeEnvironmentFile(pathConfig);
                
            _log.Info(
                "Dynamic Variables:  Keys written to local Jube.environment file.");
        }
            
        catch (Exception ex)
        {
            log.Error(
                "Jube Environment Synchronisation Start: Error in starting Jube Environment Synchronisation as " +
                ex + ".");
        }
    }

    private void DumpOutToJubeEnvironmentFile(string pathConfig)
    {
        var wr = new StreamWriter(pathConfig, false);
        foreach (var appSetting in _appSettings)
        {
            wr.WriteLine(appSetting.Value == null ? $"#{appSetting.Key}=" : $"{appSetting.Key}={appSetting.Value}");
        }
        wr.Close();
    }
    
    private void CheckEssentialValuesAndCreateRandomly()
    {
        if (_appSettings["PasswordHashingKey"] == null)
        {
            var passwordHashingKey = HashPassword.CreatePasswordInClear(64);
            _appSettings["PasswordHashingKey"] = passwordHashingKey;
        }
            
        if (_appSettings["JWTKey"] == null)
        {
            var jwtHashingKey = HashPassword.CreatePasswordInClear(64);
            _appSettings["JWTKey"] = jwtHashingKey;
        }
    }
    
    public string AppSettings(string[] keys)
    {
        return keys.Select(AppSettings).FirstOrDefault(value => value != null);
    }

    public string AppSettings(string key)
    {
        string value = null;
        try
        {
            if (_appSettings.ContainsKey(key)) value = _appSettings[key];
        }
        catch (Exception ex)
        {
            _log.Error("Jube Environment Variable: Error fetching variable with key " + key + " with error " + ex);
        }

        return value;
    }
}