using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FastStdf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using WhiteLabel.STDF.Models;
using FastStdf.IO;
using FastStdf.Records;
using FastStdf.Records.Mir;

namespace WhiteLabel.STDF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private DispatcherTimer _timer;
	private readonly ILogger<MainWindow> _logger;
	private readonly IMapper _mapper;
	private readonly string _fileFormat = "| findstr \"stdf\"";

	public MainWindow(ILogger<MainWindow> logger, IMapper mapper)
    {
		_logger = logger;
		_mapper = mapper;
        InitializeComponent();
		sourceText.Text = "LPX-76/user/";
		destinationText.Text = "\"/cygdrive/d/STDF Files/STDF/\"";
	}

	private void startButton_Click(object sender, RoutedEventArgs e)
	{
		string source = sourceText.Text;
		string destination = destinationText.Text;

		if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
		{
			MessageBox.Show("Please provide both source and destination paths.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		// Initialize and start the timer
		if (_timer == null)
		{
			_timer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromMilliseconds(500) // Set the interval to 1 sec
			};

			_timer.Tick += (sender, args) =>
			{
				try
				{
					//RunAsync(source, destination);
					RunWithLatestFile(source, destination);
					AppendTextToRichTextBox(statusText, $"Last sync Time: {DateTime.Now}");
					_logger.LogInformation($"************************************************Sync starts on {DateTime.Now}***************************************");
					_logger.LogInformation($"Last sync Time: {DateTime.Now}");
				}
				catch (Exception ex)
				{
					AppendTextToRichTextBox(statusText, $"An error exception in Run Rsync timer: {ex.Message}");
					_timer.Stop();
				}
			};
			_timer.Start();
			AppendTextToRichTextBox(statusText, "************************************************Sync started***************************************");
			_logger.LogInformation("************************************************Sync started***************************************");
		}
	}

	private void RunWithLatestFile(string remotePath, string destinationPath)
	{
		string _latestFile = GetLatestFileName(remotePath);
		if (_latestFile != null)
		{
			AppendTextToRichTextBox(statusText, $"File Path: {_latestFile}");
			_logger.LogInformation($"File Path: {_latestFile}");
			RunAsync(_latestFile, destinationPath);
		}
		else
		{
			AppendTextToRichTextBox(statusText, "No file to sync.");
			_logger.LogInformation($"No file to sync.");
		}
	}

	private string GetLatestFileName(string remotePath)
	{
		try
		{
			// Run rsync to list files in the remote directory
			Process _process = new Process()
			{
				StartInfo =
					{
						FileName = "rsync",
						Arguments = $"--include='*stdf*' --exclude='*' rsync://{remotePath}",
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						UseShellExecute = false,
						CreateNoWindow = true,
					}
			};
			AppendTextToRichTextBox(statusText, $"Listing files in: rsync://{remotePath}");
			_logger.LogInformation($"File Path: {_process.StartInfo.Arguments}");
			_logger.LogInformation($"Listing files in: rsync://{remotePath}");
			_process.Start();

			string output = _process.StandardOutput.ReadToEnd();
			string error = _process.StandardError.ReadToEnd();
			_process.WaitForExit();

			if (_process.ExitCode != 0)
			{
				AppendTextToRichTextBox(statusText, $"Error listing files: {error}");
				_logger.LogError($"Error listing files: {error}");
				return null;
			}

			// Normalize line endings to ensure splitting works
			output = output.Replace("\r\n", "\n").Replace("\r", "\n");

			// Parse the output to find the latest file
			string[] lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			DateTime latestTime = DateTime.MinValue;
			string latestFile = null;

			foreach (var line in lines)
			{
				if (line.StartsWith("-rw") || line.StartsWith("----------")) // Indicates a file
				{
					// Split the line into parts
					var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					// Ensure there are enough parts for parsing
					if (parts.Length > 4)
					{
						// Extract timestamp (combine date and time) and filename
						string date = parts[^3]; // Third-to-last part is the date
						string time = parts[^2]; // Second-to-last part is the time
						string fileName = parts[^1]; // Last part is the file name

						// Parse the timestamp
						if (DateTime.TryParse($"{date} {time}", out var fileTime))
						{
							// Check if this file is the latest
							if (fileTime > latestTime)
							{
								latestTime = fileTime;
								latestFile = fileName;
							}
						}
					}
				}
			}


			if (latestFile != null)
			{
				AppendTextToRichTextBox(statusText, $"Latest file: {latestFile}, Last Modified: {latestTime}");
				_logger.LogInformation($"Latest file: {latestFile}, Last Modified: {latestTime}");
				return $"{remotePath}{latestFile}";
			}
			else
			{
				AppendTextToRichTextBox(statusText, "No files found.");
				_logger.LogError("No files found.");
				return null;
			}
		}
		catch (Exception ex)
		{
			AppendTextToRichTextBox(statusText, $"Error: {ex.Message}");
			_logger.LogError($"Error: {ex.Message}");
			return null;
		}
	}

	private void RunAsync(string sourcePath, string destinationPath)
	{
		try
		{
			DateTime _fileChangedTime = default;
			// Initialize the rsync process
			Process _rsyncProcess = new Process()
			{
				StartInfo =
					{
						FileName = "rsync",
						Arguments = $"-av --update --append-verify --inplace --itemize-changes --progress rsync://{sourcePath} {destinationPath}",
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						UseShellExecute = false,
						CreateNoWindow = true
					}
			};

			AppendTextToRichTextBox(statusText, $"Executing command: rsync {_rsyncProcess.StartInfo.Arguments}");
			_logger.LogInformation($"Executing command: rsync {_rsyncProcess.StartInfo.Arguments}");
			_rsyncProcess.Start();
			_fileChangedTime = DateTime.Now;
			string _output = _rsyncProcess.StandardOutput.ReadToEnd();
			string _error = _rsyncProcess.StandardError.ReadToEnd();

			// Wait for the rsync process to complete
			_rsyncProcess.WaitForExit();

			if (_rsyncProcess.ExitCode != 0)
			{
				AppendTextToRichTextBox(statusText, $"Rsync failed with error: {_error}");
				_logger.LogInformation($"Rsync failed with error: {_error}");
			}
			else
			{
				AppendTextToRichTextBox(statusText, $"Rsync completed - {DateTime.Now}");
				_logger.LogInformation($"Rsync completed - {DateTime.Now}");
				DateTime _SyncTime = DateTime.Now;
				AppendTextToRichTextBox(statusText, $"Processing deltas: {DateTime.Now}");
				_logger.LogInformation($"Processing deltas: {DateTime.Now}");

				// Identity the changed files from rsync output
				string[] deltaLines = _output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				AppendTextToRichTextBox(statusText, _output);

				Parallel.ForEach(deltaLines, line =>
				{
					if (line.Contains(">f"))
					{
						var _changedFile = ExtractFileNameFromRsyncOutput(line, destinationPath);
						// Normalize and validate the file path
						_changedFile = _changedFile.Trim().Replace("\"", "").Replace("/", "\\").TrimEnd('\\');
						//byte[] fileByte = File.ReadAllBytes(_changedFile);
						//DefaultFileStreamManager _file = new DefaultFileStreamManager(_changedFile);
						AppendTextToRichTextBox(statusText, $"File Bytes: {DateTime.Now}");
						_logger.LogInformation($"File Bytes: {DateTime.Now}");
						ProcessFile(_changedFile, _SyncTime);
					}
				});

				//var tasks = deltaLines
				//		.Where(line => line.Contains(">f"))
				//		.Select(async line =>
				//		{
				//			var _changedFile = ExtractFileNameFromRsyncOutput(line, destinationPath);
				//			_changedFile = _changedFile.Trim().Replace("\"", "").Replace("/", "\\").TrimEnd('\\');
				//			byte[] fileByte = File.ReadAllBytes(_changedFile);
				//			AppendTextToRichTextBox(statusText, $"File Bytes: {DateTime.Now}");
				//			_logger.LogInformation($"File Bytes: {DateTime.Now}");
				//			await ProcessFile(fileByte, _SyncTime);
				//		});

				//await Task.WhenAll(tasks);

				//await Parallel.ForEachAsync(deltaLines, async (line, cancellationToken) =>
				//{
				//	if (line.Contains(">f"))
				//	{
				//		var _changedFile = ExtractFileNameFromRsyncOutput(line, destinationPath);
				//		// Normalize and validate the file path
				//		_changedFile = _changedFile.Trim().Replace("\"", "").Replace("/", "\\").TrimEnd('\\');
				//		byte[] fileByte = File.ReadAllBytes(_changedFile);
				//		AppendTextToRichTextBox(statusText, $"File Bytes: {DateTime.Now}");
				//		_logger.LogInformation($"File Bytes: {DateTime.Now}");
				//		await ProcessFile(fileByte, _SyncTime);
				//	}
				//});
			}
		}
		catch (Exception ex)
		{
			AppendTextToRichTextBox(statusText, ex.Message);
		}
	}

	private async void ProcessFile(string filePath, DateTime lastSyncTime)
	{
		try
		{
			MirRecord _mirRecord;
			List<PrrRecord> prrRecords;
			byte[] _fileBytes = File.ReadAllBytes(filePath);
			AppendTextToRichTextBox(statusText, $"Processing file starts");
			_logger.LogInformation($"Processing file starts");

			using var _stream = new MemoryStream(_fileBytes);
			using var _fileReader = new StdfReader(_stream);

			while (true)
			{
				var _records = await _fileReader.ReadRecordAsync();
				if (_records == null)
					break;

				if(_records is Mir mir)
				{
					_logger.LogInformation($"MIR Records: {mir}");
				}
			}

			//var stdfFile = new StdfFile(fileByte);
			#region GET THE MIR RECORD
			//var _mir = await Task.Run(() => stdfFile.GetMir());
			//var _mir = stdfFile.GetMir();
			//_mirRecord = _mapper.Map<MirRecord>(_mir);
			#endregion

			#region GET THE PRR RECORD AND ADD LAST MODIFIED DATE 
			//prrRecords = stdfFile.GetRecords()
			//				   .OfExactType<Prr>()
			//				   .AsParallel()
			//				   .WithDegreeOfParallelism(Environment.ProcessorCount)
			//				   .Select(prr => new PrrRecord
			//				   {
			//					   HeadNumber = prr.HeadNumber,
			//					   SiteNumber = prr.SiteNumber,
			//					   PartFlag = prr.PartFlag,
			//					   TestCount = prr.TestCount,
			//					   HardBin = prr.HardBin,
			//					   SoftBin = prr.SoftBin,
			//					   XCoordinate = prr.XCoordinate,
			//					   YCoordinate = prr.YCoordinate,
			//					   TestTime = prr.TestTime,
			//					   PartId = prr.PartId,
			//					   PartText = prr.PartText,
			//					   PartFix = prr.PartFix,
			//					   AbnormalTest = prr.AbnormalTest,
			//					   Failed = prr.Failed,
			//					   LastModified = lastSyncTime,
			//				   }).ToList();
			// Fetch PRR records in parallel
			//prrRecords = await Task.Run(() =>
			//{
			//	return stdfFile.GetRecords()
			//				   .OfExactType<Prr>()
			//				   .AsParallel()
			//				   .WithDegreeOfParallelism(Environment.ProcessorCount)
			//				   .Select(prr => new PrrRecord
			//				   {
			//					   HeadNumber = prr.HeadNumber,
			//					   SiteNumber = prr.SiteNumber,
			//					   PartFlag = prr.PartFlag,
			//					   TestCount = prr.TestCount,
			//					   HardBin = prr.HardBin,
			//					   SoftBin = prr.SoftBin,
			//					   XCoordinate = prr.XCoordinate,
			//					   YCoordinate = prr.YCoordinate,
			//					   TestTime = prr.TestTime,
			//					   PartId = prr.PartId,
			//					   PartText = prr.PartText,
			//					   PartFix = prr.PartFix,
			//					   AbnormalTest = prr.AbnormalTest,
			//					   Failed = prr.Failed,
			//					   LastModified = lastSyncTime,
			//				   }).ToList();
			//});
			#endregion
			//}
			//SavePrrDataToJson(prrRecords, _mirRecord);
		}
		catch (Exception ex)
		{
			AppendTextToRichTextBox(statusText, $"Process File Exception: {ex.Message}");
			_logger.LogError($"Process File Exception: {ex.Message}");
		}
	}

	private string ExtractFileNameFromRsyncOutput(string rsyncOutputLine, string destinationPath)
	{
		// Trim leading/trailing spaces
		rsyncOutputLine = rsyncOutputLine.Trim();

		// Ensure the line starts with >f
		if (!rsyncOutputLine.Contains(">f"))
			throw new ArgumentException("Invalid rsync output line format. Expected a line starting with '>f'.");

		// Split by spaces to extract the last part (filename)
		string[] parts = rsyncOutputLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		//string fileName = parts[^1]; // Use the last element as the file name
		string fileName = parts[4]; // If file name is in position 4

		// Normalize destination path and remove trailing quotes
		string normalizedDestinationPath = destinationPath.Replace("/cygdrive/d/", "D:/")
														   .Replace("/", "\\")
														   .Trim('"', '\\'); // Remove trailing quotes and slashes

		// Combine the normalized destination path with the file name
		string finalPath = Path.Combine(normalizedDestinationPath, fileName);

		// Remove quotes from the combined path, if any
		return finalPath.Trim('"');
	}

	private async void AppendTextToRichTextBox(RichTextBox richTextBox, string text)
	{
		await richTextBox.Dispatcher.InvokeAsync(() =>
		{
			richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
			richTextBox.ScrollToEnd(); // Scrolls to the end to show the latest message
		});
	}

	private void SavePrrDataToJson(List<PrrRecord> prrRecords, MirRecord mirRecord)
	{
		try
		{
			string _filePath = @"D:\STDF Files\STDF\Output\" + mirRecord.NodeName + "_" + mirRecord.LotId + ".json";
			// Initialize a list to hold all PRR records
			List<PrrRecord> _prrRecords;

			// Check if JSON file already exists
			if (File.Exists(_filePath))
			{
				// Read the existing JSON data
				var _existingJsonData = File.ReadAllText(_filePath);

				// Deserialize the existing JSON into a list of PRRRecords
				var _existingData = JsonConvert.DeserializeObject<JsonWrapper>(_existingJsonData);

				// Retrieve the existing PRR records if present, or initialize an empty list
				_prrRecords = _existingData?.Prr ?? new List<PrrRecord>();
			}
			else
				// If file doesn't exist, Start with an empty list
				_prrRecords = new List<PrrRecord>();

			// build a Hashset of existing PART_ID
			var _existingPartID = new HashSet<string>(_prrRecords.Select(record => record.PartId));

			// Filter new records to include only those with unique PART_ID
			var _newRecords = prrRecords.Where(x => !_existingPartID.Contains(x.PartId)).ToList();

			if (_newRecords.Any())
			{
				// Append only new PRR Records
				_prrRecords.AddRange(_newRecords);

				// Prepare the JSON output
				var _jsonOutput = new JsonWrapper
				{
					Mir = mirRecord,
					Prr = _prrRecords
				};

				// Serialize the JSON with indentation
				var _jsonString = JsonConvert.SerializeObject(_jsonOutput, Newtonsoft.Json.Formatting.Indented);



				//var jsonFilePath = Path.Combine(@"C:\Users\11086717\Desktop\STDF Files\TestData", $"{Path.GetFileNameWithoutExtension(filePath)}.json");
				File.WriteAllText(_filePath, _jsonString);

				AppendTextToRichTextBox(statusText, $"PRR data saved to JSON: {_filePath} and Date: {DateTime.Now}");
				_logger.LogInformation($"PRR data saved to JSON: {_filePath} and Date: {DateTime.Now}");
			}
			else
			{
				AppendTextToRichTextBox(statusText, "No new PRR records to append. File remains unchanged.");
				_logger.LogInformation("No new PRR records to append. File remains unchanged.");
			}
		}
		catch (Exception ex)
		{
			AppendTextToRichTextBox(statusText, $"SavePrrDataToJson - An exception has been occurred in {ex.Message}");
			_logger.LogInformation($"SavePrrDataToJson - An exception has been occurred in {ex.Message}");
		}
	}
}