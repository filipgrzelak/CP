﻿using Logic;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;

namespace ViewModel
{
    public class PoolViewModel : ViewModelBase
    {
        public PoolViewModel()
        {
            viewModelCircles = new();
            WindowHeight = 640;
            WindowWidth = 1230;
            PoolModel = new PoolModel(WindowWidth, WindowHeight);
            StartCommand = new CommandBase(Start);
            StopCommand = new CommandBase(Stop);
        }
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        private async void Start()
        {
            foreach(AbstractLogicCircle logicCircle in PoolModel.GetStartingCirclePositions(Count))
            {
                ModelCircle circle = new ModelCircle(logicCircle.Postion.X, logicCircle.Postion.Y, logicCircle.GetRadius());
                viewModelCircles.Add(circle);
                logicCircle.PropertyChanged += circle.Update!;
            }
            PoolModel.StartThreads();
            while (PoolModel.Animating)
            {
                await Task.Delay(10);
                Circles = new ObservableCollection<ModelCircle>(viewModelCircles);
            }
        }

        private void Stop()
        {
            PoolModel.Animating = false;
            PoolModel.InterruptThreads();
            viewModelCircles.Clear();
        }

        private ObservableCollection<ModelCircle> viewModelCircles;
        public ObservableCollection<ModelCircle> Circles
        {
            get => viewModelCircles;
            set
            {
                viewModelCircles = value;
                OnPropertyChanged(nameof(Circles));
            }
        }
        public int WindowHeight { get; }
        public int WindowWidth { get; }

        public PoolModel PoolModel { get; set; }
    }
}
