using System.Diagnostics;
using CommandLine;
using SlackSend;

var fullPath = Process.GetCurrentProcess().MainModule?.FileName;
if (fullPath != null)
{
    var slack = new SlackClient(fullPath);
    if (slack is { Settings: not null })
    {
        var parser = new Parser(with =>
        {
            with.EnableDashDash = true;
            with.AutoHelp = true;
            with.HelpWriter = Console.Out;
        });
        
        var parserResult = parser.ParseArguments<Options>(args);
        parserResult.WithNotParsed(_ => { });
        parserResult.WithParsed(args =>
        {
            slack.Settings.UpdateWith(args);
            foreach (var channel in slack.Settings.Channels!)
            {
                var req = new MessageRequest
                {
                    channel = channel,
                    as_user = true,
                    attachments = slack.Settings.Attachments
                };
                var result = slack.SendMessageAsync(slack.Settings.ApiToken!, req).Result;
                Console.WriteLine($@"result: {result.Item2}");
            }
        });
    }
    else
    {
        Console.Error.WriteLine(@"failed to load the configuration file");
    }
}