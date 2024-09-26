namespace TwitcheryNet.Events;

public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);

public delegate Task AsyncEventHandler(object sender, object e);
