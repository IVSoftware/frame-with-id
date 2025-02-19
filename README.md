My custom control is a `Border` that has been modified to support a bindable `FrameId` property that the library user will define as an `enum` in the client XAML they're developing. As shown below, the `FrameId` property either "is" or "is not" set.

```xaml
<ContentPage ...>
    <ScrollView>
        <VerticalStackLayout
            Padding="30,0"
            Spacing="25">

            <!--Explicit-->
            <controls:FrameWithId 
                FrameId="{Binding Source={x:Static local:FrameId.BarcodeFrame}}" >                
                <Image 
                    Source="dotnet_bot.png" 
                    HeightRequest="185"  
                    Aspect="AspectFit" />
            </controls:FrameWithId>
            
            <!--Implicit-->
            <controls:FrameWithId >
                <CollectionView
                    ItemsSource="{Binding Frames}" 
                    ItemTemplate="{StaticResource FrameCardTemplate}"/>
            </controls:FrameWithId>
                        
            <!--Explicit-->
            <controls:FrameWithId 
                FrameId="{Binding Source={x:Static local:FrameId.QRCodeFrame}}">
                <Button Text="Click Me" />
            </controls:FrameWithId>

        </VerticalStackLayout>
    </ScrollView>  
</ContentPage> 
```

The ID is used to bootstrap the app by running a discovery flow in the background. This enumeration might take a second or two, but the UI strives to keep startup responsive by not awaiting this. The core challenge is reliably detecting whether a property assignment _did not get set_ in the XAML so that the one-time discovery can be run in a default configuration.

___

**My Question**

Is it **_safe to assume_** that the constructor and object initializer are **_guaranteed_** to execute sequentially **_without interruption_** on the UI thread upon which they are being invoked? 
___

Because _if the answer is "yes"_ then my preferred option is super-simple, it seems to work great, and I believe this represents a worst-case by setting the delay to one tick. I've got at least two other ways I could detect this, but they're less optimal. Of course, if _you_ can think of any alternative that achieves the goal in a more robust manner, I'm listening.

##### Framework Control Self-Discovery based on FrameId

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
                // - This ASSUMES that CTOR and Object Initializer cannot
                //   be separated in time under any circumstances. 
                // - This assumption APPEARS TO BE RELIABLE HERE.
                // - But IS IT REALLY ???
                var idFtr = FrameId;
                Debug.WriteLine($"ID Before {idB4} => ID After {idFtr}");
                if(Equals(idFtr, Reserved.DefaultId))
                {
                    // The ID has 'not' been set so run discovery for default.
                    _ = MockRunDiscovery();
                }
            });
    }
    .
    .
    .
```
[![debug output][1]][1]

*The output from the `Debug.WriteLine(...)` statements*
___

##### Bindable `FrameId` property

```
    .
    .
    .
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
**MRE**

A minimal project can be found in this [REPO](https://github.com/IVSoftware/frame-with-id.git). It might make things more clear if one was to run it.


  [1]: https://i.sstatic.net/gwl02R9I.png