using SlackSend;

[assembly: BuildNumber("23")]

namespace SlackSend;

public class BuildNumberAttribute(string s) : Attribute
{
    string BuildNumber { get; set; } = s;
}
