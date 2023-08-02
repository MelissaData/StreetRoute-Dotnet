using Newtonsoft.Json;

namespace StreetRouteDotnet
{
  static class Program
  {
    static void Main(string[] args)
    {
      string baseServiceUrl = @"http://streetroute.melissadata.net/";
      string serviceEndpoint = @"v1/WEB/StreetRoute/getDistance";
      string license = "";
      string startLat = "";
      string startLong = "";
      string endLat = "";
      string endLong = "";

      ParseArguments(ref license, ref startLat, ref startLong, ref endLat, ref endLong, args);
      CallAPI(baseServiceUrl, serviceEndpoint, license, startLat, startLong, endLat, endLong);
    }

    static void ParseArguments(ref string license, ref string startLat, ref string startLong, ref string endLat, ref string endLong, string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i].Equals("--license") || args[i].Equals("-l"))
        {
          if (args[i + 1] != null)
          {
            license = args[i + 1];
          }
        }
        if (args[i].Equals("--startlat"))
        {
          if (args[i + 1] != null)
          {
            startLat = args[i + 1];
          }
        }
        if (args[i].Equals("--startlong"))
        {
          if (args[i + 1] != null)
          {
            startLong = args[i + 1];
          }
        }
        if (args[i].Equals("--endlat"))
        {
          if (args[i + 1] != null)
          {
            endLat = args[i + 1];
          }
        }
        if (args[i].Equals("--endlong"))
        {
          if (args[i + 1] != null)
          {
            endLong = args[i + 1];
          }
        }
      }
    }

    public static async Task GetContents(string baseServiceUrl, string requestQuery)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri(baseServiceUrl);
      HttpResponseMessage response = await client.GetAsync(requestQuery);

      string text = await response.Content.ReadAsStringAsync();

      var obj = JsonConvert.DeserializeObject(text);
      var prettyResponse = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

      // Print output
      Console.WriteLine("\n==================================== OUTPUT ====================================\n");
      
      Console.WriteLine("API Call: ");
      string APICall = Path.Combine(baseServiceUrl, requestQuery);
      for (int i = 0; i < APICall.Length; i += 70)
      {
        try
        {
          Console.WriteLine(APICall.Substring(i, 70));
        }
        catch
        {
          Console.WriteLine(APICall.Substring(i, APICall.Length - i));
        }
      }

      Console.WriteLine("\nAPI Response:");
      Console.WriteLine(prettyResponse);
    }
    
    static void CallAPI(string baseServiceUrl, string serviceEndPoint, string license, string startLat, string startLong, string endLat, string endLong)
    {
      Console.WriteLine("\n=================== WELCOME TO MELISSA STREET ROUTE CLOUD API ==================\n");
      
      bool shouldContinueRunning = true;
      while (shouldContinueRunning)
      {
        string inputStartLat = "";
        string inputStartLong = "";
        string inputEndLat = "";
        string inputEndLong = "";

        if (string.IsNullOrEmpty(startLat) && string.IsNullOrEmpty(startLong) && string.IsNullOrEmpty(endLat) && string.IsNullOrEmpty(endLong))
        {
          Console.WriteLine("\nFill in each value to see results");

          Console.Write("StartLatitude: ");
          inputStartLat = Console.ReadLine();

          Console.Write("StartLongitude: ");
          inputStartLong = Console.ReadLine();

          Console.Write("EndLatitude: ");
          inputEndLat = Console.ReadLine();

          Console.Write("EndLongitude: ");
          inputEndLong = Console.ReadLine();
        }
        else
        {
          inputStartLat = startLat;
          inputStartLong = startLong;
          inputEndLat = endLat;
          inputEndLong = endLong;
        }

        while (string.IsNullOrEmpty(inputStartLat) || string.IsNullOrEmpty(inputStartLong) || string.IsNullOrEmpty(inputEndLat) 
          || string.IsNullOrEmpty(inputEndLong))
        {
          Console.WriteLine("\nFill in missing required parameter");

          if (string.IsNullOrEmpty(inputStartLat))
          {
            Console.Write("StartLatitude: ");
            inputStartLat = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputStartLong))
          {
            Console.Write("StartLongitude: ");
            inputStartLong = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputEndLat))
          {
            Console.Write("EndLatitude: ");
            inputEndLat = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputEndLong))
          {
            Console.Write("EndLongitude: ");
            inputEndLong = Console.ReadLine();
          }
        }

        Dictionary<string, string> inputs = new Dictionary<string, string>()
        {
            { "format", "json"},
            { "StartLatitude", inputStartLat},
            { "StartLongitude", inputStartLong},
            { "EndLatitude", inputEndLat},
            { "EndLongitude", inputEndLong}   
        };

        Console.WriteLine("\n===================================== INPUTS ===================================\n");
        Console.WriteLine($"\t   Base Service Url: {baseServiceUrl}");
        Console.WriteLine($"\t  Service End Point: {serviceEndPoint}");
        Console.WriteLine($"\t      StartLatitude: {inputStartLat}");
        Console.WriteLine($"\t     StartLongitude: {inputStartLong}");
        Console.WriteLine($"\t        EndLatitude: {inputEndLat}");
        Console.WriteLine($"\t       EndLongitude: {inputEndLong}");

        // Create Service Call
        // Set the License String in the Request
        string RESTRequest = "";

        RESTRequest += @"&id=" + Uri.EscapeDataString(license);

        // Set the Input Parameters
        foreach (KeyValuePair<string, string> kvp in inputs)
          RESTRequest += @"&" + kvp.Key + "=" + Uri.EscapeDataString(kvp.Value);

        // Build the final REST String Query
        RESTRequest = serviceEndPoint + @"?" + RESTRequest;

        // Submit to the Web Service. 
        bool success = false;
        int retryCounter = 0;

        do
        {
          try //retry just in case of network failure
          {
            GetContents(baseServiceUrl, $"{RESTRequest}").Wait();
            Console.WriteLine();
            success = true;
          }
          catch (Exception ex)
          {
            retryCounter++;
            Console.WriteLine(ex.ToString());
            return;
          }
        } while ((success != true) && (retryCounter < 5));

        bool isValid = false;
        if (!string.IsNullOrEmpty(startLat + startLong + endLat + endLong))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

        while (!isValid)
        {
          Console.WriteLine("\nTest another record? (Y/N)");
          string testAnotherResponse = Console.ReadLine();

          if (!string.IsNullOrEmpty(testAnotherResponse))
          {
            testAnotherResponse = testAnotherResponse.ToLower();
            if (testAnotherResponse == "y")
            {
              isValid = true;
            }
            else if (testAnotherResponse == "n")
            {
              isValid = true;
              shouldContinueRunning = false;
            }
            else
            {
              Console.Write("Invalid Response, please respond 'Y' or 'N'");
            }
          }
        }
      }
      
      Console.WriteLine("\n===================== THANK YOU FOR USING MELISSA CLOUD API ====================\n");
    }
  }
}