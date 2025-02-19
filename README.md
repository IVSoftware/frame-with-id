My custom control is a `Border` that has been modified to support a bindable `FrameId` property that the library user will define as an `enum` in the client XAML they're developing. As shown below, the `FrameId` property either "is" or "is not" set.

```xaml
<ContentPage ...>
    <ScrollView>
        <VerticalStackLayout
            Padding="30,0"
            Spacing="25">
            <!--Explicit-->
            <controls:FrameWithId FrameId="{Binding Source={x:Static local:FrameId.BarcodeFrame}}" >                
                <Image Source="dotnet_bot.png" HeightRequest="185"  Aspect="AspectFit" />
            </controls:FrameWithId>
            
            <!--Implicit-->
            <controls:FrameWithId Discovered="OnFrameDiscovered">
                <CollectionView ItemsSource="{Binding Frames}" ItemTemplate="{StaticResource FrameCardTemplate}"/>
            </controls:FrameWithId>
                        
            <!--Explicit-->
            <controls:FrameWithId FrameId="{Binding Source={x:Static local:FrameId.QRCodeFrame}}">
                <Button Text="Click Me" />
            </controls:FrameWithId>
        </VerticalStackLayout>
    </ScrollView>  
</ContentPage> 
```

The ID is used to bootstrap the app by running a discovery flow in the background. This enumeration might take a second or two, but UI doen't need to wait. The core challenge is reliably detecting whether a property assignment _did not get set_ in the XAML so that the one-time discovery can be run in a default configuration.

___

**My question:** Is it _safe to assume_ that the constructor and object initializer guaranteed to execute sequentially _without interruption_? 
___

My preferred option is super-simple, it seems to work great, and I believe this represents a worst-case by setting the delay to one tick. I've got at least two other ways I could detect this, but they're less optimal.

```
enum Reserved { DefaultId, }

interface IDiscoveryMonitor { event EventHandler Discovered; }

class FrameWithId : Border, IDiscoveryMonitor
{
    public FrameWithId()
    {
        Stroke = SolidColorBrush.Transparent;
        var idB4 = FrameId;
        Task
            .Delay(TimeSpan.FromTicks(1))
            .GetAwaiter()
            .OnCompleted(() =>
            {
                // This ASSUMES that CTOR and Object Initializer cannot
                // be separated in time under any circumstances. But it
                // is DEFINITELY APPEARS TO BE RELIABLE HERE!
                var idFtr = FrameId;
                Debug.WriteLine($"ID Before {idB4} => ID After {idFtr}");
                if(Equals(idFtr, Reserved.DefaultId))
                {
                    // The ID has 'not' been set so run discovery for default.
                    _ = MockRunDiscovery();
                }
            });
    }

    public static readonly BindableProperty FrameIdProperty =
        BindableProperty.Create(
            propertyName: nameof(FrameWithId.FrameId),
            returnType: typeof(Enum),
            declaringType: typeof(FrameWithId),
            defaultValue: Reserved.DefaultId,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is FrameWithId @this)
                {
                    _ = @this.MockRunDiscovery();
                }
            });
    .
    .
    .
```

__

