using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SlackSend;

public class SlackConfiguration
{
    public string? ApiToken { get; init; }
    public string? ApiUrl { get; init; }
    public string[]? Channels { get; set; }
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
                attachment.color = options.Status;
            }
        }
        
        // override the message with the JSON string, which formated as MessageRequest
        if (!string.IsNullOrWhiteSpace(options.Json))
        {
            var json = JsonSerializer.Deserialize<MessageRequest>(options.Json);
            Channels = json?.channel.Split(';');
            Attachments = json?.attachments;
        }
    }
}

public class Attachment
{
    public List<string>? mrkdwn_in { get; set; }

    private static readonly Dictionary<string, string> colors = new()
    {
        { "warning", "#ffa500" },
        { "error", "#ff0000" },
        { "good", "#36a64f" }
    };
    
    private string _color = colors["good"];
    public string? color
    {
        get => _color;
        set => _color = value is not null ? colors[value] : colors["good"];
    }

    public string? pretext { get; set; } = "";
    public string? author_name { get; set; } = "";
    public string? author_link { get; set; } = "";
    public string? author_icon { get; set; } = "";
    public string? title { get; set; } = "";
    public string? title_link { get; set; } = "";
    public string? text { get; set; } = "";
    public List<Field>? fields { get; set; } = new();
    public string? thumb_url { get; set; } = "";
    public string? footer { get; set; } = "";
    public string? footer_icon { get; set; } = "";
    public int ts { get; set; } = 0;
}

public class Field
{
    public string? title { get; set; }
    public string? value { get; set; }
    public bool @short { get; set; }
}

public class MessageRequest
{
    public required string channel { get; init; }
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
        if (!Init(runPath, assemblyName))
        {
            Console.Error.WriteLine($"Failed to initialize SlackClient, please check the {assemblyName}.json file");
            Environment.Exit(1);
        }
    }
    private bool Init(string path, string? assemblyName)
    {
        bool ret = false;
        try
        {
            SlackConfiguration.FileName = $"{assemblyName}.json";
            string dir = Path.GetDirectoryName(path)!;
            string json = File.ReadAllText(Path.Combine(dir, SlackConfiguration.FileName));
            Settings = JsonSerializer.Deserialize<SlackConfiguration>(json);
            ret = Settings != null;
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
        var content = JsonSerializer.Serialize(req, typeof(MessageRequest));
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