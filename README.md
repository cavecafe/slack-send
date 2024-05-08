# slack-send
Simple Utility to send a slack message by API key and channels

## Usage
```bash
slack-send -m <message>
# or
slack-send -m <message> -c <channel>
# or
slack-send -m <message> -c <channel1>;<channel2> ...
# or
slack-send -d <description> -t <title> -m <message> -c <channel1>;<channel2> ...
# or
slack-send -d <description> -t <title> -m <message> -c <channel1>;<channel2> ... -j <json>
```

## Configuration (slack-send.json)
```json
{
  "ApiToken": "xoxb-<your Slack API Key for a bot>", 
  "ApiUrl": "https://slack.com/api/chat.postMessage",
  "Channels": [ "<your channel #1>", "<your channel #1>", ... ],
  "Attachments": [
    {
      "author_name": "<your sender name, i.e. GitHub Actions>",
      "author_icon": "https://github.com/fluidicon.png"
    }
  ]
}
```

## 'Attachments' JSON Format
https://api.slack.com/reference/messaging/attachments

Example:
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
            "author_link": "http://flickr.com/bobby/",
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
            "thumb_url": "http://placekitten.com/g/200/200",
            "footer": "footer",
            "footer_icon": "https://platform.slack-edge.com/img/default_application_icon.png",
            "ts": 123456789
        }
    ]
}

```



## Example
```bash
slack-send -m "Hello, World!" -c "#general"
```

