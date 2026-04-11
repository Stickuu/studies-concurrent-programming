using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(
        this,
        new PropertyChangedEventArgs(propertyName)
    );
}