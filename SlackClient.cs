using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SlackSend;

public class SlackConfiguration
{
    public string? ApiToken { get; init; }
    public string ApiUrl { get; init; } = "https://slack.com/api/chat.postMessage";
    public string?[]? Channels { get; set; }
    public Attachment[]? Attachments { get; set; }
    public static string? FileName { get; set; }

    public void UpdateWith(Options options)
    {
        if (string.IsNullOrWhiteSpace(options.Channels))
        {
            Console.Error.WriteLine($"Channels is required in the '{FileName}' or as an argument");
            Environment.Exit(1);
        }
        Channels = options.Channels.Split(';');
        if (string.IsNullOrWhiteSpace(options.Message))
        {
            Console.Error.WriteLine($"Message is required in the '{FileName}' or as an argument");
            Environment.Exit(1);
        }
        
        Attachments ??= [ new Attachment() ];
        foreach (var attachment in Attachments)
        {
            attachment.text = options.Message;
        }
        
        // Override the description with the desc option
        if (!string.IsNullOrWhiteSpace(options.Description))
        {
            foreach (var attachment in Attachments)
            {
                attachment.pretext = options.Description;
            }
        }
        
        // Override the title with the title option
        if (!string.IsNullOrWhiteSpace(options.Title))
        {
            foreach (var attachment in Attachments)
            {
                attachment.title = options.Title;
            }
        }
        
        // Override the color with the status option
        if (!string.IsNullOrWhiteSpace(options.Status))
        {
            foreach (var attachment in Attachments)
            {
                // if good warning or error, then use the pre-defined color, otherwise use the color as is unknown (grey)
                attachment.color = Attachment.PreDefinedColors.GetValueOrDefault(options.Status, "#808080");
            }
        }
        
        // override the message with the JSON string, which formated as MessageRequest
        if (!string.IsNullOrWhiteSpace(options.Json))
        {
            var json = JsonSerializer.Deserialize<MessageRequest>(options.Json);
            Channels = json?.channel?.Split(';');
            Attachments = json?.attachments;
        }
    }
}

public class Attachment
{
    public List<string>? mrkdwn_in { get; set; }

    internal static readonly Dictionary<string, string> PreDefinedColors = new()
    {
        { "warning", "#ffa500" },
        { "error", "#ff0000" },
        { Options.DefaultStatus, "#36a64f" }
    };
    
    public string color
    { get; set; } = PreDefinedColors[Options.DefaultStatus];
    
    public string pretext { get; set; } = Options.DefaultDescription;
    public string author_name { get; set; } = Options.DefaultAuthor;
    public string author_link { get; set; } = "https://snapcraft.io/slack-send/listing";
    public string author_icon { get; set; } = "https://dashboard.snapcraft.io/site_media/appmedia/2024/05/slack-send.png";
    public string title { get; set; } = Options.DefaultTitle;
    public string title_link { get; set; } = "https://api.slack.com/";
    public string text { get; set; } = Options.DefaultMessage;
    public List<Field>? fields { get; set; } = new();
    public string thumb_url { get; set; } = "";
    public string footer { get; set; } = "";
    public string footer_icon { get; set; } = "";
    public int ts { get; set; }
}

public class Field
{
    public string title { get; set; } = Options.DefaultFieldTitle;
    public string value { get; set; } = Options.DefaultFieldValue;
    public bool @short { get; set; }
}

public class MessageRequest
{
    public required string? channel { get; init; }
    public required Attachment[]? attachments { get; init; }
    public bool as_user { get; init; }
}

public class MessageResponse
{
    public bool ok { get; init; }
    public string? error { get; init; }
    public string? channel { get; init; }
    public string? ts { get; init; }
}

public class SlackClient : HttpClient
{
    public SlackConfiguration? Settings;

    public SlackClient(string runPath)
    {
        var fileName = Path.GetFileName(runPath);
        var assemblyName = Path.GetFileNameWithoutExtension(fileName);
        var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!Init(userHome, assemblyName))
        {
            Console.Error.WriteLine($"Failed to initialize SlackClient.\nPlease check the {userHome}/.{assemblyName} file");
            Environment.Exit(1);
        }
    }

    private bool Init(string path, string? assemblyName)
    {
        bool ret = false;
        try
        {
            SlackConfiguration.FileName = $".{assemblyName}";
            var configPath = Path.Combine(path, SlackConfiguration.FileName);
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                Settings = JsonSerializer.Deserialize<SlackConfiguration>(json);
                ret = Settings != null;
            }
            else
            {
                Console.Error.Write(
                    $"The configuration file '{configPath}' does not exist, \ndo you want to create it? [y/n]");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key == ConsoleKey.Y)
                {
                    Console.Write($@"Channel name [default: {Options.DefaultChannels}]:");
                    var channel = Console.ReadLine();
                    channel = string.IsNullOrWhiteSpace(channel) ? Options.DefaultChannels : channel;
                    
                    Console.Write($@"Slack message title [default: {Options.DefaultTitle}]: ");
                    var title = Console.ReadLine();
                    title = string.IsNullOrWhiteSpace(title) ? Options.DefaultTitle : title;
                    
                    Console.Write($@"Slack message body [default: {Options.DefaultMessage}]: ");
                    var message = Console.ReadLine();
                    message = string.IsNullOrWhiteSpace(message) ? Options.DefaultMessage : message;
                    
                    Console.Write($@"Slack message description [default: {Options.DefaultDescription}]: ");
                    var description = Console.ReadLine();
                    description = string.IsNullOrWhiteSpace(description) ? Options.DefaultDescription : description;
                    
                    Console.Write($@"Sender name [default: {Options.DefaultAuthor}]: ");
                    var senderName = Console.ReadLine();
                    senderName = string.IsNullOrWhiteSpace(senderName) ? Options.DefaultAuthor : senderName;
                    
                    Console.Write(@"Slack API token:");
                    var token = Console.ReadLine();

                    Settings = new SlackConfiguration
                    {
                        ApiToken = token,
                        Channels = [channel],
                        Attachments = [ 
                            new()
                            {
                                pretext = description,
                                title = title,
                                author_name = senderName,
                                text = message,
                                ts = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
                                fields = new List<Field> { new() },
                                mrkdwn_in = new List<string> { "text" }
                            }
                        ]
                    };

                    var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    });
                    File.WriteAllText(configPath, json);
                    Console.WriteLine($@"The configuration file '{configPath}' has been created.");
                    ret = true;
                }
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
        }

        return ret;
    }

    public async Task<(bool, string)> SendMessageAsync(string token, MessageRequest req)
    {
        bool ret = false;
        string details = string.Empty;
        var content = JsonSerializer.Serialize(req, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        var post = new StringContent(
            content,
            Encoding.UTF8,
            "application/json"
        );

        DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await PostAsync(Settings!.ApiUrl, post);
        var json = await result.Content.ReadAsStringAsync();
        try
        {
            MessageResponse? resp = JsonSerializer.Deserialize<MessageResponse>(json);
            ret = resp is { ok: true };
            details = ret ? $"message sent to channel '{req.channel}'" : $"failed to connect channel '{req.channel}', error='{resp?.error}'";
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
        }
        finally
        {
            result.Dispose();
        }

        return (ret, details);
    }
}