using CommandLine;

namespace SlackSend;

public class Options
{
    [Option('h', "help", Required = false, Default = false, HelpText = "Prints this help message.")]
    public bool Help { get; set; } = false;
    
    [Option('c', "channels", Required = false, Default = "general",
        HelpText =
            "Slack channels to send message, separated by ';', default is empty, which means use the channels in the configuration file.")]
    public string Channels { get; set; } = "general";
    
    [Option('d', "desc", Required = false, Default = "my description", HelpText = "description on the top of the message")]
    public string Description { get; set; } = "my description";

    [Option('t', "title", Required = false, Default = "my title", HelpText = "title to send")]
    public string Title { get; set; } = "my title";
    
    [Option('m', "message", Required = false, HelpText = "message to send")]
    public string Message { get; set; } = "message to send";

    [Option('s', "status", Required = false, Default = "good", HelpText = "Status good | warning | error, default is good")]
    public string Status { get; set; } = "good";

    [Option('j', "json", Required = false, Default = "", HelpText = "JSON formated Slack message (https://api.slack.com/reference/messaging/attachments), it overrides the message and status options, if those values exist on JSON.")]
    public string? Json { get; set; } = "";
}