namespace MQTTClient.Command;

public static class AskForCommand {
  public static CommandEnum Execute(string feedbackMessage, List<string> subscriptions) {
    Console.Clear();
    Console.WriteLine($"subscriptions: {string.Join(", ",subscriptions)}");
    Console.WriteLine("[1]Connect [2]Disonnect [3]Subscribe [4]Unsubscribe [5]Publish [0]Exit");
    Console.WriteLine(feedbackMessage);

    CommandEnum? commandSelected = null;
    while (commandSelected == null) {
      var key = Console.ReadKey(true);
      commandSelected =
        key.KeyChar == '1' ? CommandEnum.Connect :
        key.KeyChar == '2' ? CommandEnum.Disconnect :
        key.KeyChar == '3' ? CommandEnum.Subscribe :
        key.KeyChar == '4' ? CommandEnum.Unsubscribe :
        key.KeyChar == '5' ? CommandEnum.Publish :
        key.KeyChar == '0' ? CommandEnum.Quit :
        null;
    }
    return commandSelected.Value;

  }
}