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
</Query>

void Main()
{
	var query = new FileInfo(Util.CurrentQueryPath);
	var outputDirectory =  Path.Combine(query.Directory.FullName, "output");
	var csvFiles = Directory.EnumerateFiles(outputDirectory, "*.csv");
	
	var allPrices = new List<Row>();
	
	foreach (var file in csvFiles)
	{
		using (var rdr = new StreamReader(file))
		{
			var csv = new CsvReader(rdr, false);
			var classMap = csv.Configuration.AutoMap<Row>();
			
			// My price data has dollar signs so those need to be stripped out before parsing to a decimal
			classMap.Map(row => row.Price).ConvertUsing(row => decimal.Parse(row.GetField("Price").Replace("$", "")));
			
			// CsvHelper doesn't support URIs so that requires help as well
			classMap.Map(row => row.Source).ConvertUsing(row => new Uri(row.GetField("Source")));
			
			var prices = csv.GetRecords<Row>();
			
			allPrices.AddRange(prices);
		}
		
	}
	
	allPrices.Dump();

	UserCredential credential;
	
	using (var credentialsStream = new System.IO.FileStream(Path.Combine(query.DirectoryName, "credentials.json"), FileMode.Open, FileAccess.Read))
	{	
		var scopes = new string[] {SheetsService.Scope.Spreadsheets};
		
		string credPath = "token.json";
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

	String spreadsheetId = "1rH38d82bQG1XGvxDhPq2N3SNvjItcgpNeUHhpV-qOZk";
	String range = "Merge!A:E";
	SpreadsheetsResource.ValuesResource.GetRequest request =
			service.Spreadsheets.Values.Get(spreadsheetId, range);

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