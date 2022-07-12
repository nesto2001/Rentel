using DataAccess.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HostelManagement.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        private readonly IHostelRepository hostelRepository;
        private readonly IAccountRepository accountRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProvinceRepository provinceRepository;
        private readonly IDistrictRepository districtRepository;
        private readonly IWardRepository wardRepository;
        private readonly ILocationRepository locationRepository;
        private readonly IHostelPicRepository hostelPicRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomMemberRepository roomMemberRepository;
        private readonly IRentRepository rentRepository;
        private readonly IBillRepository billRepository;
        private readonly IBillDetailRepository billDetailRepository;

        public AdminDashboardModel(IHostelRepository _hostelRepository, IAccountRepository _accountRepository,
            ICategoryRepository _categoryRepository, IProvinceRepository _provinceRepository,
            IDistrictRepository _districtRepository, IWardRepository _wardRepository,
            ILocationRepository _locationRepository, IHostelPicRepository _hostelPicRepository,
            IRoomRepository _roomRepository, IRoomMemberRepository _roomMemberRepository, IRentRepository _rentRepository,
            IBillRepository _billRepository, IBillDetailRepository _billDetailRepository)
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
            roomMemberRepository = _roomMemberRepository;
            rentRepository = _rentRepository;
            billRepository = _billRepository;
            billDetailRepository = _billDetailRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string Message = HttpContext.Session.GetString("AdminDashboardMessage");
            if (!String.IsNullOrEmpty(Message))
            {
                ViewData["HostelOwnerDashboardMessage"] = Message;
                HttpContext.Session.Remove("HostelOwnerDashboardMessage");
            }
            //hostel count, room count, rented room count
            var hostels = await hostelRepository.GetHostelsList();
            ViewData["hostelCount"] = hostels.Count();
            var rooms = await roomRepository.GetRoomList();
            ViewData["roomCount"] = rooms.Count();
            ViewData["rentedRoomCount"] = rooms.Where(r => r.Status == 4).Count();
            //renting count, running rent, completed rent, pending start
            var rents = await rentRepository.GetRentList();
            ViewData["rentingCount"] = rents.Count();
            ViewData["runningRent"] = rents.Count(r => r.Status == 1 || r.Status == 2 || r.Status == 5);
            ViewData["completedRent"] = rents.Count(r => r.Status == 3 || r.Status == 6);
            ViewData["pendingStartRent"] = rents.Count(r => r.Status == 0);
            //renter count, roomMember count
            List<string> renters = new List<string>();
            foreach (var item in rents)
            {
                if (renters == null) renters.Add(item.RentedBy);
                if (!renters.Contains(item.RentedBy)) renters.Add(item.RentedBy);
            }
            ViewData["renterCount"] = renters.Count();
            ViewData["roomMemberCount"] = rents.Sum(r => r.RoomMembers.Count());
            //revenues month, revenues year (bill)
            var billDetails = await billDetailRepository.GetBillDetailList();
            var billDetailsYear = billDetails.Where(b => b.Bill.CreatedDate.Value.Year == DateTime.Now.Year);
            var billDetailsMonth = billDetailsYear.Where(b => b.Bill.CreatedDate.Value.Month == DateTime.Now.Month);
            ViewData["revenuesYear"] = billDetailsYear.Sum(b => b.Fee);
            ViewData["revenuesMonth"] = billDetailsMonth.Sum(b => b.Fee);
            return Page();
        }
    }
}
