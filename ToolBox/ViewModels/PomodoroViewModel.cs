using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ToolBox.ViewModels;

public enum PomodoroState
{
    Stopped,
    Paused,
    Session,
    Break
}

public partial class PomodoroViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimeText))]
    private TimeSpan _intervallExtra;
    private TimeSpan IntervalLength => (_lastState == PomodoroState.Break ? TimeSpan.FromMinutes(5) : TimeSpan.FromMinutes(25)) + IntervallExtra;

    private PomodoroState _state = PomodoroState.Stopped;
    private PomodoroState State
    {
        get => _state;
        set
        {
            if (!EqualityComparer<PomodoroState>.Default.Equals(_state, value))
            {
                _state = value;
                _timer.Enabled = _state is PomodoroState.Break or PomodoroState.Session;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(ToggleTimerText));
            }
        }
    }

    private PomodoroState _lastState = PomodoroState.Session;

    public string ToggleTimerText => State switch
    {
        PomodoroState.Stopped => "Start",
        PomodoroState.Paused => "Continue",
        _ => "Pause"
    };

    public string StatusText => State switch
    {
        PomodoroState.Stopped => "Stopped",
        PomodoroState.Session => "Worky Worky",
        PomodoroState.Break => "Bing Chilling",
        PomodoroState.Paused => "Paused"
    };
    
    private readonly Timer _timer = new();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArcAngle))]
    [NotifyPropertyChangedFor(nameof(TimeText))]
    private TimeSpan _timeRemaning = TimeSpan.FromMinutes(25);
    
    public float ArcAngle => (float)TimeRemaning.TotalSeconds / (float)IntervalLength.TotalSeconds * 240;
    public string TimeText => TimeRemaning.ToString();
    
    public PomodoroViewModel()
    {
        _timer.Interval = 1000;
        _timer.Elapsed += OnTimerTick;
    }

    private void OnTimerTick(object? sender, ElapsedEventArgs e)
    {
        TimeRemaning -= TimeSpan.FromSeconds(1);

        if (TimeRemaning.TotalSeconds <= 0)
        {
            State = (State == PomodoroState.Session ? PomodoroState.Break : PomodoroState.Session);
            _lastState = State;
            IntervallExtra = TimeSpan.Zero;
            TimeRemaning = IntervalLength;
        }
    }

    [RelayCommand]
    private void ToggleTimer()
    {
        State = State switch
        {
            PomodoroState.Stopped => PomodoroState.Session,
            PomodoroState.Session => PomodoroState.Paused,
            PomodoroState.Break => PomodoroState.Paused,
            PomodoroState.Paused => _lastState
        };
    }

    [RelayCommand]
    private void CancelTimer()
    {
        State = PomodoroState.Stopped;
        _lastState = PomodoroState.Session;
        IntervallExtra = TimeSpan.Zero;
        TimeRemaning = IntervalLength;
    }

    [RelayCommand]
    private void PlusFiveTimer()
    {
        IntervallExtra += TimeSpan.FromMinutes(5);
        TimeRemaning += TimeSpan.FromMinutes(5);
    }
 }