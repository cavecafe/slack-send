using CommandLine;

namespace SlackSend;

public class Options
{
    [Option('h', "help", Required = false, Default = false, HelpText = "Prints this help message.")]
    public bool Help { get; set; } = false;
    
    [Option('c', "channels", Required = false, Default = "empty",
        HelpText =
            "Slack channels to send message, separated by ';', default is empty, which means use the channels in the configuration file.")]
    public string? Channels { get; set; } = "";
    
    [Option('d', "desc", Required = false, Default = "empty", HelpText = "description on the top of the message")]
    public string? Description { get; set; } = "";

    [Option('t', "title", Required = false, Default = "empty", HelpText = "title to send")]
    public string? Title { get; set; } = "";
    
    [Option('m', "message", Required = true, HelpText = "message to send")]
    public string? Message { get; set; } = "";

    [Option('s', "status", Required = false, Default = "good", HelpText = "Status good | warning | error, default is good")]
    public string? Status { get; set; } = "good";

    [Option('j', "json", Required = false, Default = "empty", HelpText = "JSON formated Slack message (https://api.slack.com/reference/messaging/attachments), it overrides the message and status options, if those values exist on JSON.")]
    public string? Json { get; set; } = "";
}