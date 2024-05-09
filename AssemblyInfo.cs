using SlackSend;

[assembly: BuildNumber("13")]

namespace SlackSend;

public class BuildNumberAttribute(string s) : Attribute
{
    string BuildNumber { get; set; } = s;
}