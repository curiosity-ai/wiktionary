
// See https://aka.ms/new-console-template for more information
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Wiktionary;
using RocksDbSharp;
using MessagePack;

ForceInvariantCultureAndUTF8Output();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSimpleConsole();
builder.Logging.AddFilter("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogLevel.Warning);

var app = builder.Build();

var dataPath    = app.Configuration["storage"] ?? "data";
var dbPath      = app.Configuration["db"] ?? "db";
var journalPath = app.Configuration["db-journal"] ?? "journal";

Directory.CreateDirectory(dataPath);
Directory.CreateDirectory(dbPath);
Directory.CreateDirectory(journalPath);

var families = new ColumnFamilies()
                               {
                                { "languages",     new ColumnFamilyOptions() },
                                { "words",         new ColumnFamilyOptions().OptimizeForPointLookup(64)  }
                               };


var options = new DbOptions().SetCreateIfMissing(true).SetCreateMissingColumnFamilies(true).SetWalDir(journalPath).SetWALSizeLimitMB(64);

using (var db = RocksDb.Open(options, dbPath, families))
{
    var langColumn = db.GetColumnFamily("languages");
    var wordColumn = db.GetColumnFamily("words");

    if (db.Get("en", langColumn) is null)
    {
        await DownloadAndPreprocessData(app, dataPath, db, langColumn, wordColumn);
    }

    app.MapGet("/api/word", (string word, string lang) => db.Get(Encoding.UTF8.GetBytes($"{lang}-{word}"), WordDefinitionListSerializer.Instance, wordColumn));
    app.MapGet("/api/ping", () => "pong");

    app.Logger.LogInformation("The app started");

    await app.RunAsync();
}


async Task DownloadAndPreprocessData(WebApplication app, string dataPath, RocksDb db, ColumnFamilyHandle langColumn, ColumnFamilyHandle wordColumn)
{
    var jsonPath = Path.Combine(dataPath, "raw-wiktextract-data.json");
    if (!File.Exists(jsonPath))
    {

        app.Logger.LogInformation("Downloading data for the first time, please wait...");
        using var client = new HttpClient();

        var response = await client.GetAsync("https://kaikki.org/dictionary/raw-wiktextract-data.json.gz");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStreamAsync();
        using (var decompressed = new GZipStream(content, CompressionMode.Decompress))
        using (var jsonOutput = File.OpenWrite(jsonPath))
        {
            await decompressed.CopyToAsync(jsonOutput);
            await jsonOutput.FlushAsync();
        }

        app.Logger.LogInformation("Finished downloading data, will pre-process now, please wait...");
    }

    var languages = new HashSet<string>();

    int k = 0;

    var writeOptions = new WriteOptions();
    writeOptions.DisableWal(1).SetSync(false);

    using(var f = File.OpenRead(jsonPath))
    using (var reader = new StreamReader(f))
    {
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var json = JsonSerializer.Deserialize<WordDefinition>(line);
            if (json.LangCode is null || json.Word is null) continue;

            k++;

            if (k % 10000 == 0)
            {
                Console.WriteLine($"At {k:n0}, read [{json.LangCode}] [{json.Lang}] {json.Word}");
                db.Flush(new FlushOptions().SetWaitForFlush(false));
            }

            if(k % 100_000 == 0)
            {
            }

            if (languages.Add(json.LangCode))
            {
                db.Put(json.LangCode, json.Lang, langColumn, writeOptions);
            }

            var wordKey = Encoding.UTF8.GetBytes($"{json.LangCode}-{json.Word}");

            var definitions = db.Get(wordKey, WordDefinitionListSerializer.Instance, wordColumn) ?? new();
            
            definitions.Add(json);

            db.Put(wordKey, WordDefinitionListSerializer.Serialize(definitions), wordColumn, writeOptions);
        }

        db.CompactRange(null, null, langColumn);
        db.CompactRange(null, null, wordColumn);
    }

    app.Logger.LogInformation("Finished pre-processing data.");
}


void ForceInvariantCultureAndUTF8Output()
{
    bool consoleAvailable;
    try
    {
        Console.OutputEncoding = Encoding.UTF8;
        consoleAvailable = true;
    }
    catch
    {
        //This might throw if not running on a console, ignore as we don't care in that case
        consoleAvailable = false;
    }

    if (consoleAvailable)
    {
        try
        {
            Console.InputEncoding = Encoding.UTF8;
        }
        catch
        {
            //This might throw if not running on a console that reads input, ignore as we don't care in that case
        }
    }

    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
}
