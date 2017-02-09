﻿using FocusTreeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.DataContract
{
    [KnownType(typeof(Focus))]
    [DataContract(Name = "foci_container", Namespace = "focusesNs")]
    public class FociGridContainer
    {
        [DataMember(Name = "guid_id", Order = 0)]
        public Guid IdentifierID { get; private set; }

        [DataMember(Name = "text_id", Order = 1)]
        public string ContainerID { get; set; }

        [DataMember(Name = "tag", Order = 2)]
        public string TAG { get; set; }

        [DataMember(Name = "foci", Order = 3)]
        public List<Focus> FociList { get; set; }

        [DataMember(Name = "mods", Order = 4)]
        public string AdditionnalMods { get; set; }

        public FociGridContainer()
        {
            IdentifierID = Guid.NewGuid();
            FociList = new List<Focus>();
        }

        public FociGridContainer(Containers.LegacySerialization.FociGridContainer legacyItem)
        {
            IdentifierID = legacyItem.IdentifierID;
            ContainerID = legacyItem.ContainerID;
            TAG = legacyItem.TAG;
            FociList = Focus.PopulateFromLegacy(legacyItem.FociList.ToList());
        }

        public FociGridContainer(string filename)
        {
            ContainerID = filename;
            FociList = new List<Focus>();
            IdentifierID = Guid.NewGuid();
        }

        public FociGridContainer(FocusGridModel item)
        {
            IdentifierID = item.UniqueID;
            ContainerID = item.Filename;
            TAG = item.TAG;
            AdditionnalMods = item.AdditionnalMods;
            FociList = new List<Focus>();
            foreach (FocusModel model in item.FociList)
            {
                FociList.Add(new Focus()
                {
                    UniqueName = model.UniqueName,
                    Image = model.Icon,
                    X = model.X,
                    Y = model.Y,
                    Cost = model.Cost,
                    InternalScript = model.InternalScript
                });
            }
            //Repair sets
            foreach (Focus focus in FociList)
            {
                FocusModel associatedModel = 
                    item.FociList.FirstOrDefault(f => f.UniqueName == focus.UniqueName);
                foreach (PrerequisitesSetModel set in associatedModel.Prerequisite)
                {
                    PrerequisitesSet newset = new PrerequisitesSet(focus);
                    foreach (FocusModel model in set.FociList)
                    {
                        newset.FociList.Add(FociList.FirstOrDefault(f => f.UniqueName == model.UniqueName));
                    }
                    focus.Prerequisite.Add(newset);
                }
                foreach (MutuallyExclusiveSetModel set in associatedModel.MutualyExclusive)
                {
                    MutuallyExclusiveSet newset = new MutuallyExclusiveSet(
                        FociList.FirstOrDefault(f => f.UniqueName == set.Focus1.UniqueName),
                        FociList.FirstOrDefault(f => f.UniqueName == set.Focus2.UniqueName));
                    focus.MutualyExclusive.Add(newset);
                }
            }
        }

        internal static List<FociGridContainer> PopulateFromLegacy
            (List<Containers.LegacySerialization.FociGridContainer> fociContainerList)
        {
            List<FociGridContainer> list = new List<FociGridContainer>();
            foreach (Containers.LegacySerialization.FociGridContainer legacyItem in fociContainerList)
            {
                list.Add(new FociGridContainer(legacyItem));
            }
            return list;
        }
    }
}
