using System.Windows.Input;

namespace Otto.Theme.Interactivity
{

    public static class ControlCommands
    {
        public static RoutedCommand Search { get; } = new RoutedCommand(nameof(Search), typeof(ControlCommands));

        public static RoutedCommand Clear { get; } = new RoutedCommand(nameof(Clear), typeof(ControlCommands));

        public static RoutedCommand Switch { get; } = new RoutedCommand(nameof(Switch), typeof(ControlCommands));

        public static RoutedCommand RotateRight { get; } = new RoutedCommand(nameof(RotateRight), typeof(ControlCommands));

        public static RoutedCommand RotateLeft { get; } = new RoutedCommand(nameof(RotateLeft), typeof(ControlCommands));

        public static RoutedCommand Reduce { get; } = new RoutedCommand(nameof(Reduce), typeof(ControlCommands));

        public static RoutedCommand Enlarge { get; } = new RoutedCommand(nameof(Enlarge), typeof(ControlCommands));

        public static RoutedCommand Restore { get; } = new RoutedCommand(nameof(Restore), typeof(ControlCommands));

        public static RoutedCommand Open { get; } = new RoutedCommand(nameof(Open), typeof(ControlCommands));

        public static RoutedCommand Save { get; } = new RoutedCommand(nameof(Save), typeof(ControlCommands));

        public static RoutedCommand Selected { get; } = new RoutedCommand(nameof(Selected), typeof(ControlCommands));

        public static RoutedCommand Close { get; } = new RoutedCommand(nameof(Close), typeof(ControlCommands));

        public static RoutedCommand Cancel { get; } = new RoutedCommand(nameof(Cancel), typeof(ControlCommands));

        public static RoutedCommand Confirm { get; } = new RoutedCommand(nameof(Confirm), typeof(ControlCommands));

        public static RoutedCommand Yes { get; } = new RoutedCommand(nameof(Yes), typeof(ControlCommands));

        public static RoutedCommand No { get; } = new RoutedCommand(nameof(No), typeof(ControlCommands));

        public static RoutedCommand CloseAll { get; } = new RoutedCommand(nameof(CloseAll), typeof(ControlCommands));

        public static RoutedCommand CloseOther { get; } = new RoutedCommand(nameof(CloseOther), typeof(ControlCommands));

        public static RoutedCommand Prev { get; } = new RoutedCommand(nameof(Prev), typeof(ControlCommands));

        public static RoutedCommand Next { get; } = new RoutedCommand(nameof(Next), typeof(ControlCommands));

        public static RoutedCommand Jump { get; } = new RoutedCommand(nameof(Jump), typeof(ControlCommands));

        public static RoutedCommand Am { get; } = new RoutedCommand(nameof(Am), typeof(ControlCommands));

        public static RoutedCommand Pm { get; } = new RoutedCommand(nameof(Pm), typeof(ControlCommands));

        public static RoutedCommand Sure { get; } = new RoutedCommand(nameof(Sure), typeof(ControlCommands));

        public static RoutedCommand HourChange { get; } = new RoutedCommand(nameof(HourChange), typeof(ControlCommands));

        public static RoutedCommand MinuteChange { get; } = new RoutedCommand(nameof(MinuteChange), typeof(ControlCommands));

        public static RoutedCommand SecondChange { get; } = new RoutedCommand(nameof(SecondChange), typeof(ControlCommands));

        public static RoutedCommand MouseMove { get; } = new RoutedCommand(nameof(MouseMove), typeof(ControlCommands));

        public static OpenLinkCommand OpenLink { get; } = new OpenLinkCommand();

        public static ShutdownAppCommand ShutdownApp { get; } = new ShutdownAppCommand();

        public static PushMainWindow2TopCommand PushMainWindow2Top { get; } = new PushMainWindow2TopCommand();

        public static CloseWindowCommand CloseWindow { get; } = new CloseWindowCommand();

        public static RoutedCommand SortByCategory { get; } = new RoutedCommand(nameof(SortByCategory), typeof(ControlCommands));

        public static RoutedCommand SortByName { get; } = new RoutedCommand(nameof(SortByName), typeof(ControlCommands));
    }
}
