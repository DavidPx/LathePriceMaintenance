<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>Google.Apis.Sheets.v4</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>Google.Apis.Auth.OAuth2</Namespace>
  <Namespace>Google.Apis.Sheets.v4</Namespace>
  <Namespace>Google.Apis.Util.Store</Namespace>
  <Namespace>Google.Apis.Services</Namespace>
  <Namespace>Google.Apis.Sheets.v4.Data</Namespace>
  <Namespace>static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.UpdateRequest</Namespace>
</Query>

void Main()
{
	var query = new FileInfo(Util.CurrentQueryPath);
	var outputDirectory =  new DirectoryInfo(@"C:\Users\Dave\Documents\Scraper\Scraper\bin\Debug\netcoreapp2.1\output");
	var csvFiles = outputDirectory.EnumerateFiles("*.csv");
	
	var allPrices = new List<RowWithSource>();
	
	foreach (var file in csvFiles)
	{
		using (var rdr = new StreamReader(file.FullName))
		{
			var csv = new CsvReader(rdr, false);
			var classMap = csv.Configuration.AutoMap<Row>();
			
			// My price data has dollar signs so those need to be stripped out before parsing to a decimal
			classMap.Map(row => row.Price).ConvertUsing(row => decimal.Parse(row.GetField("Price").Replace("$", "")));
			
			// CsvHelper doesn't support URIs so that requires help as well
			classMap.Map(row => row.Source).ConvertUsing(row => new Uri(row.GetField("Source")));
					
			var prices = csv.GetRecords<Row>();
			
			foreach (var price in prices)
			{
				var priceWithSite = new RowWithSource(price);
				priceWithSite.Site = file.Name.Replace(".csv", "");
				allPrices.Add(priceWithSite);
			}
		}
		
	}
	
	UserCredential credential;
	
	using (var credentialsStream = new System.IO.FileStream(@"G:\My Drive\Woodworking\Turning\PriceScrapes\credentials.json", FileMode.Open, FileAccess.Read))
	{	
		var scopes = new string[] {SheetsService.Scope.Spreadsheets};
		
		string credPath = @"G:\My Drive\Woodworking\Turning\PriceScrapes\token.json";
		credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
			GoogleClientSecrets.Load(credentialsStream).Secrets,
			scopes,
			"user",
			CancellationToken.None,
			new FileDataStore(credPath, true)).Result;
	}

	var service = new SheetsService(new BaseClientService.Initializer()
	{
		HttpClientInitializer = credential,
		ApplicationName = "Price Uploader",
	});

	var spreadsheetId = "1rH38d82bQG1XGvxDhPq2N3SNvjItcgpNeUHhpV-qOZk";
	var range = "Input!A2:F";
	
	var getter = service.Spreadsheets.Values.Get(spreadsheetId, range);
	var existingData = getter.Execute();
	
	var values = new ValueRange();
	values.Range = range;
	values.Values = new List<IList<object>>();
	foreach (var row in allPrices)
	{
		values.Values.Add(new List<object> { row.ManufacturerSku, row.Price, row.Manufacturer, row.Source.ToString(), row.Extraction_Time, row.Site });
	}

	// Add in existing data that isn't in the latest extraction; prevent missing prices from wrecking our source data.  Merge it in.
	if (existingData?.Values != null)
	{
		foreach (var existingRow in existingData.Values)
		{
			var sku = existingRow[0].ToString();
			var site = existingRow[5].ToString();

			if (!values.Values.Any(v => v[0].Equals(sku) && v[5].Equals(site)))
			{
				values.Values.Add(existingRow);
				existingRow.Dump("backfilled existing row");
			}
		}
	}
	
	var putter = service.Spreadsheets.Values.Update(values, spreadsheetId, range);
	putter.ValueInputOption = ValueInputOptionEnum.RAW;
	putter.Execute().Dump();
}

class Row
{
	// Price	ManufacturerSku	Manufacturer	Source	Extraction_Time
	public decimal Price { get; set; }
	public string ManufacturerSku { get; set; }
	public string Manufacturer { get; set; }
	public Uri Source { get; set; }
	public DateTimeOffset Extraction_Time { get; set; }
}

class RowWithSource : Row
{
	public RowWithSource(Row r)
	{
		this.Price = r.Price;
		this.Manufacturer = r.Manufacturer;
		this.ManufacturerSku = r.ManufacturerSku;
		this.Source = r.Source;
		this.Extraction_Time = r.Extraction_Time;
	}
	public string Site { get; set; }
}