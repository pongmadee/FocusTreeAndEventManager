﻿using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.Containers
{
    [ProtoContract]
    public class FociGridContainer : ObservableObject
    {
        public Guid IdentifierID { get; private set; }

        [ProtoMember(1)]
        private string containerID;

        public string ContainerID
        {
            get
            {
                return containerID;
            }
            set
            {
                Set<string>(() => this.ContainerID, ref this.containerID, value);
            }
        }

        [ProtoMember(2)]
        public ObservableCollection<Focus> FociList { get; set; }

        public RelayCommand DeleteElementCommand { get; private set; }

        public FociGridContainer()
        {
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        public FociGridContainer(string filename)
        {
            ContainerID = filename;
            FociList = new ObservableCollection<Focus>();
            DeleteElementCommand = new RelayCommand(SendDeleteSignal);
            IdentifierID = Guid.NewGuid();
        }

        private void SendDeleteSignal()
        {
            Messenger.Default.Send(new NotificationMessage(this, (new ViewModelLocator()).ProjectView, "SendDeleteItemSignal"));
        }
    }
}