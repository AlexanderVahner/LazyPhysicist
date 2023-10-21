using LazyPhysicist.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace LazyContouring.Models
{
    public sealed class StructureVariable : Notifier
    {
        public static List<string> DicomTypesAvailableForCreate = new List<string> { "CONTROL", "PTV", "CTV", "GTV", "ORGAN", "FIXATION" };

        private Structure structure;
        private string structureId = "";
        private string dicomType = "CONTROL";
        private Color color = Colors.Magenta;
        private bool isTemporary = false;
        private bool isVisible = true;
        private bool isNew;
        private bool isSelected = false;

        private void SetStructure(Structure structure)
        {
            this.structure = structure;
            structureId = structure.Id;
            dicomType = structure.DicomType;
            color = structure.Color;
            NotifyPropertyChanged(nameof(Structure));
            NotifyPropertyChanged(nameof(StructureId));
            NotifyPropertyChanged(nameof(DicomType));
            NotifyPropertyChanged(nameof(Color));
            NotifyPropertyChanged(nameof(SegmentVolume));
        }

        [XmlIgnore]
        public Structure Structure { get => structure; set => SetStructure(value); }
        public string StructureId
        {
            get => structureId;
            set
            {
                if (Structure != null)
                {
                    try
                    {
                        Structure.Id = value;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    finally
                    {
                        structureId = Structure.Id;
                    }
                }
                else
                {
                    structureId = value;
                }
                NotifyPropertyChanged(nameof(StructureId));

            }
        }
        public string DicomType
        {
            get => dicomType;
            set
            {
                if (Structure == null)
                {
                    dicomType = value;
                }
                NotifyPropertyChanged(nameof(DicomType));
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                if (Structure == null)
                {
                    color = value;
                }
                NotifyPropertyChanged(nameof(Color));
            }
        }

        public SegmentVolume GetSegmentVolume() => Structure?.SegmentVolume;
        public void SetSegmentVolume(SegmentVolume value)
        {
            if (Structure != null)
            {
                try
                {
                    Structure.SegmentVolume = value;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    NotifyPropertyChanged("SegmentVolume");
                    NotifyPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public VVector[][] GetContoursOnImagePlane(int z) => structure?.GetContoursOnImagePlane(z);

        public bool CanEditSegmentVolume => Structure?.CanEditSegmentVolume(out _) ?? false;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }
        public bool IsTemporary { get => isTemporary; set => SetProperty(ref isTemporary, value); }
        public bool IsEmpty => Structure?.IsEmpty ?? true;
        public bool IsVisible { get => isVisible; set => SetProperty(ref isVisible, value); }
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }
    }
}
