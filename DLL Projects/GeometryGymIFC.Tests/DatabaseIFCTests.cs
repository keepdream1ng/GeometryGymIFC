using GeometryGym.Ifc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeometryGymIFC.Tests;
public class DatabaseIFCTests : IAsyncLifetime
{
	private string _ifcFilePath { get; set; }
	private string _currentDir { get; set; }
	public async Task InitializeAsync()
	{
		_currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		_ifcFilePath = Path.Combine(_currentDir, "ifc_test.ifc");
		return;
	}

	public Task DisposeAsync()
	{
		return Task.CompletedTask;
	}

	[Fact]
	public async Task ParseString_Parses_Step_String_Equally()
	{
		// Arrange
		string dbParsedString;
		string dbReadedString;
		string dbReadedFromPath;

		// Act
		var dbFromPath = new DatabaseIfc(_ifcFilePath);
		dbReadedFromPath = dbFromPath.ToString(FormatIfcSerialization.STEP);

		using (FileStream fs = new FileStream(_ifcFilePath, FileMode.Open))
		using (TextReader sr = new StreamReader(fs))
		{
			var dbFromStream = new DatabaseIfc(sr);
			dbReadedString = dbFromStream.ToString(FormatIfcSerialization.STEP);
		}

		using (FileStream fs = new FileStream(_ifcFilePath, FileMode.Open))
		using (StreamReader sr = new StreamReader(fs))
		{
			string ifcString = await sr.ReadToEndAsync();
			var db = DatabaseIfc.ParseString(ifcString);
			dbParsedString = db.ToString(FormatIfcSerialization.STEP);
		}

		// Assert
		Assert.True(dbParsedString.Length > 0);
		Assert.Equal(dbReadedString, dbParsedString);
		Assert.Equal(dbReadedFromPath, dbParsedString);
	}
}
