using System.Text;
using MQTTClient.Command;
using MQTTnet;
using MQTTnet.Client;

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();

var mwttClientOption = new MqttClientOptionsBuilder()
  .WithTcpServer("localhost")
  .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
  .Build();

CommandEnum? command = null;
List<string> subscriptions = new List<string>();
string feedbackMessage = "";
do {
  command = AskForCommand.Execute(feedbackMessage, subscriptions);
  feedbackMessage = command switch {
    CommandEnum.Connect => await ConnectAsync(),
    CommandEnum.Disconnect => await DisconnectAsync(),
    CommandEnum.Subscribe => await SubscribeAsync(),
    CommandEnum.Unsubscribe => await UnsubscribeAsync(),
    CommandEnum.Publish => await PublishAsync(),
    _ => "Quit"
  };
}
while (command != CommandEnum.Quit);

static Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e) {
  string json = Newtonsoft.Json.JsonConvert.SerializeObject(e.ApplicationMessage, Newtonsoft.Json.Formatting.Indented);
  string message = Encoding.Default.GetString(e.ApplicationMessage.PayloadSegment);
  Console.WriteLine($" message from {e.ClientId}  => {message}");
  return Task.CompletedTask;
}

async Task<string> ConnectAsync(){
  await mqttClient.ConnectAsync(mwttClientOption, CancellationToken.None);
  mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;
  return "MQTTClient connected";
}

async Task<string> DisconnectAsync(){
  await mqttClient.DisconnectAsync();
  mqttClient.ApplicationMessageReceivedAsync -= ApplicationMessageReceivedAsync;
  return "MQTTClient disconnected";
}

async Task<string> SubscribeAsync() {
  Console.Write("Enter topic to subscribe: ");
  string? topic = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(topic))
    return "Subscription cancelled";
  var opt = new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build();
  var result = await mqttClient.SubscribeAsync(opt, CancellationToken.None);
  subscriptions.Add(topic);
  return "Subscription reason:" + result.ReasonString;
}

async Task<string> UnsubscribeAsync() {
  Console.Write("Enter topic to unsubscribe: ");
  string? topic = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(topic))
    return "Unsubscription cancelled";
  var opt = new MqttClientUnsubscribeOptionsBuilder().WithTopicFilter(topic).Build();
  var result = await mqttClient.UnsubscribeAsync(opt, CancellationToken.None);
  subscriptions.Remove(topic);
  return "Unsubscription reason:" + result.ReasonString;
}

async Task<string> PublishAsync(){
  Console.Write("Enter topic: ");
  string? topic = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(topic))
    return "Publish cancelled";
  
  Console.Write("Enter message: ");
  string? message = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(message))
    return "Publish cancelled";

  await mqttClient.PublishStringAsync(topic,message);
  return "Publish done!";
}

