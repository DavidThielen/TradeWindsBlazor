<img align="left" width="200" src="server_client.png"/>

# TradeWindsBlazor

These are some classes I created for my Blazor application.

I will add to this when I identify additional components in my apps that might be of general use.

> This is under the MIT license. If you find this useful I ask (not a requirement) that you consider reading my book [I DON’T KNOW WHAT I’M DOING!: How a Programmer Became a Successful Startup CEO](https://a.co/d/bEpDlJR).
> 
> And if you like it, please review it on Amazon and/or GoodReads. The number of legitimate reviews helps a lot. Much appreciated.

## ExComponentBase

ExComponentBase.cs is included as an example, not to be used directly. That's why it is `internal` instead of `public`. I do recommend you copy this over to your application, extend it, and use it instead of `ComponentBase` for your components.

To use the ScoppedLoggerEx, you must have the following in your component:

```csharp
[Inject]
private ScopedLoggerFactoryEx ScopedLoggerFactoryEx { get; set; } = default!;

protected ScopedLoggerEx LoggerEx { get; set; } = default!;

protected override async Task OnInitializedAsync()
{
   await base.OnInitializedAsync();

   LoggerEx = await ScopedLoggerFactoryEx.GetLogger(GetType());
}
```

## ExPageBase

I also recommend you create the class ExPageBase, that is a subclass of ExComponentBase. Use this as your base class for any pages. In this class, add the following:

```csharp
protected override async Task OnInitializedAsync()
{

   await base.OnInitializedAsync();

   if (LoggerEx.IsEnabled(LogLevel.Trace))
      LoggerEx.LogTrace($"Entering {GetType().Name}.OnInitializedAsync(), IsPreRender={IsPreRender}");
}
```

## ILogger

After you have added the above to your `ComponentBase`, you now have the member variable `LoggerEx` that is an `ILogger` for your component. And it is scoped to the component adding username and aspNetId as scopes for all logging by the logger.

## Inject everything

I strongly recommend that you add an `[Inject]` for every service you use in any component to your `ExComponentBase`. And the same for every `[CascadingParameter]`. Saves you a lot of copy/paste to each new component.