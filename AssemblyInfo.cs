using SlackSend;

[assembly: BuildNumber("10")]

namespace SlackSend;

public class BuildNumberAttribute(string s) : Attribute
{
    string BuildNumber { get; set; } = s;
}