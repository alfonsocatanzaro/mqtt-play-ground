using MQTTnet;
using MQTTnet.Server;

var mqttFactory = new MqttFactory();
var mqttServerOptions = new MqttServerOptionsBuilder()
  .WithDefaultEndpoint()
  .WithPersistentSessions()
  .Build();

using var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

await mqttServer.StartAsync();

mqttServer.InterceptingPublishAsync += (e) => Task.Run(() => 
  Console.WriteLine($"InterceptingPublishAsync        - ClientId: {e.ClientId} {e.ApplicationMessage.Topic} {e.ApplicationMessage.ConvertPayloadToString()}")
);

mqttServer.ClientConnectedAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ClientConnectedAsync            - ClientId: {e.ClientId} EndPoint {e.Endpoint} Username: {e.UserName}")
);

mqttServer.ClientDisconnectedAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ClientDisconnectedAsync         - ClientId: {e.ClientId} EndPoint ${e.Endpoint} DisconnectType: {e.DisconnectType} ReasonString: ${e.ReasonString}")
);

mqttServer.ClientSubscribedTopicAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ClientSubscribedTopicAsync      - ClientId: {e.ClientId} TopicFilter: {e.TopicFilter.Topic}")
);

mqttServer.ClientUnsubscribedTopicAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ClientUnsubscribedTopicAsync    - ClientId: {e.ClientId} TopicFilter: {e.TopicFilter}")
);

mqttServer.ClientAcknowledgedPublishPacketAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ClientAcknowledgedPublishPacket - ClientId: {e.ClientId}")
);

mqttServer.ApplicationMessageNotConsumedAsync += (e) => Task.Run(() =>
  Console.WriteLine($"ApplicationMessageNotConsumed   -")
);

Console.WriteLine("MQTTServer started");
Console.ReadLine();

await mqttServer.StopAsync();
Console.WriteLine("MQTTServer Stopped!");