# slack-send
A simple utility to send a Slack message using Slack Web API to your channels

![slack-send](https://raw.githubusercontent.com/cavecafe/slack-send/main/slack-send.png)

## Installation
```bash
> sudo snap install slack-send
```
#### or

(https://snapcraft.io/slack-send)

![Get it from the Snap Store](https://raw.githubusercontent.com/cavecafe/slack-send/main/snap/snap-store-white.svg)

#### or

```bash
# use it in your project directly
> dotnet add package slack-send
```


## Usage

```bash
slack-send --help

  -h, --help        (default: false) Prints this help message.
  -c, --channels    (default: 'general') Slack channels to send message, separated by ';', default is empty, which means use the channels in the configuration file.
  -d, --desc        (default: 'my description') description on the top of the message
  -t, --title       (default: 'my title') title to send
  -m, --message     (default: 'message to send') message body to send
  -s, --status      (default: 'good') Status good | warning | error, default is good
  -j, --json        (default: empty) JSON formated Slack message (https://api.slack.com/reference/messaging/attachments), it overrides the message and status options, if those values exist on JSON.
  --help            Display this help screen.
  --version         Display version information.
```

### Example
```bash
# slack-send -m <message> -c <channel>
> slack-send -m "Hello, My Slack Channel!" -c "general"
```

#### or
```bash
# slack-send -m <message> -c <channel1>;<channel2> ...
> slack-send -m "Hello, My Slack Channel!" -c "general;random"
```

#### or
```bash
# slack-send -d <description> -t <title> -m <message> -c <channel1>;<channel2> ...
> slack-send -d "This is a description" -t "This is a title" -m "Hello, My Slack Channel!" -c "general;random"
```

#### or
```bash
# slack-send -d <description> -t <title> -m <message> -c <channel1>;<channel2> ... -j <json_string>
> slack-send -d "This is a description" -t "This is a title" \ 
  -m "Hello, My Slack Channel!" \
  -c "general;random" \ 
  -j "{\"color\":\"#36a64f\",\"pretext\":\"Optional pre-text that appears above the attachment block\",\"author_name\":\"author_name\",\"author_link\":\"https://flickr.com/bobby/\",\"author_icon\":\"https://placeimg.com/16/16/people\",\"title\":\"title\",\"title_link\":\"https://api.slack.com/\",\"text\":\"Optional `text` that appears within the attachment\",\"fields\":[{\"title\":\"A field's title\",\"value\":\"This field's value\",\"short\":false},{\"title\":\"A short field's title\",\"value\":\"A short field's value\",\"short\":true},{\"title\":\"A second short field's title\",\"value\":\"A second short field's value\",\"short\":true}],\"thumb_url\":\"https://placekitten.com/g/200/200\",\"footer\":\"footer\",\"footer_icon\":\"https://platform.slack-edge.com/img/default_application_icon.png\",\"ts\":123456789}"
```

#### Used with GitHub Actions events
![Message with JSON payload](https://raw.githubusercontent.com/cavecafe/slack-send/main/images/message.png)


## Note
- You need to acquire your [Slack API token](https://api.slack.com/messaging/sending) from Slack in order for slack-send to send messages to your Slack channels, typically starts with 'xoxb-'.
- The configuration file `~/.slack-send` will be created when the app run first time to store your configuration.


## Configuration file (~/.slack-send)
You can also update the ~/.slack-send to set the default values for the message, channel, and attachments.
  - `ApiToken`: Your Slack API Key for a bot, typically starts with 'xoxb-'.
  - `ApiUrl`: The Slack API URL for sending messages.
  - `Channels`: An array of channel names to send the message to.
  - `Attachments`: An array of attachments to send with the message. The format of the attachments is the same as the JSON format for attachments in the Slack API.

#### Example:
```json
{
  "ApiToken": "<your Slack API Key for a bot, typically starts with 'xoxb-'>", 
  "ApiUrl": "https://slack.com/api/chat.postMessage",
  "Channels": [ "<your channel #1>", "<your channel #2>" ],
  "Attachments": [
    {
      "author_name": "<your sender name, i.e. GitHub Actions>",
      "author_icon": "https://github.com/fluidicon.png"
    }
  ]
}
```

## 'Attachments' JSON Format
If you prefer to have fine control of your Slack message, you can use the Slack's own JSON format ([Attachments](https://api.slack.com/reference/messaging/attachments)) directly, which will override 
the message and status options in '~/.slack-send'.

#### Example:
```json
{
    "channel": "ABCDEBF1",
    "attachments": 
    [
        {
            "mrkdwn_in": ["text"],
            "color": "#36a64f",
            "pretext": "Optional pre-text that appears above the attachment block",
            "author_name": "author_name",
            "author_link": "https://flickr.com/bobby/",
            "author_icon": "https://placeimg.com/16/16/people",
            "title": "title",
            "title_link": "https://api.slack.com/",
            "text": "Optional `text` that appears within the attachment",
            "fields": 
            [
                {
                    "title": "A field's title",
                    "value": "This field's value",
                    "short": false
                },
                {
                    "title": "A short field's title",
                    "value": "A short field's value",
                    "short": true
                },
                {
                    "title": "A second short field's title",
                    "value": "A second short field's value",
                    "short": true
                }
            ],
            "thumb_url": "https://placekitten.com/g/200/200",
            "footer": "footer",
            "footer_icon": "https://platform.slack-edge.com/img/default_application_icon.png",
            "ts": 123456789
        }
    ]
}
```

![Reference to Slack Web API](https://raw.githubusercontent.com/cavecafe/slack-send/main/images/slack-api.png)

## Reference
- https://api.slack.com/messaging/sending#publishing
- https://api.slack.com/methods/chat.postMessage
- https://api.slack.com/docs/message-attachments
