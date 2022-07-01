﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject.BusinessObject;
using DataAccess;
using DataAccess.Repository;
using HostelManagement.Helpers;

namespace HostelManagement.Pages.Rooms
{
    public class CreateModel : PageModel
    {
        private IHostelRepository hostelRepository;
        private IAccountRepository accountRepository;
        private ICategoryRepository categoryRepository;
        private IProvinceRepository provinceRepository;
        private IDistrictRepository districtRepository;
        private IWardRepository wardRepository;
        private ILocationRepository locationRepository;
        private IHostelPicRepository hostelPicRepository;
        private IRoomRepository roomRepository;

        public CreateModel(IHostelRepository _hostelRepository, IAccountRepository _accountRepository,
            ICategoryRepository _categoryRepository, IProvinceRepository _provinceRepository,
            IDistrictRepository _districtRepository, IWardRepository _wardRepository,
            ILocationRepository _locationRepository, IHostelPicRepository _hostelPicRepository, IRoomRepository _roomRepository)
        {
            hostelRepository = _hostelRepository;
            accountRepository = _accountRepository;
            categoryRepository = _categoryRepository;
            provinceRepository = _provinceRepository;
            districtRepository = _districtRepository;
            wardRepository = _wardRepository;
            locationRepository = _locationRepository;
            hostelPicRepository = _hostelPicRepository;
            roomRepository = _roomRepository;
        }


        [BindProperty]
        public Room[] Rooms { get; set; }
        public int countPic { get; set; }
        public int countRoom { get; set; }

        public async Task<IActionResult> OnGetAsync(int countPics, int countRooms)
        {
            countPic = countPics;
            countRoom = countRooms;
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Location location = SessionHelper.GetObjectFromJson<Location>(HttpContext.Session, "locationPending");
            if (location != null)
            {
                await locationRepository.AddLocation(location);
            }
            Hostel hostel = SessionHelper.GetObjectFromJson<Hostel>(HttpContext.Session, "hostelPending");
            if (hostel != null)
            {
                await hostelRepository.AddHostel(hostel);
            }
            for (int i = 1; i <= countPic; i++)
            {
                string key = $"hostelPicPending{i}";
                HostelPic hostelPic = SessionHelper.GetObjectFromJson<HostelPic>(HttpContext.Session,key);
                if (hostelPic != null)
                {
                    await hostelPicRepository.AddHostelPic(hostelPic);
                }
            }
            foreach (var Room in Rooms)
            {
                Room.HostelId = hostel.HostelId;
                await roomRepository.AddRoom(Room);
            }
            return RedirectToPage("../Hostels/Details", new {id=hostel.HostelId});
        }
    }
}
