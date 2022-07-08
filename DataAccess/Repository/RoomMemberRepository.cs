﻿using BusinessObject.BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RoomMemberRepository : IRoomMemberRepository
    {
        public async Task AddRoomMember(RoomMember RoomMember) => await RoomMemberDAO.Instance.AddRoomMember(RoomMember);
        public async Task<IEnumerable<RoomMember>> GetRoomMemberList() => await RoomMemberDAO.Instance.GetRoomMemberList();
        public async Task<RoomMember> GetRoomMemberByID(int id) => await RoomMemberDAO.Instance.GetRoomMemberByID(id);
        public async Task UpdateRoomMember(RoomMember RoomMember) => await RoomMemberDAO.Instance.UpdateRoomMember(RoomMember);
    }
}
