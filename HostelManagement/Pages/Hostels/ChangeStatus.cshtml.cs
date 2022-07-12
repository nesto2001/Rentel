﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BusinessObject.BusinessObject;
using DataAccess;
using DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;

namespace HostelManagement.Pages.Hostels
{
    [Authorize(Roles = "Owner,Admin")]
    public class ChangeStatusModel : PageModel
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

        public ChangeStatusModel(IHostelRepository _hostelRepository, IAccountRepository _accountRepository,
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
        public Hostel Hostel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await hostelRepository.GetHostelByID((int)id);
            if (Hostel == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostPendingAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await hostelRepository.GetHostelByID((int)id);

            if (Hostel != null)
            {
                Hostel.Status = 0;
                await hostelRepository.UpdateHostel(Hostel);
                
            }
            return RedirectToPage("./Details", new { id = Hostel.HostelId });
        }

        public async Task<IActionResult> OnPostActiveAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await hostelRepository.GetHostelByID((int)id);

            if (Hostel != null)
            {
                Hostel.Status = 1;
                await hostelRepository.UpdateHostel(Hostel);

            }
            return RedirectToPage("./Details", new { id = Hostel.HostelId });
        }

        public async Task<IActionResult> OnPostInactiveAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await hostelRepository.GetHostelByID((int)id);

            if (Hostel != null)
            {
                Hostel.Status = 2;
                await hostelRepository.UpdateHostel(Hostel);

            }
            return RedirectToPage("./Details", new { id = Hostel.HostelId });
        }

        public async Task<IActionResult> OnPostDeniedAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Hostel = await hostelRepository.GetHostelByID((int)id);

            if (Hostel != null)
            {
                Hostel.Status = 3;
                await hostelRepository.UpdateHostel(Hostel);

            }
            return RedirectToPage("./Details", new { id = Hostel.HostelId });
        }
    }
}
