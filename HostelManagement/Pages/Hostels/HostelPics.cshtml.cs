using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject.BusinessObject;
using DataAccess.Repository;
using HostelManagement.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HostelManagement.Pages.Hostels
{
    public class HostelPicsModel : PageModel
    {
        private IHostelPicRepository hostelPicRepository;

        public HostelPicsModel(IHostelPicRepository _hostelPicRepository)
        {
            hostelPicRepository = _hostelPicRepository;
        }
        public IEnumerable<HostelPic> HostelPics { get; set; }
        public int HostelID { get; set; }
        [Required(ErrorMessage = "Please chose at least one file.")]
        [DataType(DataType.Upload)]
        //[FileExtensions(Extensions = "png,jpg,jpeg,gif")]
        [Display(Name = "Choose Hostel images(s)")]
        [BindProperty]
        public IFormFile[] FileUploads { get; set; }
        public async Task OnGetAsync(int id)
        {
            HttpContext.Session.SetInt32("HostelID", id);
            HostelID = (int)HttpContext.Session.GetInt32("HostelID");
            HostelPics = await hostelPicRepository.GetHostelPicsOfAHostel(id);
        }

        public async Task<IActionResult> OnGetDelete(int id)
        {
            HostelID = (int)HttpContext.Session.GetInt32("HostelID");
            HostelPics = await hostelPicRepository.GetHostelPicsOfAHostel(HostelID);
            HostelPic hostelPic = HostelPics.Where(h => h.HostelPicsId == id).FirstOrDefault();
            await hostelPicRepository.DeleteHostelPic(hostelPic);
            return RedirectToPage("HostelPics", new {id = HostelPics.First().HostelId});
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                
                return Page();
            }
            HostelPic hosPic = new HostelPic();
            if (FileUploads != null)
            {
                HostelID = (int)HttpContext.Session.GetInt32("HostelID");
                hosPic.HostelId = HostelID;
                foreach (var FileUpload in FileUploads)
                {
                    hosPic.HostelPicUrl = await Utilities.UploadFile(FileUpload, @"images\", FileUpload.FileName);
                    await hostelPicRepository.AddHostelPic(hosPic);
                }
            }
            return RedirectToPage("./Details", new { id = HostelID });
        }

    }
}
