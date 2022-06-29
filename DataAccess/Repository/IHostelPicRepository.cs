﻿using BusinessObject.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IHostelPicRepository
    {
        Task AddHostelPic(HostelPic hostelPic);
        Task<IEnumerable<HostelPic>> GetHostelPicsOfAHostel(int hostelId);
        Task DeleteHostelPic(HostelPic hostelPic);
    }
}
