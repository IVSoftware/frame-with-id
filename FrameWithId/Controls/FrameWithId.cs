using System;
using System.Diagnostics;

namespace FrameWithIdDemo.Controls
{
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

        public Enum FrameId
        {
            get => (Enum)GetValue(FrameIdProperty);
            set => SetValue(FrameIdProperty, value);
        }

        /// <summary>
        /// Discovery will "take some time" and should not 
        /// happen on the UI thread. This simulates that.
        /// </summary>
        private async Task MockRunDiscovery()
        {
            var seconds = 1 + (5 * _rando.NextDouble());
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            Discovered?.Invoke(this, EventArgs.Empty);
        }

        event EventHandler IDiscoveryMonitor.Discovered
        {
            add => Discovered += value;
            remove => Discovered -= value;
        }
        public static event EventHandler? Discovered;
        static Random _rando = new Random(1);
    }
}
