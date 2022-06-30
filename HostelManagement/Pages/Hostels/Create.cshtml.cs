﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject.BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HostelManagement.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace HostelManagement.Pages.Hostels
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

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            int UId = Int32.Parse(userId);
            var account = await accountRepository.GetAccountByID(UId);
            UserEmail = account.UserEmail;
            HttpContext.Session.SetString("UserEmail", UserEmail);
            //UserEmail = HttpContext.Session.GetString("UserEmail");
             //Hostel.HostelOwnerEmail = account.UserEmail;
            var locas = await locationRepository.GetLocationsList();
            locID = locas.OrderByDescending(l => l.LocationId).FirstOrDefault().LocationId + 1;
            HttpContext.Session.SetInt32("locID", locID);
            
            ViewData["CategoryId"] = new SelectList(await categoryRepository.GetCategoriesList(), "CategoryId", "CategoryName");
            //ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "AddressString");
            ViewData["ProvinceId"] = new SelectList(await provinceRepository.GetProvincesList(), "ProvinceId", "ProvinceName");
            return Page();
        }

        [BindProperty]
        public Hostel Hostel { get; set; }
        [BindProperty]
        public Location Location { get; set; }
        [BindProperty]
        public HostelPic HostelPic { get; set; }
        [BindProperty]
        public Room[] Rooms { get; set; }
        public int locID { get; set; }
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Please chose at least one file.")]
        [DataType(DataType.Upload)]
        //[FileExtensions(Extensions = "png,jpg,jpeg,gif")]
        [Display(Name = "Choose Hostel images(s)")]
        [BindProperty]
        public IFormFile[] FileUploads { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(await categoryRepository.GetCategoriesList(), "CategoryId", "CategoryName");
                ViewData["ProvinceId"] = new SelectList(await provinceRepository.GetProvincesList(), "ProvinceId", "ProvinceName");
                //UserEmail = HttpContext.Session.GetString("UserEmail");
                return Page();
            }
            locID = (int)HttpContext.Session.GetInt32("locID");
            Hostel.LocationId = locID;
            Location.LocationId = locID;
            UserEmail = HttpContext.Session.GetString("UserEmail");
            Hostel.HostelOwnerEmail = UserEmail;
            await locationRepository.AddLocation(Location);
            await hostelRepository.AddHostel(Hostel);
            if (FileUploads != null)
            {
                HostelPic.HostelId = Hostel.HostelId;
                HostelPic.Hostel = Hostel;
                foreach (var FileUpload in FileUploads)
                {
                    HostelPic.HostelPicUrl = await Utilities.UploadFile(FileUpload, @"images\", FileUpload.FileName);
                    await hostelPicRepository.AddHostelPic(HostelPic);
                }
            }
            foreach (var Room in Rooms)
            {
                Room.HostelId = Hostel.HostelId;
                await roomRepository.AddRoom(Room);
            }
            return RedirectToPage("./Details", new {id= Hostel.HostelId });
        }

        public async Task<JsonResult> OnGetLoadDistrict(int ProvinceId)
        {
            IEnumerable<District> districts = await districtRepository.GetDistrictListByProvinceId(ProvinceId);
            return new JsonResult(districts);
        }

        public async Task<JsonResult> OnGetLoadWard(int DistrictId)
        {
            IEnumerable<Ward> wards = await wardRepository.GetWardListByDistrictId(DistrictId);
            return new JsonResult(wards);
        }

    }
}
