using Microsoft.Extensions.Logging;
using RBA.DBase.Managers.Admin;
using RBA.DBase.Managers.DbWorker;
using RBA.Models.Models.RoomBookingModels;
using RBA.Models.Request.Admin.Cancel;
using RBA.Models.Request.Admin.Login;
using RBA.Models.Request.Admin.Logout;
using RBA.Models.Request.Admin.NewRoom;
using RBA.Models.Request.Admin.Reservations;
using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.DBase.Workers.AdminWorkers
{
    public class AdminWorker(IAdminDbManager dbWorker, ILogger<AdminWorker> logger) : IAdminManagment
    {
        public Task<bool> LoginAsync(LoginRequest request)
        {
            logger.LogInformation("AdminWorker: LoginAsync called with UserName: {UserName}", request.UserName);
            var result =  dbWorker.LoginAsync(request);
            logger.LogInformation("AdminWorker: LoginAsync completed with result: {Result}", result);
            return result;
        }

        public Task<bool> LogoutAsync(LogoutRequest request)
        {
            logger.LogInformation("AdminWorker: LogoutAsync called");
            var result = dbWorker.LogoutAsync(request);
            logger.LogInformation("AdminWorker: LogoutAsync completed with result: {Result}", result);
            return result;
        }

        public Task<RoomModel> InsertNewRoom(NewRoomRequest roomRequest)
        {
            logger.LogInformation("AdminWorker: InsertNewRoom called with RoomName: {RoomName}", roomRequest.Name);
            var result = dbWorker.InsertNewRoom(roomRequest);
            logger.LogInformation("AdminWorker: InsertNewRoom completed with result: {Result}", result);
            return result;
        }

        public Task<List<AmenityModel>> GetRoomConfigs()
        {
            logger.LogInformation("AdminWorker: GetRoomConfigs called");
            var result = dbWorker.GetRoomConfigs();
            logger.LogInformation("AdminWorker: GetRoomConfigs completed with result: {Result}", result);
            return result;
        }

        public Task<int> GetTotalBookings()
        {
            logger.LogInformation("AdminWorker: GetTotalBookings called");
            var result = dbWorker.GetTotalBookings();
            logger.LogInformation("AdminWorker: GetTotalBookings completed with result: {Result}", result);
            return result;
        }

        public Task<int> GetTodayBookings()
        {
            logger.LogInformation("AdminWorker: GetTodayBookings called");
            var result = dbWorker.GetTodayBookings();
            logger.LogInformation("AdminWorker: GetTodayBookings completed with result: {Result}", result);
            return result;
        }

        public Task<int> GetAllRoomsCount()
        {
            logger.LogInformation("AdminWorker: GetAllRoomsCount called");
            var result = dbWorker.GetAllRoomsCount();
            logger.LogInformation("AdminWorker: GetAllRoomsCount completed with result: {Result}", result);
            return result;
        }

        public Task<List<ReservationModel>> GetAllReservations()
        {
            logger.LogInformation("AdminWorker: GetAllReservations called");
            var result = dbWorker.GetAllReservations();
            logger.LogInformation("AdminWorker: GetAllReservations completed with result: {Result}", result);
            return result;
        }

        public Task<bool> CancelBookingAsync(CancelBookingRequest request)
        {
            logger.LogInformation("AdminWorker: CancelBookingAsync called");
            var result = dbWorker.CancelBookingAsync(request);
            logger.LogInformation("AdminWorker: CancelBookingAsync completed with result: {Result}", result);
            return result;
        }
    }
}