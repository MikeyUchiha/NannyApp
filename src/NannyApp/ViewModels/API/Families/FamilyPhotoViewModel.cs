using NannyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.API.Families
{
    public class FamilyPhotoViewModel
    {
        private FamilyPhoto _familyphoto;

        public string Url { get; internal set; }
        public int FamilyId { get; internal set; }

        public FamilyPhotoViewModel(FamilyPhoto familyphoto)
        {
            _familyphoto = familyphoto;
            GetUrlFromProfilePhoto();
        }

        private void GetUrlFromProfilePhoto()
        {
            var familyPhoto = _familyphoto;
            if (familyPhoto != null)
            {
                Url = familyPhoto.FileUrl;
            }
        }
    }
}
