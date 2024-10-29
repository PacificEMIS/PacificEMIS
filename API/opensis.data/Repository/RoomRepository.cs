/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly CRMContext? context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public RoomRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }
        //public List<TableRooms> GetAllRooms()
        //{
        //    return this.context?.TableRooms.ToList<TableRooms>();
        //}
        //

        /// <summary>
        /// Add Room
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        public RoomAddViewModel AddRoom(RoomAddViewModel rooms)
        {
            if (rooms.tableRoom is null)
            {
                return rooms;
            }
            try
            {
                rooms.tableRoom.AcademicYear = Utility.GetCurrentAcademicYear(this.context!, rooms.tableRoom.TenantId, rooms.tableRoom.SchoolId);

                var checkRoomTitle = this.context?.Rooms.AsEnumerable().Where(x => x.SchoolId == rooms.tableRoom.SchoolId && x.TenantId == rooms.tableRoom.TenantId && String.Compare(x.Title,rooms.tableRoom.Title,true)==0 && x.IsActive==true && x.AcademicYear == rooms.tableRoom.AcademicYear).FirstOrDefault();

                if (checkRoomTitle !=null)
                {
                    rooms._failure = true;
                    rooms._message = "Room Title already exists";
                }
                else
                {
                    //int? RoomlId = Utility.GetMaxPK(this.context, new Func<Rooms, int>(x => x.RoomId));

                    int? RoomId = 1;

                    var RoomData = this.context?.Rooms.Where(x => x.SchoolId == rooms.tableRoom.SchoolId && x.TenantId == rooms.tableRoom.TenantId).OrderByDescending(x => x.RoomId).FirstOrDefault();

                    if (RoomData != null)
                    {
                        RoomId = RoomData.RoomId + 1;
                    }

                    if(rooms.tableRoom != null)
                    {
                        rooms.tableRoom.RoomId = (int)RoomId;
                        rooms.tableRoom.CreatedOn = DateTime.UtcNow;
                        rooms.tableRoom.TenantId = rooms.tableRoom.TenantId;
                        rooms.tableRoom.IsActive = rooms.tableRoom.IsActive;
                        this.context?.Rooms.Add(rooms.tableRoom);
                        this.context?.SaveChanges();
                        rooms._failure = false;
                        rooms._message = "Room added successfully";
                    }
                }               
            }
            catch (Exception es)
            {
                rooms._message = es.Message;
                rooms._failure = true;
            }            
            return rooms;
        }

        /// <summary>
        /// Get Room By Id
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public RoomAddViewModel ViewRoom(RoomAddViewModel room)
        {
            if (room.tableRoom is null)
            {
                return room;
            }
            RoomAddViewModel roomAddViewModel = new RoomAddViewModel();
            try
            {
                var roomMaster = this.context?.Rooms.FirstOrDefault(x => x.TenantId == room.tableRoom.TenantId && x.SchoolId == room.tableRoom.SchoolId && x.RoomId == room.tableRoom.RoomId);
                if (roomMaster != null)
                {
                    room.tableRoom = roomMaster;                    
                    room._tenantName = room._tenantName;
                    room._failure = false;
                    return room;
                }
                else
                {
                    roomAddViewModel._failure = true;
                    roomAddViewModel._message = NORECORDFOUND;
                    return roomAddViewModel;
                }
            }
            catch (Exception es)
            {
                roomAddViewModel._failure = true;
                roomAddViewModel._message = es.Message;
                return roomAddViewModel;
            }
        }

        /// <summary>
        /// Update Room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public RoomAddViewModel UpdateRoom(RoomAddViewModel room)
        {
            if (room.tableRoom is null)
            {
                return room;
            }
            try
            {
                var roomMaster = this.context?.Rooms.FirstOrDefault(x => x.TenantId == room.tableRoom.TenantId && x.SchoolId == room.tableRoom.SchoolId && x.RoomId == room.tableRoom.RoomId);
                if (roomMaster != null)
                {
                    var checkRoomTitle = this.context?.Rooms.AsEnumerable().Where(x => x.SchoolId == room.tableRoom.SchoolId && x.TenantId == room.tableRoom.TenantId && x.RoomId != room.tableRoom.RoomId && String.Compare(x.Title, room.tableRoom.Title, true) == 0 && x.IsActive == true && x.AcademicYear == roomMaster.AcademicYear).FirstOrDefault();

                    if (checkRoomTitle != null)
                    {
                        room._failure = true;
                        room._message = "Room Title already exists";
                    }
                    else
                    {
                        //those field are not updated set previous data
                        room.tableRoom.AcademicYear = roomMaster.AcademicYear;
                        room.tableRoom.CreatedBy = roomMaster.CreatedBy;
                        room.tableRoom.CreatedOn = roomMaster.CreatedOn;

                        var courseSectionData = this.context?.AllCourseSectionView.Where(b => b.TenantId == room.tableRoom.TenantId && b.SchoolId == room.tableRoom.SchoolId && (b.CalRoomId == room.tableRoom.RoomId || b.VarRoomId == room.tableRoom.RoomId || b.FixedRoomId == room.tableRoom.RoomId) && b.IsActive == true && b.DurationEndDate!.Value.Date >= DateTime.Today.Date).ToList();

                        if (courseSectionData != null && courseSectionData.Any() == true)
                        {
                            if (room.tableRoom.IsActive == false && roomMaster.IsActive == true)
                            {
                                room._failure = true;
                                room._message = "Room Can Not Be InActive. Because It Has Association";
                                return room;
                            }

                            List<int> studentCountList = new();
                            var csIDs = courseSectionData.Select(s => s.CourseSectionId).Distinct().ToList();
                            foreach (var csId in csIDs)
                            {
                                var studentCoursesectionScheduleData = this.context?.StudentCoursesectionSchedule.Where(x => x.TenantId == room.tableRoom.TenantId && x.SchoolId == room.tableRoom.SchoolId && x.CourseSectionId == csId && x.IsDropped != true).ToList();
                                if (studentCoursesectionScheduleData?.Any() == true)
                                {
                                    studentCountList.Add(studentCoursesectionScheduleData.Count);
                                }
                            }
                            var maxCount = studentCountList.Count > 0 ? studentCountList.Max() : 0;
                            if (room.tableRoom.Capacity < maxCount)
                            {
                                room.tableRoom.Capacity = roomMaster.Capacity;
                                this.context?.Entry(roomMaster).CurrentValues.SetValues(room.tableRoom);
                                this.context?.SaveChanges();
                                room._failure = false;
                                room._message = "Room capacity could not be updated as it has association";
                            }
                            else
                            {
                                this.context?.Entry(roomMaster).CurrentValues.SetValues(room.tableRoom);
                                this.context?.SaveChanges();
                                room._failure = false;
                                room._message = "Room Updated Successfully";
                            }
                        }
                        else
                        {
                            this.context?.Entry(roomMaster).CurrentValues.SetValues(room.tableRoom);
                            this.context?.SaveChanges();
                            room._failure = false;
                            room._message = "Room Updated Successfully";
                        }
                    }
                }
                else
                {
                    room.tableRoom = null;
                    room._failure = true;
                    room._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {
                room._failure = true;
                room._message = ex.Message;
            }
            return room;
        }

        public RoomAddViewModel UpdateRoom_old(RoomAddViewModel room)
        {
            if (room.tableRoom is null)
            {
                return room;
            }
            try
            {
                var roomMaster = this.context?.Rooms.FirstOrDefault(x => x.TenantId == room.tableRoom.TenantId && x.SchoolId == room.tableRoom.SchoolId && x.RoomId == room.tableRoom.RoomId);
                if (roomMaster != null)
                {
                    var checkRoomTitle = this.context?.Rooms.AsEnumerable().Where(x => x.SchoolId == room.tableRoom.SchoolId && x.TenantId == room.tableRoom.TenantId && x.RoomId != room.tableRoom.RoomId && String.Compare(x.Title, room.tableRoom.Title, true) == 0 && x.IsActive == true && x.AcademicYear == roomMaster.AcademicYear).FirstOrDefault();

                    if (checkRoomTitle != null)
                    {
                        room._failure = true;
                        room._message = "Room Title already exists";
                    }
                    else
                    {
                        //room.tableRoom.UpdatedOn = DateTime.UtcNow;
                        //room.tableRoom.CreatedOn = roomMaster.CreatedOn;
                        //room.tableRoom.CreatedBy = roomMaster.CreatedBy;
                        //this.context.Entry(roomMaster).CurrentValues.SetValues(room.tableRoom);
                        //this.context?.SaveChanges();
                        //room._failure = false;
                        //room._message = "Room Updated Successfully";

                        //var courseSectionData = this.context?.AllCourseSectionView.FirstOrDefault(b => b.TenantId == room.tableRoom.TenantId && b.SchoolId == room.tableRoom.SchoolId && (b.CalRoomId == room.tableRoom.RoomId || b.VarRoomId == room.tableRoom.RoomId || b.FixedRoomId == room.tableRoom.RoomId) && b.IsActive == true && b.DurationEndDate.Value.Date>=DateTime.Today.Date);

                        var courseSectionData = this.context?.AllCourseSectionView.FirstOrDefault(b => b.TenantId == room.tableRoom.TenantId && b.SchoolId == room.tableRoom.SchoolId && (b.CalRoomId == room.tableRoom.RoomId || b.VarRoomId == room.tableRoom.RoomId || b.FixedRoomId == room.tableRoom.RoomId) && b.IsActive == true && b.DurationEndDate!.Value.Date >= DateTime.Today.Date);

                        if (courseSectionData != null)
                        {
                            room._failure = false;
                            room._message = "Room Can Not Be Updated. Because It Has Association";
                            return room;
                        }
                        else
                        {
                            if (room.tableRoom != null)
                            {
                                room.tableRoom.AcademicYear = roomMaster.AcademicYear;
                                room.tableRoom.CreatedBy = roomMaster.CreatedBy;
                                room.tableRoom.CreatedOn = roomMaster.CreatedOn;
                                this.context?.Entry(roomMaster).CurrentValues.SetValues(room.tableRoom);
                                this.context?.SaveChanges();
                                room._failure = false;
                                room._message = "Room Updated Successfully";
                            }
                        }
                    }
                }
                else
                {
                    room.tableRoom = null;
                    room._failure = true;
                    room._message = NORECORDFOUND;
                }
            }
            catch (Exception ex)
            {

                room._failure = true;
                room._message = ex.Message;

            }
            return room;
        }

        /// <summary>
        /// Get All Room
        /// </summary>
        /// <param name="roomList"></param>
        /// <returns></returns>
        public RoomListModel GetAllRooms(RoomListModel roomList)
        {
            RoomListModel roomListModel = new RoomListModel(); 
            try
            {
                var room = this.context?.Rooms.Where(x => x.TenantId == roomList.TenantId && x.SchoolId == roomList.SchoolId && (roomList.IncludeInactive == false || roomList.IncludeInactive == null ? x.IsActive != false : true) && x.AcademicYear == roomList.AcademicYear).OrderBy(x => x.SortOrder).ToList();

                if (room != null && room.Any())
                {
                    if (roomList.IsListView == true)
                    {
                        room.ForEach(c =>
                        {
                            c.CreatedBy = Utility.CreatedOrUpdatedBy(this.context, roomList.TenantId, c.CreatedBy!=null ? c.CreatedBy : "");
                            c.UpdatedBy = Utility.CreatedOrUpdatedBy(this.context, roomList.TenantId, c.UpdatedBy!=null ? c.UpdatedBy : "");
                        });
                    }

                    roomListModel.TableroomList = room;
                    roomListModel._tenantName = roomList._tenantName;
                    roomListModel._token = roomList._token;
                    roomListModel._failure = false;
                }
                else
                {
                    roomListModel._failure = true;
                    roomListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                roomListModel._message = es.Message;
                roomListModel._failure = true;
                roomListModel._tenantName = roomList._tenantName;
                roomListModel._token = roomList._token;
            }
            return roomListModel;

        }
        
        /// <summary>
        /// Delete Room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public RoomAddViewModel DeleteRoom(RoomAddViewModel room)
        {
            if (room.tableRoom is null)
            {
                return room;
            }
            try
            {
                var Room= this.context?.Rooms.FirstOrDefault(x => x.TenantId == room.tableRoom.TenantId && x.SchoolId == room.tableRoom.SchoolId && x.RoomId == room.tableRoom.RoomId);

                if (Room != null)
                {
                    var courseSectionData= this.context?.AllCourseSectionView.FirstOrDefault(b => b.TenantId == room.tableRoom.TenantId && b.SchoolId == room.tableRoom.SchoolId && (b.CalRoomId == room.tableRoom.RoomId || b.VarRoomId == room.tableRoom.RoomId || b.FixedRoomId == room.tableRoom.RoomId) && b.IsActive == true && b.DurationEndDate!.Value.Date >= DateTime.Today.Date);

                    if (courseSectionData != null)
                    {
                        room._failure = true;
                        room._message = "Room Can Not Be Deleted. Because It Has Association";
                    }
                    else
                    {
                        this.context?.Rooms.Remove(Room);
                        this.context?.SaveChanges();
                        room._failure = false;
                        room._message = "Room deleted successfullyy";
                    }
                }
                //this.context?.Rooms.Remove(Room);
                //this.context?.SaveChanges();
                //room._failure = false;
                //room._message = "Room deleted successfullyy";
            }
            catch (Exception es)
            {
                room._failure = true;
                room._message = es.Message;
            }
            return room;
        }

        /// <summary>
        /// Update Room Sort Order
        /// </summary>
        /// <param name="roomSortOrderViewModel"></param>
        /// <returns></returns>
        public RoomSortOrderViewModel UpdateRoomSortOrder(RoomSortOrderViewModel roomSortOrderViewModel)
        {
            try
            {
                var roomsRecords = new List<Rooms>();
                var targetRoom = this.context?.Rooms.FirstOrDefault(x => x.SortOrder == roomSortOrderViewModel.PreviousSortOrder && x.SchoolId == roomSortOrderViewModel.SchoolId && x.TenantId == roomSortOrderViewModel.TenantId && x.AcademicYear == roomSortOrderViewModel._academicYear);
                if (targetRoom != null)
                {
                    targetRoom.SortOrder = roomSortOrderViewModel.CurrentSortOrder;
                    targetRoom.UpdatedBy = roomSortOrderViewModel.UpdatedBy;
                    targetRoom.UpdatedOn = DateTime.UtcNow;
                    if (roomSortOrderViewModel.PreviousSortOrder > roomSortOrderViewModel.CurrentSortOrder)
                    {
                        roomsRecords = this.context?.Rooms.Where(x => x.SortOrder >= roomSortOrderViewModel.CurrentSortOrder && x.SortOrder < roomSortOrderViewModel.PreviousSortOrder && x.TenantId == roomSortOrderViewModel.TenantId && x.SchoolId == roomSortOrderViewModel.SchoolId && x.AcademicYear == roomSortOrderViewModel._academicYear).ToList();

                        if (roomsRecords != null && roomsRecords.Any())
                        {
                            roomsRecords.ForEach(x => { x.SortOrder = x.SortOrder + 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = roomSortOrderViewModel.UpdatedBy; });
                        }
                    }
                    if (roomSortOrderViewModel.CurrentSortOrder > roomSortOrderViewModel.PreviousSortOrder)
                    {
                        roomsRecords = this.context?.Rooms.Where(x => x.SortOrder <= roomSortOrderViewModel.CurrentSortOrder && x.SortOrder > roomSortOrderViewModel.PreviousSortOrder && x.SchoolId == roomSortOrderViewModel.SchoolId && x.TenantId == roomSortOrderViewModel.TenantId && x.AcademicYear == roomSortOrderViewModel._academicYear).ToList();
                        if (roomsRecords != null && roomsRecords.Any())
                        {
                            roomsRecords.ForEach(x => { x.SortOrder = x.SortOrder - 1; x.UpdatedOn = DateTime.UtcNow; x.UpdatedBy = roomSortOrderViewModel.UpdatedBy; });
                        }
                    }
                    this.context?.SaveChanges();
                    roomSortOrderViewModel._failure = false;
                }
            }
            catch (Exception es)
            {
                roomSortOrderViewModel._message = es.Message;
                roomSortOrderViewModel._failure = true;
            }
            return roomSortOrderViewModel;
        }
    }
}
    
