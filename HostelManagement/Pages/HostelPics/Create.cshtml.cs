﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject.BusinessObject;
using DataAccess;

namespace HostelManagement.Pages.HostelPics
{
    public class CreateModel : PageModel
    {
        private readonly DataAccess.HostelManagementContext _context;

        public CreateModel(DataAccess.HostelManagementContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["HostelId"] = new SelectList(_context.Hostels, "HostelId", "HostelOwnerEmail");
            return Page();
        }

        [BindProperty]
        public HostelPic HostelPic { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.HostelPics.Add(HostelPic);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}