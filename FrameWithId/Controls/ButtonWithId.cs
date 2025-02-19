using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameWithIdDemo.Controls
{
    class ButtonWithId : Button
    {
        public ButtonWithId()
        {
            Task
                .Delay(TimeSpan.FromTicks(1))
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    _ = MockInitButton();
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
                    if (bindable is ButtonWithId @this)
                    {
                        _ = @this.MockInitButton();
                    }
                });

        public Enum FrameId
        {
            get => (Enum)GetValue(FrameIdProperty);
            set => SetValue(FrameIdProperty, value);
        }

        /// <summary>
        /// Should not be performed on the UI thread. 
        /// </summary>
        async Task MockInitButton()
            => await Task.Delay(TimeSpan.FromSeconds(2.5));
    }
}
