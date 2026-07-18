// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Diagnostics;

namespace DustInTheWind.CaveOfWonders.Cli.Utils;

internal static class Measure
{
	public static void Action(string actionName, Action action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		action();
		stopwatch.Stop();

		Console.WriteLine($"{actionName}: {stopwatch.ElapsedMilliseconds} ms.");
	}

	public static T Action<T>(string actionName, Func<T> action)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		T result = action();
		stopwatch.Stop();

		Console.WriteLine($"{actionName}: {stopwatch.ElapsedMilliseconds} ms.");

		return result;
	}
}