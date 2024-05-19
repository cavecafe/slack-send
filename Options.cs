using CommandLine;

namespace SlackSend;

public class Options
{
    public const string DefaultDescription = "my description";
    public const string DefaultTitle = "my title";
    public const string DefaultMessage = "message to send";
    public const string DefaultStatus = "good";
    public const string DefaultChannels = "general";
    public const string DefaultAuthor = "slack-send";
    public const string DefaultFieldTitle = "my field title";
    public const string DefaultFieldValue = "my field value";
    
    [Option('h', "help", Required = false, Default = false, HelpText = "Prints this help message.")]
    public bool Help { get; set; } = false;
    
    [Option('c', "channels", Required = false, Default = DefaultChannels,
        HelpText =
            "Slack channels to send message, separated by ';', default is empty, which means use the channels in the configuration file.")]
    public string Channels { get; set; } = DefaultChannels;
    
    [Option('d', "desc", Required = false, Default = DefaultDescription, HelpText = "description on the top of the message")]
    public string Description { get; set; } = DefaultDescription;

    [Option('t', "title", Required = false, Default = DefaultTitle, HelpText = "title to send")]
    public string Title { get; set; } = DefaultTitle;
    
    [Option('m', "message", Required = false, HelpText = DefaultMessage)]
    public string Message { get; set; } = DefaultMessage;

    [Option('s', "status", Required = false, Default = DefaultStatus, HelpText = "Status good | warning | error, default is good")]
    public string Status { get; set; } = DefaultStatus;

    [Option('j', "json", Required = false, Default = "", HelpText = "JSON formated Slack message (https://api.slack.com/reference/messaging/attachments), it overrides the message and status options, if those values exist on JSON.")]
    public string? Json { get; set; } = "";
    
}