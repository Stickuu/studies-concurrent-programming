//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace TP.ConcurrentProgramming.PresentationView
{
  public partial class App : Application
  {
    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        desktop.MainWindow = new MainWindow();
      }
      base.OnFrameworkInitializationCompleted();
    }

    public static void Main(string[] args)
    {
      BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
      return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace();
    }
  }
}
