﻿using FocusTreeManager.CodeStructures;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MonitoredUndo;
using System;
using System.Windows;

namespace FocusTreeManager.Model
{
    public class ScriptModel : ObservableObject, ISupportsUndo
    {
        private Guid ID;

        public Guid UniqueID
        {
            get
            {
                return ID;
            }
        }

        private string filename;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (value == filename)
                {
                    return;
                }
                DefaultChangeFactory.Current.OnChanging(this,
                         "Filename", filename, value, "Filename Changed");
                filename = value;
                RaisePropertyChanged(() => Filename);
            }
        }

        private Script internalScript;

        public Script InternalScript
        {
            get
            {
                return internalScript;
            }
            set
            {
                if (internalScript == value)
                {
                    return;
                }
                internalScript = value;
                RaisePropertyChanged(() => InternalScript);
            }
        }

        public RelayCommand<FrameworkElement> SaveScriptCommand { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public RelayCommand EditElementCommand { get; private set; }

        public ScriptModel(string Filename)
        {
            filename = Filename;
            this.ID = Guid.NewGuid();
            //Messenger
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            //Commands
            SaveScriptCommand = new RelayCommand<FrameworkElement>(SaveScript);
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            EditElementCommand = new RelayCommand(SendEditSignal);
        }

        private void SaveScript(FrameworkElement obj)
        {
            (new ViewModelLocator()).Scripter.SaveScript();
            InternalScript = (new ViewModelLocator()).Scripter.ManagedScript;
        }

        private void NotificationMessageReceived(NotificationMessage msg)
        {
            if (this.Filename == null)
            {
                return;
            }
            //Always manage container renamed
            if (msg.Notification == "ContainerRenamed")
            {
                RaisePropertyChanged(() => Filename);
            }
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }

        private void SendEditSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this,
                (new ViewModelLocator()).ProjectView, "SendEditItemSignal"));
        }

        #region Undo/Redo

        public object GetUndoRoot()
        {
            return (new ViewModelLocator()).Main;
        }

        #endregion

    }
}
