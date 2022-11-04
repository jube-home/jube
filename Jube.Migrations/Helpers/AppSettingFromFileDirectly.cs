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
using System.IO;
using System.Reflection;

namespace Jube.Migrations.Helpers
{
    public class AppSettingFromFileDirectly
    {
        public static string AppSetting(string key)
        {
            var runningDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var pathConfig = Path.Combine(runningDirectory ?? throw new InvalidOperationException(),
                "Jube.environment");
            var configFile = new FileInfo(pathConfig);
            
            var sr = new StreamReader(configFile.FullName);
            var line = sr.ReadLine();
            while (line != null)
            {
                var lineSplits = line.Split('=', 2);
                if (!lineSplits[0].StartsWith("#"))
                {
                    if (lineSplits[0] == key)
                    {
                        return lineSplits[1];
                    }
                }

                line = sr.ReadLine();
            }

            return null;
        }
    }
}