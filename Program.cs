using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Util
{
    class Slack
    {
        private static readonly HttpClient client = new HttpClient();
        private static Settings? settings;


        public class Settings
        {
            public string? ApiToken { get; set; }
            public string? ApiUrl { get; set; }
            public string? Channels { get; set; }
        }

        public class Message
        {
            public string? channel { get; set; }
            public string? text { get; set; }
            public bool as_user { get; set; }
            public Attachment[]? attachments { get; set; }
        }

        public class MessageResponse
        {
            public bool ok { get; set; }
            public string? error { get; set; }
            public string? channel { get; set; }
            public string? ts { get; set; }
        }


        public class Attachment
        {
            public string? fallback { get; set; }
            public string? text { get; set; }
            public string? image_url { get; set; }
            public string? color { get; set; }
        }


        public static bool Init()
        {
            bool ret = false;
            try
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string dir = Path.GetDirectoryName(path)!;

                string json = File.ReadAllText(Path.Combine(dir, "slack.settings.json"));
                settings = JsonSerializer.Deserialize<Settings>(json);
                ret = settings != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return ret;
        }

        public static async Task<bool> SendMessageAsync(string token, Message msg)
        {
            bool ret = false;
            var content = JsonSerializer.Serialize(msg);
            var httpContent = new StringContent(
                content,
                Encoding.UTF8,
                "application/json"
            );

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var res = await client.PostAsync(settings!.ApiUrl, httpContent);
            var json = await res.Content.ReadAsStringAsync();
            try
            {
                MessageResponse? msgRes = JsonSerializer.Deserialize<MessageResponse>(json);
                ret = (msgRes != null && msgRes.ok);
            }
            catch { }

            return ret;
        }

        static async Task Main(string[] args)
        {
            if (!Init())
            {
                Console.WriteLine("slack.settings.json NOT found");
                return;
            }

            args = new string[] { "message title", "good", "detailed description" };

            if (args.Length == 0 || args.Length == 2)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string fileName = Path.GetFileName(path)!;
                Console.WriteLine($"usage: {fileName} <message> [<good|bad> <additional message>]");
                return;
            }

            Attachment? attached = null;
            if (args.Length >= 3)
            {
                bool isGood = (args[1].ToLower() == "good");
                attached = new Attachment
                {
                    fallback = "fall-back",
                    text = args[2],
                    color = isGood ? "good" : "danger",
                    image_url = ""
                };
            }

            string[] channels = settings!.Channels!.Split(';');

            foreach (string channel in channels)
            {
                var msg = new Message
                {
                    channel = channel,
                    text = args[0],
                    as_user = true,
                    attachments = new Attachment[] { }
                };

                if (attached != null)
                {
                    msg.attachments.Append(attached);
                }

                bool result = await SendMessageAsync(settings.ApiToken!, msg);
                Console.WriteLine(result ? $"message has been sent to '{channel}'" : "failed to send message");
            }
        }
    }
}
